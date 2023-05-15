using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class PlayerListManager : MonoBehaviour
{
    public static PlayerListManager Instance;
    [SerializeField] GameObject controlPlayerPrefab;
    [SerializeField] GameObject serverControlPlayerPrefab;
    private Dictionary<byte, GameObject> playerList;
    private Dictionary<byte, byte> playerScore;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            playerList= new Dictionary<byte, GameObject>();
            playerScore = new Dictionary<byte, byte>();
        } else
        {
            Destroy(this.gameObject);
        }

    }

    private void Start()
    {

    }
    public void CreateControllablePlayer(byte playerID)
    {
        playerList.Add(playerID, Instantiate(controlPlayerPrefab));
    }
    public void CreateControllablePlayer(byte playerID,Vector3 pos)
    {
        GameObject newPlayer = Instantiate(controlPlayerPrefab,pos,Quaternion.identity);
        playerList.Add(playerID, newPlayer);
        playerScore.Add(playerID, 0);
    }
    public void CreatePlayer(byte playerId,Vector3 pos)
    {
        GameObject newPlayer = Instantiate(serverControlPlayerPrefab, pos, Quaternion.identity);
        playerList.Add(playerId, newPlayer);
        playerScore.Add(playerId, 0);
    }
    public void CreatePlayer()
    {
        playerList.Add(Convert.ToByte(playerList.Count), Instantiate(serverControlPlayerPrefab));
    }

    public GameObject getPlayerById(byte playerId)
    {
        GameObject player;
        playerList.TryGetValue(playerId, out player);
        return player;
    }

    public Dictionary<byte,GameObject> getPlayerList() { return playerList; }

    public byte getPlayerId(GameObject player)
    {
        foreach(KeyValuePair<byte,GameObject> keyValuePair in playerList)
        {
            if (keyValuePair.Value == player) 
            {
                return keyValuePair.Key;
            }
        }
        throw new Exception("Player not found");
    }

    public void UpdatePlayerScore(byte playerId, byte score)
    {
        playerScore[playerId] = score;
    }

    public byte getPlayerScore(byte playerId)
    {
        return playerScore[playerId];
    }

    public Dictionary<byte,byte> getPlayerScoreMap()
    {
        return playerScore;
    }
}
