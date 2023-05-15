using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    private void FixedUpdate()
    {
        string newText = "";
        PlayerListManager plm = PlayerListManager.Instance;
        Dictionary<byte,byte> playerScore = plm.getPlayerScoreMap();
        foreach(KeyValuePair<byte,byte> pair in playerScore)
        {
            newText += "Player id : " + pair.Key + " score = " + pair.Value + "\n";
        }
        text.text= newText;
    }
}
