using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class ServerManager : MonoBehaviour
{
    public NetworkDriver driver;
    protected List<NetworkConnection> connections;
    public static ServerManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            this.gameObject.SetActive(false);
            return;
        }

        ServerInfoTransfer transfer = ServerInfoTransfer.Instance;
        if (transfer.connectionType != ConnectionType.HOST) { this.gameObject.SetActive(false); return; }
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
            Debug.Log("Server is listening!");
            driver.Listen();
        }
        connections = new List<NetworkConnection>(4);
    }

    private void OnDestroy()
    {
        return;
        driver.Dispose();
        connections.Clear();
    }

    private void FixedUpdate()
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
            SendNewPlayerInfo(c);
        }
    }

    private void SendNewPlayerInfo(NetworkConnection c)
    {
        PlayerListManager plm = PlayerListManager.Instance;
        byte newPlayerId = (byte)plm.getPlayerList().Count;

        //First send the controllable player
        NetNewPlayer message = new NetNewPlayer(newPlayerId, true, Vector3.zero);
        SendToClient(c, message);
        Debug.Log("Sent to client their character");
        
        //Then send the players ingame to new connection (uncontrollable)
        foreach(KeyValuePair<byte,GameObject> pair in plm.getPlayerList())
        {
            Debug.Log("Sending playerId " + pair.Key + " to Id " + message.playerId);
            NetNewPlayer message2 = new NetNewPlayer(pair.Key, false, pair.Value.transform.position);
            SendToClient(c, message2);
        }
        Debug.Log("Sent other player's info to client");

        //FInally send the existing players the new player info
        NetNewPlayer message3 = new NetNewPlayer(newPlayerId, false, Vector3.zero);
        BroadCastExcept(message3,c); //Broadcasts except to one connection
        Debug.Log("Sent new player to other players");
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
        //Debug.Log("Recieved OpCode" + opCode);
        switch (opCode)
        {
            case OpCode.PLAYER_POSITION: msg = new NetPlayerPos(stream); break;
            case OpCode.BALL_MESSAGE: msg = new NetBallMessage(stream); break;
            case OpCode.UPDATE_SCORE: msg = new NetUpdateScore(stream); break;

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

    public void BroadCastExcept(NetMessage msg, NetworkConnection except)
    {
        foreach (NetworkConnection connection in connections)
        {
            if (connection.IsCreated && connection != except)
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
