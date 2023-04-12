using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetPlayerPos : NetMessage
{
    // 0 - 8 OP CODE
    // 8 - 128 
    // OpCode Code (from parent)
    public int PlayerId { set; get; }  
    public float PosX { set; get; }
    public float PosY { set; get; }
    public float PosZ { get; set; }
    public NetPlayerPos()
    {
        Code = OpCode.PLAYER_POSITION;
    }
    public NetPlayerPos(DataStreamReader reader)
    {
        Code = OpCode.PLAYER_POSITION;
        Deserialize(reader);
    }
    public NetPlayerPos(int playerId, float posX, float posY, float posZ)
    {
        Code = OpCode.PLAYER_POSITION;
        PlayerId = playerId;
        PosX = posX;
        PosY = posY;
        PosZ = posZ;
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(PlayerId);
        writer.WriteFloat(PosX);
        writer.WriteFloat(PosY);
        writer.WriteFloat(PosZ);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        //First byte already read by Server func to determine the type, only message remains
        PlayerId= reader.ReadInt();
        PosX= reader.ReadFloat();
        PosY= reader.ReadFloat();
        PosZ= reader.ReadFloat();
    }
    public override void ReceivedOnServer(BaseServer server)
    {
        Debug.Log("SERVER::" + PlayerId + "::" + FormatPos());
        server.BroadCast(this);
    }
    public override void ReceivedOnClient()
    {
        Debug.Log("CLIENT::" + PlayerId + "::" + FormatPos());
    }
    private string FormatPos()
    {
        return "(" + PosX + " , " + PosY + " , " + PosZ + ")";
    }
}
