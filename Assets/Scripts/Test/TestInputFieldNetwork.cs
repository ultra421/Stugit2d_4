using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestInputFieldNetwork : MonoBehaviour
{
    [SerializeField] private TMP_InputField chatInput;
    
    public void OnSubmitClick()
    {
        NetChatMessage msg = new NetChatMessage(chatInput.text);
        FindObjectOfType<BaseClient>().SendToServer(msg);
    }
}
