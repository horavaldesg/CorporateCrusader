using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static event Action<int> XpAdded;
    public static event Action<int> LevelIncreased;
    private float _timeAlive;

    public int TotalXp
    {
        get;
        private set;
    }
    
    private int CurrentXp
    {
        get;
        set;
    }

    public int CurrentLevel
    {
        get;
        set;
    }
    
    private void Awake()
    {
        Instance = this;
        TotalXp = 250;
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

    public static void AddXp(int xp)
    {
        //Adds Xp
        Instance.CurrentXp += xp;
        if (Instance.CheckLevelUpgrade())
        {
            Instance.UpgradeLevel();
        }
        
        UIManager.UpdateXpBar(Instance.CurrentXp);
        //XpAdded?.Invoke(Instance.CurrentXp);
    }

    private bool CheckLevelUpgrade()
    {
        return Instance.CurrentXp >= Instance.TotalXp;
    }

    private void UpgradeLevel()
    {
        TotalXp += 5; // Come up with formula that increases total xp required per level
        CurrentXp = 0;
        AddLevel();
    }

    private void AddLevel()
    {
        CurrentLevel++;
        LevelIncreased?.Invoke(CurrentLevel);
    }
}
