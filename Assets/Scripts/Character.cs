using System;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
public abstract class Character : NetworkBehaviour
{
    protected Action OnUpdateAction { get; set; }
    protected abstract FireAction fireAction { get; set; }

    [SyncVar] protected Vector3 serverPosition;
    [SyncVar] protected Quaternion serverRotation;

    protected virtual void Initiate()
    {
        OnUpdateAction += Movement;
    }

    private void Update()
    {
        OnUpdate();
    }

    private void OnUpdate()
    {
        OnUpdateAction?.Invoke();
    }

    [Command]
    protected void CmdUpdateTransform(Vector3 position, Quaternion rotation)
    {
        serverPosition = position;
        serverRotation = rotation;
    }

    public abstract void Movement();
}
