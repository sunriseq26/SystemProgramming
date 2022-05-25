using System.Collections;
using UnityEngine;
using Mirror;

public class PlayerCharacter : Character
{
    struct MyMessage: NetworkMessage
    {
        //
    }


    [Range(0, 100)] [SerializeField] private int health = 100;

    [Range(0.5f, 10.0f)] [SerializeField] private float movingSpeed = 8.0f;
    [SerializeField] private float acceleration = 3.0f;
    private const float gravity = -9.8f;
    private CharacterController characterController;
    private MouseLook mouseLook;

    private Vector3 currentVelocity;

    protected override FireAction fireAction { get; set; }

    public NetworkConnection connection { get; set; }

    private void MyMessageHandler(NetworkConnectionToClient networkConnectionToClient, MyMessage message)
    {

    }

    protected override void Initiate()
    {
        NetworkServer.RegisterHandler<MyMessage>(MyMessageHandler);
        NetworkServer.UnregisterHandler<MyMessage>();


        base.Initiate();
        fireAction = gameObject.AddComponent<RayShooter>();
        fireAction.Init(this);
        fireAction.Reloading();
        characterController = GetComponentInChildren<CharacterController>();
        characterController ??= gameObject.AddComponent<CharacterController>();
        mouseLook = GetComponentInChildren<MouseLook>();
        mouseLook ??= gameObject.AddComponent<MouseLook>();
    }

    [ServerCallback]
    public void GetDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
            connection.Disconnect();
    }

    public override void Movement()
    {
        if (mouseLook != null && mouseLook.PlayerCamera != null)
        {
            mouseLook.PlayerCamera.enabled = hasAuthority;
        }

        if (hasAuthority)
        {
            var moveX = Input.GetAxis("Horizontal") * movingSpeed;
            var moveZ = Input.GetAxis("Vertical") * movingSpeed;
            var movement = new Vector3(moveX, 0, moveZ);
            movement = Vector3.ClampMagnitude(movement, movingSpeed);
            movement *= Time.deltaTime;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                movement *= acceleration;
            }

            movement.y = gravity;
            movement = transform.TransformDirection(movement);

            characterController.Move(movement);
            mouseLook.Rotation();

            CmdUpdateTransform(transform.position, transform.rotation);

        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, serverPosition, ref currentVelocity, movingSpeed * Time.deltaTime);
            transform.rotation = serverRotation;
        }
    }

    private void Start()
    {
        Initiate();
    }
    private void OnGUI()
    {
        if (Camera.main == null)
        {
            return;
        }

        var info = $"Health: {health}\nClip: {fireAction.bulletCount}";
        var size = 12;
        var bulletCountSize = 50;
        var posX = Camera.main.pixelWidth / 2 - size / 4;
        var posY = Camera.main.pixelHeight / 2 - size / 2;
        var posXBul = Camera.main.pixelWidth - bulletCountSize * 2;
        var posYBul = Camera.main.pixelHeight - bulletCountSize;
        GUI.Label(new Rect(posX, posY, size, size), "+");
        GUI.Label(new Rect(posXBul, posYBul, bulletCountSize * 2, bulletCountSize * 2), info);
    }
    
    [Command]
    internal void CmdShoot(Ray ray)
    {
        var hits = Physics.RaycastAll(ray);

        PlayerCharacter ourPlayer = GetComponent<PlayerCharacter>();
        PlayerCharacter otherPlayer = null;
        Transform targetTransform = null;
        for (int i = hits.Length - 1; i > -1; --i)
        {
            PlayerCharacter playerCharacter = hits[i].transform.GetComponent<PlayerCharacter>();
            if (playerCharacter == null)
            {
                targetTransform = hits[i].transform;
                break;
            }

            if (playerCharacter.netId == ourPlayer.netId)
            {
                continue;
            }
            else
            {
                otherPlayer = playerCharacter;
                targetTransform = hits[i].transform;
                break;
            }
        }
        
        otherPlayer?.GetDamage(FireAction.DAMAGE);
        
        TargetShootResult(ourPlayer.connection, targetTransform != null, targetTransform.position);
    }
    
    [TargetRpc]
    private void TargetShootResult(NetworkConnection connect, bool isShootSucces, Vector3 position)
    {
        if (isShootSucces)
        {
            StartCoroutine(ShootResult(position));
        }
    }
    
    private IEnumerator ShootResult(Vector3 position)
    {
        var shoot = fireAction.Bullets.Dequeue();
        fireAction.bulletCount = fireAction.Bullets.Count.ToString();
        fireAction.Ammunition.Enqueue(shoot);
        shoot.SetActive(true);
        shoot.transform.position = position;
    
        yield return new WaitForSeconds(2.0f);
        shoot.SetActive(false);
    }
}
