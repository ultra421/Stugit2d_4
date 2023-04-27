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
        isGround = false;
    }

    public void GetSettings()
    {

    }
    public void MoveLeft()
    {

    }
    public void MoveRight()
    {

    }

}
