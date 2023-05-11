using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetChatMessage : NetMessage
{
    // 0 - 8 OP CODE
    // 8 - 128 STRING MESSAGE
    // OpCode Code (from parent)
    public FixedString128Bytes ChatMessage { set; get; }
    
    public NetChatMessage()
    {

    }
    public NetChatMessage(DataStreamReader reader)
    {
        Code = OpCode.CHAT_MESSAGE;
        Deserialize(reader);
    }
    public NetChatMessage(string msg) 
    {
        Code = OpCode.CHAT_MESSAGE;
        ChatMessage= msg;
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteFixedString128(ChatMessage);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        //First byte already read by Server func to determine the type, only message remains
        ChatMessage = reader.ReadFixedString128();

    }
    public override void ReceivedOnServer(ServerManager server)
    {
        Debug.Log("SERVER::" + ChatMessage);
    }
    public override void ReceivedOnClient()
    {
        Debug.Log("CLIENT::" + ChatMessage);
    }
}
