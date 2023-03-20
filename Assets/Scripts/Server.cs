using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

public class Server : MonoBehaviour
{
    public NetworkDriver driver;
    public string ipAddress = "127.0.0.1";
    public ushort port = 9000;
        
    // Start is called before the first frame update
    void Start()
    {
        driver = NetworkDriver.Create();
        var exportDir = NetworkEndPoint.Parse(ipAddress, port);
        if (driver.Bind(exportDir) != 0 ){
            Debug.LogError("Could connect to port");
        } else
        {
            driver.Listen();
            Debug.Log("Server listening");
        }

    }

    // Update is called once per frame
    void Update()
    {
        HandleConnections();

    }

    private void readIncomingMessages()
    {
        foreach(var connection in connections)
        {
            DataStreamReader readStream;
            NetworkEvent.Type eventType = driver.PopEventForConnection(connection, out readStream);
            while (eventType != NetworkEvent.Type.Empty)
            {
                switch (eventType)
                {
                    case NetworkEvent.Type.Data:
                        FixedString128Bytes text = readStream.ReadFixedString128();
                        Debug.Log("Servidor got " + text);
                        break;
                    case NetworkEvent.Type.Disconnect:
                        Debug.Log("Type disconnect");
                        break;
                    default:
                        Debug.Log("Unkown event type " + eventType);
                        break;
                }
            }
        }
    }

    private NativeList<NetworkConnection> connections;

    private void HandleConnections()
    {
        driver.ScheduleUpdate().Complete();

        for (int k = 0; k < connections.Length; k++)
        {
            if (!connections[k].IsCreated)
            {
                connections.RemoveAtSwapBack(k);
                k--;
            }
        }

        NetworkConnection newConnection = driver.Accept();
        while (newConnection != default(NetworkConnection))
        {
            connections.Add(newConnection);
            Debug.Log("New connection accepted");
            newConnection = driver.Accept();

        }
    }

    private void

    private void OnDestroy()
    {
        if (driver.IsCreated)
        {
            driver.Dispose();
        }
        Debug.Log("Server has been stopped");   
    }
}
