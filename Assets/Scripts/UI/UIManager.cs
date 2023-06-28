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
    [SerializeField] private TMP_Text enemiesKilledText;
    [SerializeField] private RectTransform xpBar;

    private PlayerControls _controls;
    private Animator _anim;
    
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
        GameManager.XpAdded += UpdateXpBar;
    }

    private void OnDisable()
    {
        //Disables Player Input
        _controls.Player.Disable();
        EnemyDetector.EnemyDied -= UpdateKills;
        GameManager.XpAdded -= UpdateXpBar;
    }

    private void Start()
    {
        UpdateKills(0);
        UpdateXpBar(0);
    }

    private void UpdateKills(int enemiesKilled)
    {
        enemiesKilledText.SetText(enemiesKilled == 0 ? "0" : enemiesKilled.ToString("##"));
    }

    private void UpdateXpBar(int coinsCollected)
    {
        var xpToAdd = (float)coinsCollected / GameManager.Instance.TotalXp;
        var xpBarXScaler = Mathf.Clamp(xpToAdd, 0, 1);
        xpBar.localScale = new Vector3(xpBarXScaler, 1, 1);
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
