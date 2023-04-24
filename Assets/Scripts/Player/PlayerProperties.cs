using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PlayerProperties : MonoBehaviour {

    [Header("Transform")]
    public Vector2 Speed;
    public Vector2 Pos;
    [Header("Properties")]
    public float Gravity;
    public float InitialJumpSpeed;
    public float AccelX;
    public byte PlayerID;
    public Vector2 MaxSpeedVector;
    public float MaxSpeedX;
    public float MaxSpeedY;
    public bool IsGround;
    public float DragAccelX;
    float airTime;


    public void Start()
    {
        InitDefaultValues();
    }

    public PlayerProperties()
    {
        InitDefaultValues();
    }
    public PlayerProperties(PlayerProperties newProperties)
    {
        //Get all properties
        PropertyInfo[] properties = typeof(PlayerProperties).GetProperties();
        foreach(PropertyInfo property in properties)
        {
            if (property.GetValue(newProperties) != null)
            {
                //Iterate trough list and set values of THIS object to the Incoming
                property.SetValue(this, property.GetValue(newProperties));
            } 
        }
    }

    public void UpdateProperties(PlayerProperties newProperties)
    {
        //Get all properties
        PropertyInfo[] properties = typeof(PlayerProperties).GetProperties();
        foreach (PropertyInfo property in properties)
        {
            //Iterate trough list and set values of THIS object to the Incoming
            property.SetValue(this, property.GetValue(newProperties));
        }
    }

    private void InitDefaultValues()
    {
        PlayerID = 0;
        MaxSpeedX = 9f;
        MaxSpeedY = 15f;
        Speed = new Vector2(0, 0);
        MaxSpeedVector = new Vector2(MaxSpeedX, MaxSpeedY);
        Gravity = 30f;
        InitialJumpSpeed = 13f;
        AccelX = 25;
        IsGround = false;
        Pos = new Vector2(0, 0);
        DragAccelX = 35;
    }

    public void FixedUpdate()
    {
        CheckAirTime();
    }

    private void CheckAirTime()
    {
        if (!IsGround)
        {
            airTime += Time.deltaTime;
        } else if (IsGround)
        {
            airTime = 0;
        }
    }

}
