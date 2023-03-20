using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CanvasNetworkManager : MonoBehaviour
{

    private static CanvasNetworkManager Instance { get; set; }
    // Start is called before the first frame update

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ClickGet()
    {
        string url = "http://localhost/Unity/2d_4/managerGet.php";
        StartCoroutine(SendGet(url));
    }

    IEnumerator SendGet(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Connection error" + request.error);
        } else
        {
            string response = request.downloadHandler.text;
            Debug.Log("Response from server " + response);
        }
    }

    public void ClickPost()
    {

    }

}
