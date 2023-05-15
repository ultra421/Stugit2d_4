using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetUpdateScore : NetMessage
{
    private byte playerId;
    private byte score;

    public NetUpdateScore(DataStreamReader reader)
    {
        Code = OpCode.UPDATE_SCORE;
        Deserialize(reader);
    }
    public NetUpdateScore(byte playerId, byte score)
    {
        Code = OpCode.UPDATE_SCORE;
        this.playerId = playerId;
        this.score = score;
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteByte(playerId);
        writer.WriteByte(score);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        //Code already read by pump
        playerId = reader.ReadByte();
        score = reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
        PlayerListManager plm = PlayerListManager.Instance;
        plm.UpdatePlayerScore(playerId, score);
        Debug.Log("Recieved update Score on client " + playerId + " =score= " + score);
    }
    public override void ReceivedOnServer(ServerManager server)
    {
        PlayerListManager plm = PlayerListManager.Instance;
        plm.UpdatePlayerScore(playerId, score);
        server.BroadCast(this);
        BallManager.instance.ResetBall();
    }

}
