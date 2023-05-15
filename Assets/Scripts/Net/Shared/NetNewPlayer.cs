using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetNewPlayer : NetMessage 
{
    public byte playerId;
    public bool isControllable;
    public float posX , posY, posZ;

    public NetNewPlayer(byte playerId, bool isControllable,Vector3 pos)
    {
        Code = OpCode.NEW_PLAYER;
        this.playerId = playerId;
        this.isControllable = isControllable;
        posX = pos.x;
        posY = pos.y;
        posZ = pos.z;
    }

    public NetNewPlayer(DataStreamReader reader)
    {
        Code = OpCode.NEW_PLAYER;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code); // OpCode
        writer.WriteByte(playerId); //playerId
        writer.WriteRawBits((uint)(isControllable? 1 : 0),1); //Is controllable
        //Position
        writer.WriteFloat(posX); 
        writer.WriteFloat(posY); 
        writer.WriteFloat(posZ);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        //First byte already read by Server func to determine the type, only message remains
        playerId = reader.ReadByte();
        isControllable = reader.ReadRawBits(1) == 1;
        //Position
        posX = reader.ReadFloat();
        posY = reader.ReadFloat();
        posZ = reader.ReadFloat();
    }
    public override void ReceivedOnServer(ServerManager server)
    {
        
    }
    public override void ReceivedOnClient()
    {
        Debug.Log("Recieved NEW_PLAYER on Client for PlayerID:" + playerId + " isControllable?=" + isControllable);
        PlayerListManager pim = PlayerListManager.Instance;
        Debug.Log("Is pim instanced?");
        Debug.Log("Are positions instanced?" + posX + " " + posY + " " + posZ);
        Debug.Log("pim instanced?=" + pim != null);
        if (isControllable)
        {
            pim.CreateControllablePlayer(playerId, new Vector3(posX, posY, posZ));
            ClientManager.Instance.playerId = playerId;
        } else
        {
            pim.CreatePlayer(playerId,new Vector3(posX,posY, posZ));
        }
    }
}
