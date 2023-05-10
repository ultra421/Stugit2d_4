using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerCollisionManager : MonoBehaviour
{
    PlayerActionController controller;

    private void Start()
    {
        controller = gameObject.GetComponentInParent<PlayerActionController>();
        //Ignore all gameobjects that are balls
        GameObject[] ballList = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ball in ballList)
        {
            Debug.Log("Ignoring " + ball);
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), ball.GetComponent<CircleCollider2D>());
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("SolidGround"))
        {
            GroundCollision(collision);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("SolidGround"))
        {
            LeaveGround(collision);
        }
    }

    private void GroundCollision(Collision2D collision)
    {
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contacts.Length];
        collision.GetContacts(contacts);
        int count = 0;
        bool isGroundChanged = false;
        foreach (ContactPoint2D contact in contacts)
        {
            Debug.DrawLine(contact.point, this.transform.position, Color.red);
            //Debug.Log("Contact at " + contact.point);
            //Sum to average to get the average
            float angle = Vector2.Angle(contact.normal, Vector2.up);
            //Debug.Log("Angle between point " + count + " " + angle);
            count++;

            if (angle > 45)
            { //Is wall

            }else if (angle < 45)
            { //Ground
                isGroundChanged = true;
                controller.SetGround(true);
            }
        }
        //If isGround has not changed means that ground has not been touched, therefore, set ground to false
        if (!isGroundChanged)
        {
            controller.SetGround(false);
        }
    }

    private void LeaveGround(Collision2D collision)
    {
        controller.SetGround(false);
        return;
    }
}
