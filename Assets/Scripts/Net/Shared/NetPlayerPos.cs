using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.VisualScripting;
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

    public float VelX { set; get; }
    public float VelY { set; get; }
    public float VelZ { get; set; }

    public NetPlayerPos()
    {
        Code = OpCode.PLAYER_POSITION;
    }
    public NetPlayerPos(DataStreamReader reader)
    {
        Code = OpCode.PLAYER_POSITION;
        Deserialize(reader);
    }
    public NetPlayerPos(int playerId, float posX, float posY, float posZ, Vector3 vel)
    {
        Code = OpCode.PLAYER_POSITION;
        PlayerId = playerId;
        PosX = posX;
        PosY = posY;
        PosZ = posZ;

        VelX = vel.x;
        VelY = vel.y;
        VelZ = vel.z;
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(PlayerId);

        writer.WriteFloat(PosX);
        writer.WriteFloat(PosY);
        writer.WriteFloat(PosZ);

        writer.WriteFloat(VelX);
        writer.WriteFloat(VelY);
        writer.WriteFloat(VelZ);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        //First byte already read by Server func to determine the type, only message remains
        PlayerId= reader.ReadInt();
        PosX= reader.ReadFloat();
        PosY= reader.ReadFloat();
        PosZ= reader.ReadFloat();

        VelX= reader.ReadFloat();
        VelY= reader.ReadFloat();
        VelZ= reader.ReadFloat();
    }
    public override void ReceivedOnServer(ServerManager server)
    {
        //Debug.Log("SERVER::" + PlayerId + "::" + FormatPos());
        server.BroadCast(this);
    }
    public override void ReceivedOnClient()
    {
        //Debug.Log("CLIENT::" + PlayerId + "::" + FormatPos());
        PlayerListManager plm = PlayerListManager.Instance;
        GameObject playerToUpdate = plm.getPlayerById((byte)PlayerId);

        //check if is controllable prefab, then don't update
        if (playerToUpdate.name.Contains("clientControlPlayer"))
        {
            //Do nothing
        } else
        {
            playerToUpdate.transform.position = new Vector3(PosX, PosY, PosZ);
            playerToUpdate.GetComponent<Rigidbody2D>().velocity = new Vector3(VelX, VelY, VelZ);
        }
        
    }
    private string FormatPos()
    {
        return "(" + PosX + " , " + PosY + " , " + PosZ + ")";
    }
}
