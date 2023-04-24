using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    List<PlayerInputFlag> inputFlags;
    List<PlayerInputFlag> previousInputFlags;
    List<PlayerInputFlag> inputFlagsDown;
    List<PlayerInputFlag> previousInputFlagsDown;
    Rigidbody2D rb;
    [SerializeField] private PlayerProperties properties;
    private float timeSinceJumpInput;

    void Start()
    {
        inputFlags= new List<PlayerInputFlag>();
        previousInputFlags = inputFlags;
        inputFlagsDown = new List<PlayerInputFlag>();
        previousInputFlagsDown = inputFlagsDown;
        rb = GetComponent<Rigidbody2D>();
        timeSinceJumpInput = 0;
    }

    public void ApplyPlayerProperties(PlayerProperties playerProperties)
    {

    }

    private void FixedUpdate()
    {
        InitialChecks(); //Run at the start of every frame
        ApplyInputFlags(); //Apply user inputs
        DeAccelerationChecks(); //Apply modifiers
        TickTimeSince(); //Tick timers
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
        inputFlagsDown = PlayerInputManager.getInputListDown();
        //Continous inputs
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
            }
        }
        //getDownKey inputs
        foreach(PlayerInputFlag flag in inputFlagsDown)
        {
            Vector2 newSpeed = properties.Speed;
            switch (flag)
            {
                case PlayerInputFlag.jump:
                    Debug.Log("Jummp!");
                    timeSinceJumpInput = 0;
                    Jump(newSpeed);
                    break;
            }
        }
        //coyote inputs
        if (timeSinceJumpInput < 0.3f) { Jump(properties.Speed); }

        //Store previusInputFlags for additional checking
        previousInputFlags = inputFlags;
        previousInputFlagsDown = inputFlagsDown;
    }
    private void MoveLeft(Vector2 newSpeed)
    {
        //Check if going the ohter way
        if (properties.Speed.x > 0)
        {
            newSpeed.x += -properties.AccelX * Time.deltaTime * 2;
        } else
        {
            newSpeed.x += -properties.AccelX * Time.deltaTime;
        }  
        properties.Speed = newSpeed;
    }
    private void MoveRight(Vector2 newSpeed)
    {
        if (properties.Speed.x < 0)
        {
            newSpeed.x += properties.AccelX * Time.deltaTime * 2;
        }
        else
        {
            newSpeed.x += properties.AccelX * Time.deltaTime;
        }
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
        Gravity();
        SidewaysDrag();
        MaxSpeed();
    }
    private void Gravity()
    {
        if (!properties.IsGround)
        {
            Vector2 newSpeed = properties.Speed;
            newSpeed.y += -properties.Gravity * Time.deltaTime * GetVerticalMultiplier();
            properties.Speed = newSpeed;
        }
    }
    private void SidewaysDrag()
    {
        if (!LeftOrRightPressed())
        {
            Vector2 newSpeed = properties.Speed;
            float multiplier = GetDragMultiplier();

            if (newSpeed.x < 0.6 && newSpeed.x > -0.6)
            {
                newSpeed = new Vector2(0, newSpeed.y);
            }
            else if (properties.Speed.x > 0) //right
            {
                newSpeed.x -= properties.DragAccelX * Time.deltaTime * multiplier;
            }
            else if (properties.Speed.x < 0) //left
            {
                newSpeed.x += properties.DragAccelX * Time.deltaTime * multiplier;
            }
            properties.Speed = newSpeed;
        }
    }
    private void MaxSpeed()
    {
        Vector2 newSpeed = properties.Speed;
        if (newSpeed.x > properties.MaxSpeedX)
        {
            newSpeed.x = properties.MaxSpeedX;
        } else if (newSpeed.x < -properties.MaxSpeedX)
        {
            newSpeed.x = -properties.MaxSpeedX;
        }
        properties.Speed = newSpeed;
    }
    private bool LeftOrRightPressed()
    {
        return (inputFlags.Contains(PlayerInputFlag.left) || inputFlags.Contains(PlayerInputFlag.right)) ;
    }
    private float GetDragMultiplier()
    {
        float multiplier;
        if (properties.IsGround)
        {
            multiplier = 1.0f;
        } else if (!properties.IsGround)
        {
            multiplier = 0.7f;
        } else
        {
            multiplier = 1f;
        }
        return multiplier;
    }
    private float GetVerticalMultiplier()
    {
        //Affects gravity
        float multiplier;
        if (properties.Speed.y < 0) //Going down
        {
            if (inputFlags.Contains(PlayerInputFlag.down))
            {
                multiplier = 4f;
            }
            else
            {
                multiplier = 1f;
            }
        } else //Going up
        {
            if (inputFlags.Contains(PlayerInputFlag.jump))
            {
                multiplier = 0.8f;
            }
            else if (inputFlags.Contains(PlayerInputFlag.down))
            {
                multiplier = 3f;
            }
            else
            {
                multiplier = 1.4f;
            }
        }
        return multiplier;
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

    private void TickTimeSince()
    {
        timeSinceJumpInput += Time.deltaTime;
    }

}
