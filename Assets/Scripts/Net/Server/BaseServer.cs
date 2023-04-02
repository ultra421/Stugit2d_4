using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using 

public class BaseServer : MonoBehaviour
{
    public NetworkDriver driver;
    protected List<NetworkConnection> connections;

#if UNITY_EDITOR
    private void Start() { Init(); }
    private void Update() { UpdateServer(); }
    private void OnDestroy() { ShutDown(); }

#endif

    public virtual void Init()
    {
        //Init the driver
        driver = NetworkDriver.Create();
        NetworkEndPoint endpoint = NetworkEndPoint.AnyIpv4; //Who can connect to us
        endpoint.Port = 5522;
        if (driver.Bind(endpoint) != 0)
        {
            Debug.LogError("There was an error binding to port " + endpoint.Port);
        }
        else
        {
            driver.Listen();
        }
        //Init the connection list
        connections = new List<NetworkConnection>(4);

    }
    public virtual void ShutDown()
    {
        driver.Dispose();
        connections.Clear();
    }
    public virtual void UpdateServer()
    {
        driver.ScheduleUpdate().Complete(); //Unity Job system, so thread is unlocked?
        CleanupConnections(); //If connections fails remove
        AcceptNewConnections(); //Accept incoming connections
        UpdateMessagePump(); //Read messages
    }

    private void CleanupConnections()
    {
        for (int i = 0; i < connections.Count; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAt(i);
                i--; //Go back as removeAt shifts the array by -1 when delete
            }
        }
    }

    private void AcceptNewConnections()
    {
        NetworkConnection c;
        while ((c = driver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c);
            Debug.Log("Accepted a connection " + c);
        }
    }

    protected virtual void UpdateMessagePump()
    {
        DataStreamReader stream;
        for (int i = 0; i < connections.Count; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = driver.PopEventForConnection(connections[i],out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    uint number = stream.ReadByte();
                    Debug.Log("Got " + number + " from Client");
                } 
                else if  (cmd == NetworkEvent.Type.Disconnect){
                    Debug.Log("Client diesconnected from server");
                    connections[i] = default(NetworkConnection);
                }
            }
        }
    }

}
