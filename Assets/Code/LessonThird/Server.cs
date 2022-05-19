using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{  
    private const int MAX_CONNECTION = 10;

    private int port = 5805;

    private int hostID;
    private int reliableChannel;

    private bool isStarted = false;
    private byte error;

    Dictionary<int, string> connectionIdAndNames = new Dictionary<int, string>();
    Dictionary<int, bool> nameEntered = new Dictionary<int, bool>();

    public void StartServer()
    {              
        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();         
        reliableChannel = cc.AddChannel(QosType.Reliable);

        HostTopology topology = new HostTopology(cc, MAX_CONNECTION); 
        hostID = NetworkTransport.AddHost(topology, 5805);

        isStarted = true;
    }

    void Update()
    {
        if (!isStarted)
            return;

        int recHostId;
        int connectionId;
        int channelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);

        while (recData != NetworkEventType.Nothing)
        {
            switch (recData)
            {
                case NetworkEventType.Nothing:
                    break;

                case NetworkEventType.ConnectEvent:
                    nameEntered.Add(connectionId, false);
                    
                    Debug.Log($"Player {connectionId} has connected.");
                    break;

                case NetworkEventType.DataEvent:
                    string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    if (nameEntered[connectionId] == false)
                    {
                        nameEntered[connectionId] = true;
                        connectionIdAndNames.Add(connectionId, message);
                        SendMessageToAll($"{connectionIdAndNames[connectionId]} has connected.");
                    }
                    else
                    {
                        SendMessageToAll($"{connectionIdAndNames[connectionId]}: {message}");
                        Debug.Log($"{connectionIdAndNames[connectionId]}: {message}");
                    }
                    break;

                case NetworkEventType.DisconnectEvent:
                    connectionIdAndNames.Remove(connectionId);

                    SendMessageToAll($"{connectionIdAndNames[connectionId]} has disconnected.");
                    Debug.Log($"{connectionIdAndNames[connectionId]} has disconnected.");
                    break;

                case NetworkEventType.BroadcastEvent:
                    break;

            }

            recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);            
        }
    }

    public void ShutDownServer()
    {
        if (!isStarted)
            return;

        NetworkTransport.RemoveHost(hostID);
        NetworkTransport.Shutdown();
        isStarted = false;
    }

    public void SendMessage(string message, int connectionID)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(message);
                
        NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, message.Length * sizeof(char), out error);
        if ((NetworkError)error != NetworkError.Ok)
            Debug.Log((NetworkError)error);
    }

    public void SendMessageToAll(string message)
    {
        for (int i = 0; i < connectionIdAndNames.Count; i++)        
            SendMessage(message, connectionIdAndNames[i]);        
    }

}