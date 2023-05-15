using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScriptManager : MonoBehaviour
{
    public static GameScriptManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this.gameObject);
        }
    }


}
