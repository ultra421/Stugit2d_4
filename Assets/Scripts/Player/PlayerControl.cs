using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(Vector2.left * 3);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(Vector2.right * 3);
        }
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(Vector2.up * 3);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(Vector2.down * 3);
        }

    }

}
