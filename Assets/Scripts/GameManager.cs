using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void AddXp()
    {
        //Adds Xp
    }

    private void CurrentXp()
    {
        //Current Player XP
    }
}
