using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class ClientManager : MonoBehaviour
{
    public NetworkDriver driver;
    public NetworkConnection connection;
    private void Start()
    {
        driver = NetworkDriver.Create();
        connection = default(NetworkConnection);
        DontDestroyOnLoad(this);
    }

    private void BuildConnection(string ip, int port)
    {
        NetworkEndPoint endpoint = NetworkEndPoint.LoopbackIpv4;
        endpoint.Port = 5522;
        connection = driver.Connect(endpoint);
    }
}
