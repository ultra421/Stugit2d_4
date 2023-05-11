using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class ServerManager : MonoBehaviour
{
    public NetworkDriver driver;
    protected List<NetworkConnection> connections;

    private void Start()
    {
        ServerInfoTransfer transfer = GameObject.Find("InfoTransfer").GetComponent<ServerInfoTransfer>();
        Debug.Log("Got Info from transfer" + transfer.username);

        return;
        //Init the driver
        driver = NetworkDriver.Create();
        //Set any ip to be able to connect
        NetworkEndPoint endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = 5522;
        if (driver.Bind(endpoint) != 0)
        {
            Debug.LogError("Error at connection port" + endpoint.Port);
        } else
        {
            driver.Listen();
        }
        connections = new List<NetworkConnection>(4);
    }

    private void OnDestroy()
    {
        driver.Dispose();
        connections.Clear();
    }

    private void Update()
    {
        driver.ScheduleUpdate().Complete();
        CleanupConnections();
        AcceptNewConnections();
        UpdateMessagePump();
    }

    private void CleanupConnections()
    {
        for (int i = 0; i < connections.Count; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAt(i);
                i--;
            }
        }
    }

    private void AcceptNewConnections()
    {
        NetworkConnection c;
        while ((c = driver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c);
            Debug.Log("Accepted a connection " + c.GetHashCode());
        }
    }

    private void UpdateMessagePump()
    {
        DataStreamReader stream;
        for (int i = 0; i < connections.Count; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                //If data is sent
                if (cmd == NetworkEvent.Type.Data)
                {
                    OnData(stream);
                }
                //If player disconnects
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client diesconnected from server");
                    connections[i] = default(NetworkConnection);
                }
            }
        }
    }

    public void OnData(DataStreamReader stream)
    {
        NetMessage msg = null;
        //First byte of message will be the operation code
        var opCode = (OpCode)stream.ReadByte();
        Debug.Log("Recieved OpCode" + opCode);
        switch (opCode)
        {
            case OpCode.PLAYER_POSITION: msg = new NetPlayerPos(stream); break;
            default:
                Debug.Log("Message recieved had no OPCode " + opCode);
                break;
        }

        //Message has been processed and reecieved on server
        msg.ReceivedOnServer(this);
    }

    public void BroadCast(NetMessage msg)
    {
        foreach(NetworkConnection connection in connections)
        {
            if (connection.IsCreated)
            {
                SendToClient(connection, msg);
            }
        }
    }

    public void SendToClient(NetworkConnection connection, NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }

}