using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    public TMP_Text timerText;

    [SerializeField] private TMP_Text enemiesKilledText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image xpBarFill;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private GameObject levelUpScreen;
    
    [Header("Inventory References")]
    [SerializeField] private Transform invRow1;
    [SerializeField] private Transform invRow2;
    [SerializeField] private List<Transform> last3InvItems;

    private PlayerControls _controls;
    private Animator _anim;
    private bool _canAddLevel;
    private bool _inventoryOpen = false;
    private ScreenOrientation currentOrientation;
    
    private void Awake()
    {
        Instance = this;

        _controls = new PlayerControls();
        _controls.Player.Escape.performed += tgb => ToggleOptions();

        _anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        //Enables Player Input
        _controls.Player.Enable();
        EnemyDetector.EnemyDied += UpdateKills;
        CloudSaveManager.AddKills += UpdateKills;
        GameManager.LevelIncreased += LevelUpdated;
        GameManager.XpAdded += UpdateXpBar;
        GameManager.LevelChanged += ToggleLevelUpScreen;
        LevelUpUpgradeManager.UpgradeEnded += ToggleLevelUpScreen;
    }

    private void OnDisable()
    {
        //Disables Player Input
        _controls.Player.Disable();
        EnemyDetector.EnemyDied -= UpdateKills;
        CloudSaveManager.AddKills -= UpdateKills;
        GameManager.XpAdded -= UpdateXpBar;
        GameManager.LevelIncreased -= LevelUpdated;
        GameManager.LevelChanged -= ToggleLevelUpScreen;
        LevelUpUpgradeManager.UpgradeEnded -= ToggleLevelUpScreen;
    }

    private void Start()
    {
        UpdateKills(0);
        UpdateXpBar(0);
        UpdateCoins(0);
        LevelUpdated(GameManager.Instance.CurrentLevel = 1);

        currentOrientation = Screen.orientation;
    }

    private void Update()
    {
        //check if screen orientation changed
        if(currentOrientation != Screen.orientation)
        {
            //update screen orientation
            currentOrientation = Screen.orientation;

            //update screen selection buttons
            if(_inventoryOpen) UpdateInventoryRows(currentOrientation);
        }
    }

    private void UpdateKills(int enemiesKilled)
    {
        enemiesKilledText.SetText(enemiesKilled == 0 ? "0" : enemiesKilled.ToString("##"));
    }

    public static void UpdateXpBar(float coinsCollected)
    {
        var xpToAdd = (float)coinsCollected / GameManager.Instance.TotalXp;
        var xpBarXScaler = Mathf.Clamp(xpToAdd, 0, 1);
        Instance.xpBarFill.fillAmount = xpBarXScaler;
        if (Instance.xpBarFill.fillAmount >= 1)
        {
            Instance.xpBarFill.fillAmount = 0;
        }
    }

    public static void UpdateCoins(int coinsCollected)
    {
        Instance.coinsText.SetText(coinsCollected == 0 ? "0" : coinsCollected.ToString("##"));
    }

    private void LevelUpdated(int level)
    {
        levelText.SetText(level == 0 ? "Level 0" : "Level "  + level);
    }

    private void UpdateInventoryRows(ScreenOrientation orientation)
    {
        //fix inventory rows based on current screen orientation
        switch(orientation)
        {
            case ScreenOrientation.Portrait:
            case ScreenOrientation.PortraitUpsideDown:
                invRow2.gameObject.SetActive(true);
                last3InvItems[0].SetParent(invRow2);
                last3InvItems[1].SetParent(invRow2);
                last3InvItems[2].SetParent(invRow2);
                break;
            case ScreenOrientation.LandscapeLeft:
            case ScreenOrientation.LandscapeRight:
                if(invRow2.childCount > 0)
                {
                    last3InvItems[0].SetParent(invRow1);
                    last3InvItems[1].SetParent(invRow1);
                    last3InvItems[2].SetParent(invRow1);
                    invRow2.gameObject.SetActive(false);
                }
                break;
        }

    }

    public void ToggleOptions()
    { 
        _anim.SetTrigger("ToggleOptions");
        StopTime();
    }

    public void ToggleLevelUpScreen()
    {
        levelUpScreen.SetActive(!levelUpScreen.activeSelf);
        StopTime();
    }

    private void StopTime()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }
    
    public void ToggleInventory()
    {
        //update inventory open bool and fix rows if inventory opened
        _inventoryOpen = !_inventoryOpen;
        if(_inventoryOpen) UpdateInventoryRows(currentOrientation);
        StopTime();
        _anim.SetTrigger("ToggleInventory");
    }

    public void Restart()
    {
        //Restarts Level
        StopTime();
        var currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    public void QuitToMainMenu()
    {
        //Returns to main menu
        StopTime();
        SceneManager.LoadScene("MainMenu");
    }
}
