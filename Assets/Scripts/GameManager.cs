using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static event Action<int> XpAdded;
    public static event Action<int> LevelIncreased;
    public static event Action ChangePhase;
    public static event Action LevelChanged;
    private float _timeAlive;
    public List<GameObject> enemiesSpawnedList = new();
    private InGameLevelLoader _levelLoader;
    public SpriteRenderer levelBackground;
    public EnemySpawner enemySpawner;
    public bool ToggleLevelUpScreen;
    
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

    public int CurrentCoins
    {
        get;
        set;
    }
    
    private void Awake()
    {
        Instance = this;
        TotalXp = 250;
        _levelLoader = Resources.Load<InGameLevelLoader>("InGameLevel");
    }

    private void Start()
    {
        levelBackground.sprite = _levelLoader.levelLoader.levelBackground;
        levelBackground.size = new Vector2(1000, 1000);
    }

    private void Update()
    {
        if (ToggleLevelUpScreen)
        {
            LevelChanged?.Invoke();
            ToggleLevelUpScreen = false;
        }
        
        // Play time keeper
        _timeAlive += Time.deltaTime;
        //Converts time alive to minutes and seconds
        var min = GetMinutes();
        var sec = GetSeconds();
        var timerTextMin = min < 10 ? "0" + min + ":" : min + ":";
        timerTextMin += sec < 10 ? "0" + sec : sec;
        UIManager.Instance.timerText.SetText(timerTextMin);
        if(ChangePhaseCheck())
            ChangePhase?.Invoke();
        //Debug.Log(min + ":" + sec);
    }

    private bool ChangePhaseCheck()
    {
        if (_timeAlive <= 1) return false;
        return GetMinutes() % 5 == 0 && GetSeconds() == 0;
    }

    private int GetMinutes()
    {
        return Mathf.FloorToInt(_timeAlive / 60);
    }

    private int GetSeconds()
    {
        return Mathf.FloorToInt(_timeAlive - GetMinutes() * 60);
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

    public static void AddCoins(int coinsToAdd)
    {
        Instance.CurrentCoins += coinsToAdd;
        UIManager.UpdateCoins(Instance.CurrentCoins);
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
        LevelChanged?.Invoke();
    }
}
