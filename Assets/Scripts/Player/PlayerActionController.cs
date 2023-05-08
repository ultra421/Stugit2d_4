using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    private PlayerInputManager pim;
    [Header("Positional")]
    public Vector2 velocity, position;
    [Header("Attribtues")]
    public float MaxSpeedX;
    public float MaxSpeedY;
    public float AccelX;
    public float jumpVel;
    public float jumps;
    public float gravity;
    public float gravityMultiplier;
    public bool isGround;

    private Rigidbody2D rb;
    private Dictionary<string, float> multipliers;
    private PlayerMiscTimer timer;

    private void Start()
    {
        rb = this.transform.GetComponent<Rigidbody2D>();
        position = rb.position;
        velocity = rb.velocity;

        MaxSpeedX = 5f;
        MaxSpeedY = 15f;
        AccelX = 25f;
        jumpVel = 15f;
        jumps = 1;
        gravity = 40f;
        isGround = false;

        pim = PlayerInputManager.instance;
        multipliers = new Dictionary<string, float>
        {
            { "gravity", 1 }
        };

        timer = GetComponent<PlayerMiscTimer>();
    }

    private void FixedUpdate()
    {
        PreProcessChecks(); //Restores and prepares variables for processing
        SetMultipliers();   //Process the multipliers
        ProcessActions();   //Processes actions read from PlayerInputManager
        Gravity();          //Gravity
        ApplyActions();     //Applies the actions to the rigidbody
    }
    private void PreProcessChecks()
    {
        position = rb.position;
        velocity = rb.velocity;
    }
    private void SetMultipliers()
    {
        if (pim.GetAction(PlayerAction.jump))
        {
            multipliers["gravity"] = 0.65f;
        } else
        {
            multipliers["gravity"] = 2;
        }
    }
    private void ProcessActions()
    {
        List<PlayerAction> thisFrameActions = pim.GetFrameActions();
        foreach (PlayerAction action in thisFrameActions)
        {
            //Debug.Log("Frame action:" + action.ToString() + " pressed for " + pim.ActionTime(action));
            switch (action)
            {
                case PlayerAction.left:
                    MoveLeft();
                    break;
                case PlayerAction.right:
                    MoveRight();
                    break;
                case PlayerAction.jump:
                    Jump();
                    break;
            }
        }
    }
    private void MoveLeft()
    {
        velocity.x -= AccelX * Time.deltaTime;
    }
    private void MoveRight()
    {
        velocity.x += AccelX * Time.deltaTime;
    }
    private void Jump()
    {
        //Return if pressed for longer than 0.4f seconds
        if (pim.ActionTime(PlayerAction.jump) > 0.4f) { return; }

        //Set gravity multiplier for falling slower/faster
        if (isGround || (timer.sinceGround < 0.08f && jumps != 0))
        {
            velocity.y = jumpVel;
            jumps--;
        }
    }
    private void Gravity()
    {
        if (!isGround)
        {
            velocity.y -= gravity * multipliers["gravity"] * Time.deltaTime;
        }
    }
    private void ApplyActions()
    {
        rb.velocity = velocity;
    }
    
    //Functions for external use
    public void SetGround(bool status)
    {
        isGround = status;
        if (isGround)
        {
            jumps = 1;
        }
    }


}
