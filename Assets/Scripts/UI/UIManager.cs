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
    
    private void Awake()
    {
        Instance = this;
        _controls = new PlayerControls();
        _controls.Player.Escape.performed += tgb => OptionsToggle();
        UpdateUI(0);
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
        EnemyDetector.EnemyDied += UpdateUI;
    }

    private void OnDisable()
    {
        //Disables Player Input
        _controls.Player.Disable();
        EnemyDetector.EnemyDied -= UpdateUI;
    }

    public void Resume()
    {
        OptionsToggle();
    }

    public void Restart()
    {
        //Restarts Level
    }
    
    private void UpdateUI(int enemiesKilled)
    {
        killsText.SetText(enemiesKilled == 0 ? "0" : enemiesKilled.ToString("##"));
    }

    public void QuitToMainMenu()
    {
        //Returns to main menu
        SceneManager.LoadScene("MainMenu");
    }
}
