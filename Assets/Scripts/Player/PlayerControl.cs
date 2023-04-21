using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    List<PlayerInputFlag> inputFlags;
    List<PlayerInputFlag> previousInputFlags;
    Rigidbody2D rb;
    PlayerProperties properties;

    byte PlayerId { get; set; }
    void Start()
    {
        inputFlags= new List<PlayerInputFlag>();
        previousInputFlags = inputFlags;
        rb = GetComponent<Rigidbody2D>();
        properties = new PlayerProperties(rb);
        PlayerId = 0;
    }

    public void ApplyPlayerProperties(PlayerProperties playerProperties)
    {

    }

    private void FixedUpdate()
    {
        InitialChecks(); //Run at the start of every frame
        ApplyInputFlags(); //Apply user inputs
        DeAccelerationChecks(); //Apply modifiers
        ApplyValuesToRigidbody();
    }


    private void InitialChecks()
    {
        properties.Speed = rb.velocity;
        properties.Pos = rb.position;
    }

    private void ApplyInputFlags()
    {
        //Get inputs from manager
        inputFlags = PlayerInputManager.getInputList();
        foreach (PlayerInputFlag flag in inputFlags)
        {
            Vector2 newSpeed = properties.Speed; //Speed to be edited by each method
            switch (flag)
            {
                case PlayerInputFlag.left:
                    MoveLeft(newSpeed);
                    break;
                case PlayerInputFlag.right:
                    MoveRight(newSpeed);
                    break;
                case PlayerInputFlag.jump:
                    Jump(newSpeed);
                    break;
                
            }
        }
        //Store previusInputFlags for additional checking
        previousInputFlags = inputFlags;
    }
    private void MoveLeft(Vector2 newSpeed)
    {
        newSpeed.x += -properties.AccelX * Time.deltaTime;
        properties.Speed = newSpeed;
    }

    private void MoveRight(Vector2 newSpeed)
    {
        newSpeed.x += properties.AccelX * Time.deltaTime;
        properties.Speed = newSpeed;
    }

    private void Jump(Vector2 newSpeed)
    {
        if (properties.IsGround)
        {
            newSpeed.y = properties.InitialJumpSpeed;
            properties.Speed = newSpeed;
            properties.IsGround = false;
        }
    }

    void DeAccelerationChecks()
    {
        //Gravity
        if (!properties.IsGround)
        {
            Vector2 newSpeed = properties.Speed;
            newSpeed.y += -properties.Gravity * Time.deltaTime;
            properties.Speed = newSpeed;
        }

        //Drag
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.collider.gameObject.CompareTag("SolidGround"))
        {
            Debug.Log("Ground collided");
            properties.IsGround = true;
        } else
        {
           
        }
    }

    private void ApplyValuesToRigidbody()
    {
        rb.velocity = properties.Speed;
    }

}
