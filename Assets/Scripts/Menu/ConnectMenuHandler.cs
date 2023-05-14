using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ConnectMenuHandler : MonoBehaviour
{
    [SerializeField] TMP_InputField ip;
    [SerializeField] TMP_InputField username;
    [SerializeField] GameObject transfer; //Transfer the info between menu and game
    [SerializeField] TMP_InputField password;
    [SerializeField] TextMeshProUGUI statusText;

    public void Submit(string type)
    {
        if (type == "connect")
        {
            StartCoroutine(Login(result =>
            {
                if (result) 
                {
                    Connect();
                } else
                {
                    statusText.text = "Wrong username/password";
                }
            }));
            
        } else if (type == "host")
        {
            StartCoroutine(Login(result =>
            {
                if (result)
                {
                    Host();
                }
                else
                {
                    statusText.text = "Wrong username/password";
                }
            }));
        } else if (type == "register")
        {
            StartCoroutine(Register());
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

    private IEnumerator Login(Action<bool> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username.text);
        form.AddField("password", password.text);
        form.AddField("login", "true");

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/Unity/2d_4/requestHandler.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            callback(false);
        }
        else
        {
            string responseText = www.downloadHandler.text;
            Debug.Log(responseText);
            callback(bool.Parse(responseText));
        }
    }

    private IEnumerator Register()
    {
        Debug.Log("Registering...");
        string url = "http://localhost/Unity/2d_4/requestHandler.php";
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("register", "true"));
        formData.Add(new MultipartFormDataSection("username", username.text));
        formData.Add(new MultipartFormDataSection("password", password.text));
        UnityWebRequest www = UnityWebRequest.Post(url, formData);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            // Registration was successful
            Debug.Log("User registered!");
        }
        else
        {
            // Registration failed
            Debug.Log("Registration failed: " + www.error);
        }
    }
}
