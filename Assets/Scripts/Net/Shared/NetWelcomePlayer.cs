using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetWelcomePlayer : NetMessage
{

    List<WelcomePlayerPayload> playerInfo;
    bool isFirstPlayer;
    public NetWelcomePlayer()
    {
        Code = OpCode.INITIAL_PLAYER_LIST;
    }
    public NetWelcomePlayer(List<WelcomePlayerPayload> playerInfo)
    {
        Code = OpCode.INITIAL_PLAYER_LIST;
        this.playerInfo = playerInfo;
    }

    public NetWelcomePlayer(DataStreamReader reader)
    {
        Code = OpCode.INITIAL_PLAYER_LIST;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer) 
    {
        //Write the OpCode
        writer.WriteByte((byte)Code);
        //Write lenght of list
        writer.WriteByte((byte)playerInfo.Count);
        foreach (WelcomePlayerPayload payload in playerInfo)
        {
            // Write out the Vector3 position as three floats
            writer.WriteFloat(payload.pos.x);
            writer.WriteFloat(payload.pos.y);
            writer.WriteFloat(payload.pos.z);

            // Write out the ID as a byte
            writer.WriteByte(payload.id);

            // Write out the name string
            writer.WriteFixedString128(payload.name);
        }

        //Send if is first player
        isFirstPlayer = playerInfo.Count == 0;
        writer.WriteRawBits(1, 1);
    }
    public override void Deserialize(DataStreamReader reader) 
    {
        playerInfo = new List<WelcomePlayerPayload>();
        //Get lenght of loop
        byte loopLength = reader.ReadByte();
        for (int i = 0; i < loopLength; i++)
        {
            //Read out the player Positon
            Vector3 playerPos = new Vector3(
                reader.ReadFloat(),
                reader.ReadFloat(),
                reader.ReadFloat()
            );

            //Read their ID
            byte playerId = reader.ReadByte();

            //Read the name
            FixedString128Bytes name = reader.ReadFixedString128();
            WelcomePlayerPayload newPayload = new WelcomePlayerPayload(playerPos, playerId);
            playerInfo.Add(newPayload);
        }

        //Check if is firstPlayer
        isFirstPlayer = reader.ReadRawBits(1) == 1;

    }
    public override void ReceivedOnClient() 
    {
        if (isFirstPlayer)
        {
            PlayerListManager.Instance.CreateControllablePlayer(0);
            ClientManager.Instance.playerId = 0;
        } else
        {
            foreach(WelcomePlayerPayload info in playerInfo)
            {
                PlayerListManager plm = PlayerListManager.Instance;
                plm.CreatePlayer(info.id);
            }
        }
    }
    public override void ReceivedOnServer(ServerManager server) 
    {
        
    }

    private byte getBiggestId()
    {
        byte biggest = 0;
        foreach(WelcomePlayerPayload info in playerInfo)
        {
            if (info.id > biggest) { biggest = info.id; }
        }
        return biggest;
    }
}

public class WelcomePlayerPayload
{
    public Vector3 pos;
    public byte id;
    public string name;

    public WelcomePlayerPayload(Vector3 pos, byte id)
    {
        this.pos = pos;
        this.id = id;
    }
}