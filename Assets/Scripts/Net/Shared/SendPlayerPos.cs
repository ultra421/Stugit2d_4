using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendPlayerPos : MonoBehaviour
{
    private float lastSendTime;
    private BaseClient client;

    private void Start()
    {
        client = FindObjectOfType<BaseClient>();
    }
    private void Update()
    {
        //if (Time.time - lastSendTime > 1.0f)
        //{
        //    Vector3 clientPos = transform.position;
        //    NetPlayerPos pos = new NetPlayerPos(1, clientPos.x, clientPos.y, clientPos.z);
        //    client.SendToServer(pos);
        //    lastSendTime= Time.time;
        //}
    }
}
