using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    private PlayerInputManager pim;
    [Header("Positional")]
    public Vector2 velocity, position;
    [Header("Attribtues")]
    public byte playerId;
    public float MaxSpeedX;
    public float MaxSpeedY;
    public float AccelX;
    public float dragAccelX;
    public float jumpVel;
    public float jumps;
    public float gravity;
    public float gravityMultiplier;
    public bool isGround;

    private Rigidbody2D rb;
    private Dictionary<string, float> multipliers;
    private PlayerMiscTimer timer;
    private Vector2 lastVelocity;

    private void Start()
    {
        rb = this.transform.GetComponent<Rigidbody2D>();
        position = rb.position;
        velocity = rb.velocity;
        playerId = 0;

        MaxSpeedX = 11f;
        MaxSpeedY = 13f;
        AccelX = 60f;
        dragAccelX = 40f;
        jumpVel = 13f;
        jumps = 1;
        gravity = 40f;
        isGround = false;

        pim = PlayerInputManager.instance;
        multipliers = new Dictionary<string, float>
        {
            { "gravity", 1 },
            { "accelX",1 }
        };

        timer = GetComponent<PlayerMiscTimer>();
        lastVelocity = velocity;
    }

    private void FixedUpdate()
    {
        PreProcessChecks();     //Restores and prepares variables for processing
        SetMultipliers();       //Process the multipliers
        ProcessActions();       //Processes actions read from PlayerInputManager
        PostProcessActions();   //Check stuff after processing actions, such as drag
        Gravity();              //Gravity
        ApplyActions();         //Applies the actions to the rigidbody
    }

    private void LateUpdate()
    {
        //Network stuff takes place here
        Vector3 currentPos = this.transform.position;
        ClientManager clientManager = ClientManager.Instance;
        NetPlayerPos message = new NetPlayerPos(clientManager.playerId,
        currentPos.x,currentPos.y,currentPos.z);

        clientManager.SendtoServer(message);
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
            multipliers["gravity"] = 3;
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
        //If going right, when moving left set multiplier to 2
        multipliers["accelX"] = velocity.x > 0 ? 2 : 1;
        velocity.x -= AccelX * Time.deltaTime * multipliers["accelX"];
    }
    private void MoveRight()
    {
        multipliers["accelX"] = velocity.x < 0 ? 2 : 1;
        velocity.x += AccelX * Time.deltaTime * multipliers["accelX"]; 
    }
    private void Jump()
    {
        //Return if pressed for longer than 0.4f seconds
        if (pim.ActionTime(PlayerAction.jump) > 0.4f) { return; }

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
    private void PostProcessActions()
    {
        Drag();
        CheckMaxSpeed();
    }
    private void Drag()
    {
        //Debug.Log("velocity.x = " + velocity + " lastVel = " + lastVelocity);
        if (velocity.x == 0)
        {
            return;
        }

        //Check if sign change
        if (!IsHorizontalPress() && velocity.x != 0)
        { //If not pressing keys
            if (velocity.x > 0)
            { //Right
                velocity.x -= dragAccelX * Time.deltaTime;
            }
            else
            { //Left
                velocity.x += dragAccelX * Time.deltaTime;
            }
        }

        if (!(velocity.x * lastVelocity.x > 0) && !IsHorizontalPress() 
            && velocity.x < 1 && velocity.x > -1)
        { // Opposite symbols
            velocity.x = 0;
        }
    }
    private void CheckMaxSpeed()
    {
        if (velocity.x > MaxSpeedX)
        {
            velocity.x = MaxSpeedX;
        } else if (velocity.x < -MaxSpeedX)
        {
            velocity.x = -MaxSpeedX;
        }

        if( velocity.y > MaxSpeedY)
        {
            velocity.y = MaxSpeedY;
        } else if(velocity.y < -MaxSpeedY)
        {
            velocity.y = -MaxSpeedY;
        }
    }
    private bool IsHorizontalPress()
    {
        List<PlayerAction> frameActions = pim.GetFrameActions();
        return (
           frameActions.Contains(PlayerAction.left) ||
           frameActions.Contains(PlayerAction.right)
        );
    }
    private void ApplyActions()
    {
        rb.velocity = velocity;
        lastVelocity = new Vector2(velocity.x,velocity.y);
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
