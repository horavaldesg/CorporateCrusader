using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatSelection : MonoBehaviour
{
    public static HatSelection Instance;
    
    public Hat _chosenHat;
    public List<Hat.ChosenHat> chosenHats;

    private void Awake()
    {
        Instance = this;
        _chosenHat = Resources.Load<Hat>("PlayerStats/ChosenHats");
    }

    private void Start()
    {
        chosenHats = _chosenHat.chosenHats;
    }

    public List<Hat.ChosenHat> GetChosenHats()
    {
        return chosenHats;
    }
}
