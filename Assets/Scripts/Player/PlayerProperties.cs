using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PlayerProperties {
    public float MaxSpeedX { get;set; }
    public float MaxSpeedY { get;set; }
    public Vector2 Speed { get;set; }
    public Vector2 MaxSpeedVector { get;set; }
    public float Gravity { get; set; }
    public float InitialJumpSpeed { get; set; }
    public float AccelX { get; set; }
    public bool IsGround { get; set; }
    public Vector2 Pos { get; set; }


    public PlayerProperties(Rigidbody2D rb)
    {
        InitDefaultValues(rb);
    }
    public PlayerProperties(PlayerProperties newProperties)
    {
        //Get all properties
        PropertyInfo[] properties = typeof(PlayerProperties).GetProperties();
        foreach(PropertyInfo property in properties)
        {
            //Iterate trough list and set values of THIS object to the Incoming
            property.SetValue(this, property.GetValue(newProperties));
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

    private void InitDefaultValues(Rigidbody2D rb)
    {
        MaxSpeedX = 5f;
        MaxSpeedY = 15f;
        Speed = new Vector2(0, 0);
        MaxSpeedVector = new Vector2(MaxSpeedX, MaxSpeedY);
        Gravity = 20f;
        InitialJumpSpeed = 10f;
        AccelX = 15f;
        IsGround = false;
        Pos = rb.position;
    }

}
