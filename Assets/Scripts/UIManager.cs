using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [SerializeField] private GameObject optionsMenu;
    private PlayerControls _controls;

    private void Awake()
    {
        Instance = this;
        _controls = new PlayerControls();
        _controls.Player.Escape.performed += tgb => OptionsToggle();
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
    }

    private void OnDisable()
    {
        //Disables Player Input
        _controls.Player.Disable();
    }

    public void Resume()
    {
        OptionsToggle();
    }

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
