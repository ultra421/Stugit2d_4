using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetNewPlayer : NetMessage 
{
    public byte playerId;

    public NetNewPlayer(byte playerId)
    {
        Code = OpCode.NEW_PLAYER;
        this.playerId = playerId;
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteByte(playerId);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        //First byte already read by Server func to determine the type, only message remains
        playerId = reader.ReadByte();
    }
    public override void ReceivedOnServer(ServerManager server)
    {
        PlayerListManager.Instance.CreatePlayer(playerId);
    }
    public override void ReceivedOnClient()
    {
        PlayerListManager.Instance.CreatePlayer(playerId);
    }
}
