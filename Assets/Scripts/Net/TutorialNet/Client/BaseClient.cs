using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class BaseClient : MonoBehaviour
{
    public NetworkDriver driver;
    protected NetworkConnection connection;

#if UNITY_EDITOR
    private void Start() { Init(); }
    private void Update() { UpdateServer(); }
    private void OnDestroy() { ShutDown(); }

#endif

    public virtual void Init()
    {
        //Init the driver
        driver = NetworkDriver.Create();
        connection = default(NetworkConnection); //just in case

        NetworkEndPoint endpoint = NetworkEndPoint.LoopbackIpv4; //Who can connect to us
        endpoint.Port = 5522;
        connection = driver.Connect(endpoint);

    }
    public virtual void ShutDown()
    {
        driver.Dispose();
    }
    public virtual void UpdateServer()
    {
        driver.ScheduleUpdate().Complete(); //Unity Job system, so the thread unlockes?
        CheckAlive();
        UpdateMessagePump(); //Read messages
    }
    private void CheckAlive()
    {
        if (!connection.IsCreated)
        {
            Debug.LogError("Something wren wrong with the connection");
        }
    }
    protected virtual void UpdateMessagePump()
    {
        DataStreamReader stream;
        NetworkEvent.Type cmd;

        while ((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We have connected with the server");
            } 
            else if (cmd == NetworkEvent.Type.Data) {
                OnData(stream);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client diesconnected from server");
                connection = default(NetworkConnection);
            }
        }
    }
    public virtual void OnData(DataStreamReader stream)
    {
        NetMessage msg = null; //Unknown type of message
        //Get operation code from first byte of message
        var opCode = (OpCode)stream.ReadByte();
        Debug.Log("Recieved OpCode" + opCode);
        switch (opCode)
        {
            case OpCode.CHAT_MESSAGE: msg = new NetChatMessage(stream); break;
            case OpCode.PLAYER_POSITION: msg = new NetPlayerPos(stream); break;
            default:
                Debug.Log("Message recieved had no OpCode " + opCode);
                break;
        }

        //Should already be parsed with the constructor
        msg.ReceivedOnClient();
    }
    public virtual void SendToServer(NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        //BeginSend returns a driver ready to write for the connnection (maybe)
        msg.Serialize(ref writer);
        //Message has been written
        driver.EndSend(writer);
    }

}
