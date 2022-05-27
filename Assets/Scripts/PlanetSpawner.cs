using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlanetSpawner : NetworkBehaviour
{
    [SerializeField] PlanetOrbit _planetPrefab;

    public override void OnStartServer()
    {
        base.OnStartServer();

        GameObject planetInstance = Instantiate(_planetPrefab.gameObject);
        planetInstance.name = _planetPrefab.name;
        Debug.Log(planetInstance.name);
        NetworkServer.Spawn(planetInstance, NetworkServer.localConnection);

        Destroy(gameObject);
    }
}
