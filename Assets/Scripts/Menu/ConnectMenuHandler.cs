using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConnectMenuHandler : MonoBehaviour
{
    [SerializeField] TMP_InputField ip;
    [SerializeField] TMP_InputField username;

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
    }

    private void Host()
    {
        Debug.Log("Host with ip" + ip.text + " username: " + username.text);
    } 
}
