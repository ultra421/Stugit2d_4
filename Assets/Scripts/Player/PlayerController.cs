using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInputManager pim;
    [Header("Positional")]
    public Vector2 velocity, position;
    [Header("Attribtues")]
    public float MaxSpeedX;
    public float MaxSpeedY;
    public float AccelX;
    public float jumpVel;
    public float gravity;
    public bool isGround;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = this.transform.GetComponent<Rigidbody2D>();
        position = rb.position;
        velocity = rb.velocity;

        MaxSpeedX = 5f;
        MaxSpeedY = 15f;
        AccelX = 25f;
        jumpVel = 15f;
        gravity = 19.6f;
        isGround = false;

        pim = PlayerInputManager.instance;
    }

    private void FixedUpdate()
    {
        PreProcessChecks(); //Restores and prepares variables for processing
        ProcessActions();   //Processes actions read from PlayerInputManager 
        Gravity();          //Gravity
        ApplyActions();     //Applies the actions to the rigidbody
    }
    private void PreProcessChecks()
    {
        position = rb.position;
        velocity = rb.velocity;
    }
    private void ProcessActions()
    {
        List<PlayerAction> thisFrameActions = pim.GetFrameActions();
        foreach (PlayerAction action in thisFrameActions)
        {
            Debug.Log("Frame action:" + action.ToString() + " pressed for " + pim.ActionTime(action));
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
        if (isGround)
        {
            velocity.y = jumpVel;
        }
    }
    private void Gravity()
    {
        if (!isGround)
        {
            velocity.y -= gravity * Time.deltaTime;
        }
    }
    private void ApplyActions()
    {
        rb.velocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("SolidGround"))
        {
            Debug.Log("Touched Ground");
            isGround = true;
            velocity.y = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("SolidGround"))
        {
            Debug.Log("Left ground");
            isGround = false;
        }
    }

}
