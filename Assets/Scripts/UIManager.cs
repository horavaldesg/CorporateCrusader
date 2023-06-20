using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [SerializeField] private GameObject optionsMenu;

    private void Awake()
    {
        Instance = this;
    }

    public void Play()
    {
        //Play
    }

    public void OptionsToggle(bool state)
    {
        //Toggles options menu depending on state recieved
        optionsMenu.SetActive(true);
    }

    public void Restart()
    {
        //Restarts Level
    }

    public void Quit()
    {
        //Returns to main menu
    }
}
