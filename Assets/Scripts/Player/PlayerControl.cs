using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    List<PlayerInputFlag> inputFlags;
    List<PlayerInputFlag> previousInputFlags;
    Rigidbody2D rb;
    void Start()
    {
        inputFlags= new List<PlayerInputFlag>();
        previousInputFlags = inputFlags;
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        ApplyInputFlags();
    }

    private void ApplyInputFlags()
    {
        //Get inputs from manager
        inputFlags = PlayerInputManager.getInputList();
        foreach (PlayerInputFlag flag in inputFlags)
        {
            switch (flag)
            {
                case PlayerInputFlag.up:
                    MoveUp();
                    break;
                case PlayerInputFlag.down:
                    MoveDown();
                    break;
                case PlayerInputFlag.left:
                    MoveLeft();
                    break;
                case PlayerInputFlag.right:
                    MoveRight();
                    break;
            }
        }
        previousInputFlags = inputFlags;
    }

    private void MoveUp()
    {
        rb.velocity = new Vector2(0, 1);
    }

    private void MoveDown()
    {
        rb.velocity = new Vector2(0, -1);
    }

    private void MoveLeft()
    {
        rb.velocity = new Vector2(-1, 0);
    }

    private void MoveRight()
    {
        rb.velocity = new Vector2(1, 0);
    }

}
