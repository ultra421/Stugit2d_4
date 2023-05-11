using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ConnectionType
{
    HOST,
    CLIENT
}
public class ServerInfoTransfer : MonoBehaviour
{
    // Start is called before the first frame update
    public ConnectionType connectionType;
    public string ip;
    public int port;
    public string username;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetConnectionInfo(string ip, int port, ConnectionType type, string username)
    {
        this.ip = ip;
        this.port = port;
        this.connectionType = type;
        this.username = username;
    }

}
