using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public enum PlayerInputFlag
{
    none = 0,
    left = 1,
    right = 2,
    up = 4,
    down = 8,
    jump = 16,
    run = 32,
    act1 = 64,
    act2 = 128,
    act3 = 256,
}
public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(instance.gameObject);
        }
    }

    public static short GetEnumShort()
    {
       if (instance == null) { throw new NullInputInstanceException("Instance is null"); }

        short inputs = 0;
        if (Input.GetKey(KeyCode.LeftArrow)) { inputs += (short)PlayerInputFlag.left; }
        if (Input.GetKey(KeyCode.RightArrow)) { inputs += (short)PlayerInputFlag.right; }
        if (Input.GetKey(KeyCode.UpArrow)) { inputs += (short)PlayerInputFlag.up; }
        if (Input.GetKey(KeyCode.DownArrow)) { inputs += (short)PlayerInputFlag.down; }
        if (Input.GetKey(KeyCode.Z)) { inputs += (short)PlayerInputFlag.jump; }
        if (Input.GetKey(KeyCode.LeftShift)) { inputs += (short)PlayerInputFlag.run; }
        if (Input.GetKey(KeyCode.X)) { inputs += (short)PlayerInputFlag.act1; }
        if (Input.GetKey(KeyCode.C)) { inputs += (short)PlayerInputFlag.act2; }
        if (Input.GetKey(KeyCode.V)) { inputs += (short)PlayerInputFlag.act3; }
        return inputs;
    }

    public static List<PlayerInputFlag> getInputList()
    {
        short inputs = GetEnumShort();
       
        BitArray bitArray = new BitArray(BitConverter.GetBytes(inputs));
        List<PlayerInputFlag> flagList = new List<PlayerInputFlag>();

        for (int i = 0; i < bitArray.Length; i++)
        {
            if (bitArray[i]) //returns true if bit is 1
            {
                // 1 << X == to 2^X, it shifts the bits by X so it == power
                PlayerInputFlag flag = (PlayerInputFlag)(1 << i);
                flagList.Add(flag);
            }
        }

        return flagList;
    }
}

public class NullInputInstanceException : Exception 
{
    string message;
    public NullInputInstanceException(string message)
    {
        this.message = message;
    }
    public string toString() { return message; }
}
