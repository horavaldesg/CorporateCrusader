using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static event Action<float> XpAdded;
    public static event Action<int> LevelIncreased;
    public static event Action ChangePhase;
    public static event Action EnemyIndexIncrease;
    public static event Action LevelChanged;
    public static event Action<EnemyContainer> EnemiesLoaded;
    private float _timeAlive;
    public List<GameObject> enemiesSpawnedList = new();
    private InGameLevelLoader _levelLoader;
    public SpriteRenderer levelBackground;
    
    public bool ToggleLevelUpScreen;

    [Header("Drop Crates")]
    [SerializeField] private GameObject crateObject;

    [Tooltip("Every x seconds")]
    [SerializeField] private int crateSpawnRate;

    [SerializeField] private int maxCrates;
    
    private bool _canSpawnCrate;
    private List<GameObject> _cratesAdded = new();
    
    public int TotalXp
    {
        get;
        private set;
    }
    
    private float CurrentXp
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

    public List<int> _luckCoinList = new();
  
    
    
    private void Awake()
    {
        Instance = this;
        TotalXp = 1000;
        _levelLoader = Resources.Load<InGameLevelLoader>("InGameLevel");
    }

    private void Start()
    {
        EnemiesLoaded?.Invoke(_levelLoader.levelLoader.enemiesThatSpawn);
        levelBackground.sprite = _levelLoader.levelLoader.levelBackground;
        levelBackground.size = new Vector2(1000, 1000);
        _canSpawnCrate = true;
        SetLuckCoinList(50);
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
        
        if(IncreaseEnemyIndex() && CanInvoke)
        {
            EnemyIndexIncrease?.Invoke();
            CanInvoke = false;
        }

        if (!CanInvoke)
        {
            CanInvoke = GetSeconds() > 1;
        }

        //Add Max Crate Count
        if (CheckSeconds(crateSpawnRate) && _canSpawnCrate)
        {
            SpawnCrates();
            _canSpawnCrate = false;
        }

        if (!_canSpawnCrate)
        {
            //_canSpawnCrate = !CheckSeconds(crateSpawnRate + 1);
        }
        //Debug.Log(min + ":" + sec);
    }

    private bool ChangePhaseCheck()
    {
        if (_timeAlive <= 1) return false;
        return GetMinutes() % 5 == 0 && GetSeconds() == 0;
    }

    private bool CanInvoke
    {
        get;
        set;
    }

    private bool IncreaseEnemyIndex()
    {
        if (_timeAlive <= 1) return false;
        return GetMinutes() % 2 == 0 && GetSeconds() == 0;
    }

    public int GetMinutes()
    {
        return Mathf.FloorToInt(_timeAlive / 60);
    }

    public int GetSeconds()
    {
        return Mathf.FloorToInt(_timeAlive - GetMinutes() * 60);
    }
    
    public bool CheckSeconds(int number)
    {
        return GetSeconds() % number == 0 && GetSeconds() != 0;
    }

    public static void AddXp(int xp)
    {
        //Adds Xp
        var xpMulti = PlayerController.Instance.xPMultiplier;
        var xpToAdd = xp * xpMulti;
        xpToAdd += xp;
        Instance.CurrentXp += xpToAdd;
        if (Instance.CheckLevelUpgrade())
        {
            Instance.UpgradeLevel();
        }
        
        UIManager.UpdateXpBar(Instance.CurrentXp);
        //XpAdded?.Invoke(Instance.CurrentXp);
    }

    public static void AddCoins(int coinsToAdd)
    {
        var playerCoins = PlayerController.Instance.coinMultiplier;
        var coinGain = coinsToAdd + playerCoins;
        Instance.CurrentCoins += coinGain;
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

    private void SpawnCrates()
    {
        var crate = Instantiate(crateObject);
        crate.transform.position = EnemySpawner.Instance.GetRadius(5);
        AddCrate(crate);
    }

    private void AddCrate(GameObject crate)
    {
        _cratesAdded.Add(crate);
    }

    public void RemoveCrate(GameObject crate)
    {
        if (_cratesAdded.Contains(crate))
        {
            _cratesAdded.Remove(crate);
        }
    }

    public void KillAllEnemies()
    {
        foreach (var enemy in enemiesSpawnedList)
        {
            enemy.TryGetComponent(out Enemy enemyComp);
            if(enemyComp)
                enemyComp.TakeDamage(enemyComp.health, SelectedWeapon.Attributes.Blunt);
        }
    }

    public void SetLuckCoinList(int percentage)
    {
        _luckCoinList.Clear();

        for (var i = 0; i < percentage - 10; i++)
        {
            if (i % 2 == 0) _luckCoinList.Add(i);
        }

        for (var i = 1; i < percentage; i++)
        {
            if (i % 2 == 1) _luckCoinList.Add(i);
        }
    }
}
