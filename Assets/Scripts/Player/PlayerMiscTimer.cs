using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMiscTimer : MonoBehaviour
{
    PlayerActionController controller;
    public float sinceGround;
    void Start()
    {
        controller = this.gameObject.GetComponent<PlayerActionController>();
        sinceGround = 0;
    }

    private void FixedUpdate()
    {
        if (!controller.isGround)
        {
            sinceGround += Time.deltaTime;
        } else
        {
            sinceGround = 0;
        }
    }
}
