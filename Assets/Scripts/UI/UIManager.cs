using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private TMP_Text enemiesKilledText;
    [SerializeField] private TMP_Text levelText;
    public TMP_Text timerText;
    [SerializeField] private RectTransform xpBar;
    private Image _xpBarFill;
    private PlayerControls _controls;
    private Animator _anim;
    private bool _canAddLevel;
    
    private void Awake()
    {
        Instance = this;

        _controls = new PlayerControls();
        _controls.Player.Escape.performed += tgb => ToggleOptions();
        xpBar.TryGetComponent(out _xpBarFill);
        _anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        //Enables Player Input
        _controls.Player.Enable();
        EnemyDetector.EnemyDied += UpdateKills;
        GameManager.LevelIncreased += LevelUpdated;
    }

    private void OnDisable()
    {
        //Disables Player Input
        _controls.Player.Disable();
        EnemyDetector.EnemyDied -= UpdateKills;
        GameManager.XpAdded -= UpdateXpBar;
        GameManager.LevelIncreased -= LevelUpdated;
    }

    private void Start()
    {
        UpdateKills(0);
        UpdateXpBar(0);
        LevelUpdated(GameManager.Instance.CurrentLevel = 1);
    }

    private void UpdateKills(int enemiesKilled)
    {
        enemiesKilledText.SetText(enemiesKilled == 0 ? "0" : enemiesKilled.ToString("##"));
    }

    public static void UpdateXpBar(int coinsCollected)
    {
        var xpToAdd = (float)coinsCollected / GameManager.Instance.TotalXp;
        var xpBarXScaler = Mathf.Clamp(xpToAdd, 0, 1);
        Instance._xpBarFill.fillAmount = xpBarXScaler;
        if (Instance._xpBarFill.fillAmount >= 1)
        {
            Instance._xpBarFill.fillAmount = 0;
        }
    }

    private void LevelUpdated(int level)
    {
        levelText.SetText(level == 0 ? "Level 0" : "Level "  + level);
    }

    public void ToggleOptions() => _anim.SetTrigger("ToggleOptions");
    public void ToggleInventory() => _anim.SetTrigger("ToggleInventory");

    public void Restart()
    {
        //Restarts Level
    }

    public void QuitToMainMenu()
    {
        //Returns to main menu
        SceneManager.LoadScene("MainMenu");
    }
}
