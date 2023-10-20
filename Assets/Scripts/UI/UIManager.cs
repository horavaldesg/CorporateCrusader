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

    [SerializeField] private int profileXPForVictory = 20;

    [Header("HUD UI References")]
    [SerializeField] private TMP_Text enemiesKilledText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image xpBarFill;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private GameObject levelUpScreen;
    
    [Header("Inventory UI References")]
    [SerializeField] private Transform invRow1;
    [SerializeField] private Transform invRow2;
    [SerializeField] private List<Transform> last3InvItems;
    [SerializeField] private InventoryItem[] inventoryItems = new InventoryItem[6];

    [Header("Game Over UI References")]
    [SerializeField] private TMP_Text timeSurvivedText;
    [SerializeField] private TMP_Text enemiesDefeatedGOText;
    [SerializeField] private Transform coinsCollectedGOPanel;
    [SerializeField] private Transform xpEarnedGOPanel;

    [Header("Victory UI References")]
    [SerializeField] private TMP_Text enemiesDefeatedVText;
    [SerializeField] private Transform coinsCollectedVPanel;
    [SerializeField] private Transform xpEarnedVPanel;

    private PlayerControls _controls;
    private Animator _anim;
    private bool _canAddLevel;
    private bool _inventoryOpen = false;
    private int _numDeaths = 0;
    private ScreenOrientation currentOrientation;

    private enum InventoryDisplay { Weapons, Equipment, Hats };

    private InventoryDisplay inventoryDisplay;
    
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
        if(Time.timeScale != 1) Time.timeScale = 1; //make sure game isn't paused
        
        UpdateKills(0);
        UpdateXpBar(0);
        UpdateCoins(0);
        LevelUpdated(GameManager.Instance.CurrentLevel = 1);

        currentOrientation = Screen.orientation;
        inventoryDisplay = InventoryDisplay.Weapons;
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
        var xpToAdd = coinsCollected / GameManager.Instance.TotalXp;
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

    private void UpdateInventoryDisplay()
    {
        switch(inventoryDisplay)
        {
            case InventoryDisplay.Weapons:
                //loop through inventory items (6 inventory panels)
                for (var i = 0; i < WeaponManager.Instance.weaponsAdded.Count; i++)
                {
                    inventoryItems[i].SetWeaponDisplay(WeaponManager.Instance.weaponsAdded[i]);
                   
                   
                    //NOTE: GET CURRENT SELECTED WEAPON AT INDEX i
                    //IF NO WEAPON AT INDEX i, LEAVE WEAPON = NULL

                    //if no weapon, hide display, otherwise set weapon display
                    // if(!canShow) inventoryItems[i].HideDisplay();
                }

                for (var i = WeaponManager.Instance.weaponsAdded.Count; i < inventoryItems.Length; i++)
                {
                    inventoryItems[i].HideDisplay();
                }

                break;
            case InventoryDisplay.Equipment:
                //loop through inventory items (6 inventory panels)
                for (var i = 0; i < WeaponManager.Instance.equipmentAdded.Count; i++)
                {
                    inventoryItems[i].SetEquipmentDisplay(WeaponManager.Instance.equipmentAdded[i]);
                }
                for (var i = WeaponManager.Instance.equipmentAdded.Count; i < inventoryItems.Length; i++)
                {
                    inventoryItems[i].HideDisplay();
                }
               
                    //NOTE: GET CURRENT SELECTED EQUIPMENT AT INDEX i
                    //IF NO EQUIPMENT AT INDEX i, LEAVE EQUIPMENT = NULL

                    //if no equipment, hide display, otherwise set equipment display
                  
                break;
            case InventoryDisplay.Hats:
                //loop through inventory items (6 inventory panels)
                for(int i = 0; i < HatSelection.Instance.GetChosenHats().Count; i++)
                {
                    //NOTE: GET CURRENT SELECTED HAT AT INDEX i

                    //set hat display
                    inventoryItems[i].SetHatDisplay(HatSelection.Instance.GetChosenHats()[i]);
                }
                
                for (var i = HatSelection.Instance.GetChosenHats().Count; i < inventoryItems.Length; i++)
                {
                    inventoryItems[i].HideDisplay();
                }
                
                break;
        }
    }

    public void ToggleOptions()
    { 
        _anim.SetTrigger("ToggleOptions");
        ToggleGamePaused();
    }

    public void ToggleLevelUpScreen()
    {
        levelUpScreen.SetActive(!levelUpScreen.activeSelf);
        ToggleGamePaused();
    }

    private void ToggleGamePaused() => Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    
    public void ToggleInventory()
    {
        //update inventory open bool
        _inventoryOpen = !_inventoryOpen;

        //fix rows and update displayed items if inventory opened
        if(_inventoryOpen)
        {
            UpdateInventoryRows(currentOrientation);
            UpdateInventoryDisplay();
        }
        ToggleGamePaused();
        _anim.SetTrigger("ToggleInventory");
    }

    public void WeaponsDisplayButton()
    {
        //first check if already displaying this
        if(inventoryDisplay == InventoryDisplay.Weapons) return;

        //set and update current inventory display
        inventoryDisplay = InventoryDisplay.Weapons;
        UpdateInventoryDisplay();
    }

    public void EquipmentDisplayButton()
    {
        //first check if already displaying this
        if(inventoryDisplay == InventoryDisplay.Equipment) return;

        //set and update current inventory display
        inventoryDisplay = InventoryDisplay.Equipment;
        UpdateInventoryDisplay();
    }

    public void HatsDisplayButton()
    {
        //first check if already displaying this
        if(inventoryDisplay == InventoryDisplay.Hats) return;

        //set and update current inventory display
        inventoryDisplay = InventoryDisplay.Hats;
        UpdateInventoryDisplay();
    }

    //fades out screen before restarting the level
    public void RestartButton() => _anim.SetTrigger("FadeOutToRestart");

    public void RestartLevel()
    {
        //restarts level
        var currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    //fades out screen before returning to main menu
    public void QuitButton() => _anim.SetTrigger("FadeOut");

    //returns to main menu
    public void ReturnToMainMenu() => SceneManager.LoadScene("MainMenu");

    #region Death Screen UI

    public void PlayerDeath()
    {
        ToggleGamePaused();

        if(_numDeaths == 0) _anim.SetTrigger("FirstDeath");
        else
        {
            InitGameOverPanel(); //set values on game over screen
            _anim.SetTrigger("SecondDeath");
        }

        _numDeaths++; //increment death count
    }

    public void WatchedAd()
    {
        ToggleGamePaused(); //unpause game
        _anim.SetTrigger("WatchedAd"); //hide "keep playing" screen
        PlayerController.Instance.RevivePlayer(); //heal player
    }

    public void SkipAdButton()
    {
        InitGameOverPanel(); //set values on game over screen
        _anim.SetTrigger("SkipAd");
    }

    private void InitGameOverPanel()
    {
        timeSurvivedText.text = timerText.text; //set time survived
        enemiesDefeatedGOText.text = "Enemies Defeated: " + enemiesKilledText.text; //set enemies defeated text

        if(int.Parse(coinsText.text) == 0) coinsCollectedGOPanel.gameObject.SetActive(false); //hide coins collected panel if none were collected
        else coinsCollectedGOPanel.GetComponentInChildren<TMP_Text>().text = "x" + coinsText.text; //else set coins collected text

        //NOTE: Determine if we want to give profile xp even if you fail a level and how much that would be
        //for now just hiding xp earned panel
        xpEarnedGOPanel.gameObject.SetActive(false);
    }

    //initialize rewards before fading out screen and returning to main menu
    public void ContinueGOButton()
    {
        RewardsManager.Instance.InitializeRewards();
        RewardsManager.Instance.CoinsCollected = int.Parse(coinsText.text);
        RewardsManager.Instance.ProfileXPEarned = 0; //NOTE: update this with actual xp earned if we add that above
        _anim.SetTrigger("FadeOut");
    }

    #endregion

    #region Victory Screen UI

    public void GameWon()
    {
        ToggleGamePaused(); //pause game

        enemiesDefeatedVText.text = "Enemies Defeated: " + enemiesKilledText.text; //set enemies defeated text

        if(int.Parse(coinsText.text) == 0) coinsCollectedVPanel.gameObject.SetActive(false); //hide coins collected panel if none were collected
        else coinsCollectedVPanel.GetComponentInChildren<TMP_Text>().text = "x" + coinsText.text; //else set coins collected text

        xpEarnedVPanel.GetChild(0).GetComponent<TMP_Text>().text = "+" + profileXPForVictory; //set profile xp earned text

        _anim.SetTrigger("GameWon");
    }

    //initialize rewards before fading out screen and returning to main menu
    public void ContinueVButton()
    {
        RewardsManager.Instance.InitializeRewards();
        RewardsManager.Instance.CoinsCollected = int.Parse(coinsText.text);
        RewardsManager.Instance.ProfileXPEarned = profileXPForVictory;
        _anim.SetTrigger("FadeOut");
    }

    #endregion
}
