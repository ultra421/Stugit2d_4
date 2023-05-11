using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectMenuHandler : MonoBehaviour
{
    [SerializeField] TMP_InputField ip;
    [SerializeField] TMP_InputField username;
    [SerializeField] GameObject transfer; //Transfer the info between menu and game

    public void Submit(string type)
    {
        if (type == "connect")
        {
            Connect();
        } else if (type == "host")
        {
            Host();
        }
    }

    private void Connect()
    {
        Debug.Log("Connecting with ip" + ip.text + " username: " + username.text);
        ServerInfoTransfer transferScript = transfer.GetComponent<ServerInfoTransfer>();
        transferScript.SetConnectionInfo(
            ip.text,
            5522,
            ConnectionType.CLIENT,
            username.text
        );

        SceneManager.LoadScene("gameScene");
    }

    private void Host()
    {
        Debug.Log("Host with ip" + ip.text + " username: " + username.text);
        ServerInfoTransfer transferScript = transfer.GetComponent<ServerInfoTransfer>();
        transferScript.SetConnectionInfo(
            ip.text,
            5522,
            ConnectionType.HOST,
            username.text
        );

        SceneManager.LoadScene("gameScene");
    } 
}
