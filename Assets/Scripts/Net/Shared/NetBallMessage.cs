using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetBallMessage : NetMessage
{
   
    private float posX, posY, posZ;
    private float velX, velY, velZ;
    private float dirX, dirY, dirZ;
    private byte lastBallTouchId;
    public NetBallMessage(DataStreamReader reader)
    {
        Code = OpCode.BALL_MESSAGE;
        Deserialize(reader);
    }

    public NetBallMessage(Vector3 pos,Vector3 vel, Vector3 dir, byte playerId)
    {
        Code = OpCode.BALL_MESSAGE;
        this.posX = pos.x;
        this.posY = pos.y;
        this.posZ = pos.z;

        this.velX = vel.x;
        this.velY = vel.y;
        this.velZ = vel.z;

        this.dirX = dir.x;
        this.dirY = dir.y;
        this.dirZ = dir.z;

        this.lastBallTouchId = playerId;
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        Code = OpCode.BALL_MESSAGE;
        writer.WriteByte((byte)Code); // OpCode

        writer.WriteFloat(posX);
        writer.WriteFloat(posY);
        writer.WriteFloat(posZ);

        writer.WriteFloat(velX);
        writer.WriteFloat(velY);
        writer.WriteFloat(velZ);

        writer.WriteFloat(dirX);
        writer.WriteFloat(dirY);
        writer.WriteFloat(dirZ);

        writer.WriteByte(lastBallTouchId);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        //Code has already been read by the pump
        posX = reader.ReadFloat();
        posY = reader.ReadFloat();
        posZ = reader.ReadFloat();

        velX = reader.ReadFloat();
        velY = reader.ReadFloat();
        velZ = reader.ReadFloat();

        dirX = reader.ReadFloat();
        dirY = reader.ReadFloat();
        dirZ = reader.ReadFloat();

        lastBallTouchId = reader.ReadByte();
    }
    public override void ReceivedOnServer(ServerManager server)
    {
        BallManager.instance.UpdateBallPosition(new Vector3(posX, posY, posZ)
            , new Vector3(velX, velY, velZ), new Vector3(dirX, dirY, dirZ));
        BallManager.instance.SetTouchId(lastBallTouchId);
        ServerManager.instance.BroadCast(this);
    }
    public override void ReceivedOnClient()
    {
        //Check if this is a pure client (no host) then update the ball
        if (!ServerManager.instance.gameObject.activeSelf)
        {
            BallManager.instance.UpdateBallPosition(new Vector3(posX, posY, posZ)
                , new Vector3(velX, velY, velZ), new Vector3(dirX, dirY, dirZ));
            BallManager.instance.SetTouchId(lastBallTouchId);
        }
    }
}
