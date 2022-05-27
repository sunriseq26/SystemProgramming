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
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            var spawnTransform = GetStartPosition();
            
            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            //_players.Add(conn.connectionId, player.GetComponent<ShipController>());
            //_players[conn.connectionId].onPlayerCollided += OnPlayerCollided;
            Debug.Log($"Name: {player.GetComponent<ShipController>().PlayerName}");

            NetworkServer.AddPlayerForConnection(conn, player);

        }
    }
}
