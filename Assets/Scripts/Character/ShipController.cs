using System;
using Main;
using Mechanics;
using Network;
using UI;
using UnityEngine;
using Mirror;

namespace Characters
{
    public class ShipController : NetworkMovableObject
    {
        [SerializeField] private Transform _cameraAttach;
        private CameraOrbit _cameraOrbit;
        private PlayerLabel playerLabel;
        private float _shipSpeed;
        private Rigidbody _rigidbody;

        [SyncVar] public string _playerName;

        private Vector3 currentPositionSmoothVelocity;
        private Vector3 startPosition;

        protected override float speed => _shipSpeed;

        public string PlayerName
        {
            get => _playerName;
            set => _playerName = value;
        }

        private void OnGUI()
        {
            if (_cameraOrbit == null)            
                return;
            
            _cameraOrbit.ShowPlayerLabels(playerLabel);
        }

        public override void OnStartAuthority()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)            
                return;

            _cameraOrbit = FindObjectOfType<CameraOrbit>();
            _cameraOrbit.Initiate(_cameraAttach == null ? transform : _cameraAttach);
            playerLabel = GetComponent<PlayerLabel>();
            startPosition = transform.position;
            base.OnStartAuthority();
        }

        protected override void HasAuthorityMovement()
        {
            var spaceShipSettings = SettingsContainer.Instance?.SpaceShipSettings;
            if (spaceShipSettings == null)            
                return;            

            var isFaster = Input.GetKey(KeyCode.LeftShift);
            var speed = spaceShipSettings.ShipSpeed;
            var faster = isFaster ? spaceShipSettings.Faster : 1.0f;

            _shipSpeed = Mathf.Lerp(_shipSpeed, speed * faster, spaceShipSettings.Acceleration);

            var currentFov = isFaster ? spaceShipSettings.FasterFov : spaceShipSettings.NormalFov;
            _cameraOrbit.SetFov(currentFov, spaceShipSettings.ChangeFovSpeed);

            var velocity = _cameraOrbit.transform.TransformDirection(Vector3.forward) * _shipSpeed;
            _rigidbody.velocity = velocity * (_updatePhase == UpdatePhase.FixedUpdate ? Time.fixedDeltaTime : Time.deltaTime);

            if (!Input.GetKey(KeyCode.C))
            {
                var targetRotation = Quaternion.LookRotation(Quaternion.AngleAxis(_cameraOrbit.LookAngle, -transform.right) * velocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            }

            if (isServer)
            {
                SendToClients();
            }
            else
            {
                CmdSendTransform(transform.position, transform.rotation.eulerAngles);
            }
        }

        protected override void FromOwnerUpdate()
        {
            transform.position = Vector3.SmoothDamp(transform.position, serverPosition, ref currentPositionSmoothVelocity, speed);
            transform.rotation = Quaternion.Euler(serverEulers);                       
        }

        protected override void SendToClients()
        {
            serverPosition = transform.position;
            serverEulers = transform.eulerAngles;
        }

        [Command]
        private void CmdSendTransform(Vector3 position, Vector3 eulers)
        {
            serverPosition = position;
            serverEulers = eulers;
        }

        [ClientCallback]
        private void LateUpdate()
        {
            _cameraOrbit?.CameraMovement();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlanetOrbit>() || other.GetComponent<ShipController>())
            {
                NetworkServer.SetClientNotReady(connectionToClient);
                transform.position = startPosition;
                NetworkServer.SetClientReady(connectionToClient);
            }
        }
    }
}
