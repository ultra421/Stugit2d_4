using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using UnityEngine.SceneManagement;

public class ClientManager : MonoBehaviour
{
    public NetworkDriver driver;
    public NetworkConnection connection;
    public static ClientManager Instance;
    public int playerId = -1;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        driver = NetworkDriver.Create();
        connection = default(NetworkConnection);
        DontDestroyOnLoad(this);
        string ip = ServerInfoTransfer.Instance.ip;
        BuildConnection(ip, 5522);
    }

    private void BuildConnection(string ip, int port)
    {
        NetworkEndPoint endpoint = NetworkEndPoint.Parse(ip, (ushort)port);
        connection = driver.Connect(endpoint);
    }

    private void FixedUpdate()
    {
        driver.ScheduleUpdate().Complete();
        CheckAlive();
        UpdateMessagePump();
    }

    private void CheckAlive()
    {
        if (!connection.IsCreated)
        {
            Debug.LogError("Something wrent wrong with conectin");
        }
    }

    private void UpdateMessagePump()
    {
        DataStreamReader stream;
        NetworkEvent.Type cmd;

        while ((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We have connected with the server");
                if (connection.GetState(driver) != NetworkConnection.State.Connected)
                {
                    Debug.Log("Connection error");
                    ServerInfoTransfer menuHandler = FindObjectOfType<ServerInfoTransfer>();
                    Destroy(menuHandler.gameObject);
                    SceneManager.LoadScene("menu");
                } else
                {
                    Debug.Log("Succesfull connection");
                }
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                OnData(stream);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client diesconnected from server");
                connection = default(NetworkConnection);
            }
        }
    }

    private void OnData(DataStreamReader stream)
    {
        NetMessage msg = null; //Unknown type of message
        //Get operation code from first byte of message
        var opCode = (OpCode)stream.ReadByte();
        switch (opCode)
        {
            case OpCode.PLAYER_POSITION: msg = new NetPlayerPos(stream); break;
            case OpCode.NEW_PLAYER: msg = new NetNewPlayer(stream); break;
            case OpCode.BALL_MESSAGE: msg = new NetBallMessage(stream); break;
            case OpCode.UPDATE_SCORE: msg = new NetUpdateScore(stream); break;
            default:
                Debug.Log("Message recieved had no OpCode " + opCode);
                break;
        }

        //Should already be parsed with the constructor
        msg.ReceivedOnClient();
    }

    public void SendtoServer(NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        //BeginSend returns a driver ready to write for the connnection (maybe)
        msg.Serialize(ref writer);
        //Message has been written
        driver.EndSend(writer);
    }
}
