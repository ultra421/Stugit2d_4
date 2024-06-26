using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAction
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
    private Dictionary<PlayerAction, float> timePressed;
    private Dictionary<PlayerAction, float> timeSincePressed;
    private List<PlayerAction> thisFrameActions;

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

    private void Start()
    {
        timePressed = new Dictionary<PlayerAction, float>();
        timeSincePressed = new Dictionary<PlayerAction, float>();
        foreach (PlayerAction action in Enum.GetValues(typeof(PlayerAction)))
        {
            timePressed.Add(action, 0);
            timeSincePressed.Add(action, 0);
        }
        thisFrameActions = new List<PlayerAction>();
    }

    //Executes before any other script (-1 on Script Execution Order)
    private void FixedUpdate()
    {
        //Get pressed actions as a List
        GetPressedActions();
        //Tick the time they have been pressed
        TickTime();
        //Used for early inputs
        InputCompensation();
    }

    private void LateUpdate()
    {
        //Clear this frame's frameActions after FixedUpdate has been processed
        thisFrameActions.Clear();
    }

    private void GetPressedActions()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) { thisFrameActions.Add(PlayerAction.left); }
        if (Input.GetKey(KeyCode.RightArrow)) { thisFrameActions.Add(PlayerAction.right); }
        if (Input.GetKey(KeyCode.UpArrow)) { thisFrameActions.Add(PlayerAction.up); }
        if (Input.GetKey(KeyCode.DownArrow)) { thisFrameActions.Add(PlayerAction.down); }
        if (Input.GetKey(KeyCode.Z)) { thisFrameActions.Add(PlayerAction.jump); }
        if (Input.GetKey(KeyCode.LeftShift)) { thisFrameActions.Add(PlayerAction.run); }
        if (Input.GetKey(KeyCode.X)) { thisFrameActions.Add(PlayerAction.act1); }
        if (Input.GetKey(KeyCode.V)) { thisFrameActions.Add(PlayerAction.act2); }
        if (Input.GetKey(KeyCode.C)) { thisFrameActions.Add(PlayerAction.act3); }
    }

    /* Iterates over all the possible actions and sums the time passed to the ones that have been pressed
     * And the ones that haven't are reset to 0 */
    private void TickTime()
    {
        foreach (PlayerAction action in Enum.GetValues(typeof(PlayerAction))) {
            if (thisFrameActions.Contains(action))
            {
                timePressed[action] += Time.deltaTime;
            } else
            {
                timePressed[action] = 0;
                timeSincePressed[action] += Time.deltaTime;
            }
        }
    }
    //Used for compensation of inputs, such as pressing jump before touching ground
    private void InputCompensation()
    {
        if (timeSincePressed[PlayerAction.jump] < 0.15f)
        {
            thisFrameActions.Add(PlayerAction.jump);
        }
    }
    public bool GetAction(PlayerAction action)
    {
        return thisFrameActions.Contains(action);
    }

    public bool GetActionDown(PlayerAction action)
    {
        return timePressed[action] >= Time.deltaTime;
    }

    public float ActionTime(PlayerAction action)
    {
        return timePressed[action];
    }

    public List<PlayerAction> GetFrameActions()
    {
        return thisFrameActions;
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
