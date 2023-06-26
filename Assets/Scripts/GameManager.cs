using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static event Action<int> XpAdded;
    private float _timeAlive;

    public int TotalXp
    {
        get;
        set;
    }
    
    public int CurrentXp
    {
        get;
        set;
    }
    
    private void Awake()
    {
        Instance = this;
        TotalXp = 100;
    }

    private void Update()
    {
        // Play time keeper
        _timeAlive += Time.deltaTime;
        //Converts time alive to minutes and seconds
        var min = Mathf.FloorToInt(_timeAlive / 60);
        var sec = Mathf.FloorToInt(_timeAlive - min * 60);
        
        //Debug.Log(min + ":" + sec);
    }

    public void AddXp(int xp)
    {
        //Adds Xp
        CurrentXp += xp;
        XpAdded?.Invoke(CurrentXp);
    }
}
