using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [SerializeField] private GameObject optionsMenu;
    private PlayerControls _controls;
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private RectTransform xpBar;
    
    private void Awake()
    {
        Instance = this;
        _controls = new PlayerControls();
        _controls.Player.Escape.performed += tgb => OptionsToggle();
    }

    private void Start()
    {
        UpdateXpBar(0);
        UpdateKills(0);
    }

    public void Play()
    {
        //Play
    }

    private void OptionsToggle()
    {
        //Toggles options menu depending on state recieved
        optionsMenu.SetActive(!optionsMenu.activeSelf);
    }
    
    private void OnEnable()
    {
        //Enables Player Input
        _controls.Player.Enable();
        EnemyDetector.EnemyDied += UpdateKills;
        GameManager.XpAdded += UpdateXpBar;
    }

    private void OnDisable()
    {
        //Disables Player Input
        _controls.Player.Disable();
        EnemyDetector.EnemyDied -= UpdateKills;
        GameManager.XpAdded -= UpdateXpBar;
    }

    public void Resume()
    {
        OptionsToggle();
    }

    public void Restart()
    {
        //Restarts Level
    }
    
    private void UpdateKills(int enemiesKilled)
    {
        killsText.SetText(enemiesKilled == 0 ? "0" : enemiesKilled.ToString("##"));
    }

    private void UpdateXpBar(int coinsCollected)
    {
        var xpToAdd = (float)coinsCollected / GameManager.Instance.TotalXp;
        var xpBarXScaler = Mathf.Clamp(xpToAdd, 0, 1);
        xpBar.localScale = new Vector3(xpBarXScaler, 1, 1);
    }

    public void QuitToMainMenu()
    {
        //Returns to main menu
        SceneManager.LoadScene("MainMenu");
    }
}
