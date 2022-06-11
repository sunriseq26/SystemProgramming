using System;
using System.Collections.Generic;
using Characters;
using UnityEngine;
using Mirror;
using Mirror.Examples.Chat;

namespace Main
{
    public class SolarSystemNetworkManager : NetworkManager
    {
        // public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        // {
        //     var spawnTransform = GetStartPosition();
        //     var name = GetComponent<NetworkManagerHUD>().PlayerName;
        //     var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
        //     player.GetComponent<ShipController>().PlayerName = name;
        //     Debug.Log($"Name: {player.GetComponent<ShipController>().PlayerName}");
        //
        //     NetworkServer.AddPlayerForConnection(conn, player);
        //
        // }
    }
}
