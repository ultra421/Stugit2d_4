using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerListManager : MonoBehaviour
{
    public static PlayerListManager Instance;
    [SerializeField] GameObject controlPlayerPrefab;
    [SerializeField] GameObject serverControlPlayerPrefab;
    private Dictionary<byte, GameObject> playerList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            playerList= new Dictionary<byte, GameObject>();
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
    public void CreatePlayer(byte playerId)
    {
        playerList.Add(playerId, Instantiate(serverControlPlayerPrefab));
    }
    public void CreatePlayer()
    {
        playerList.Add(Convert.ToByte(playerList.Count), Instantiate(serverControlPlayerPrefab));
    }

    public GameObject getPlayerById(byte playerId)
    {
        return playerList[playerId];
    }

    public Dictionary<byte,GameObject> getPlayerList() { return playerList; }
}
