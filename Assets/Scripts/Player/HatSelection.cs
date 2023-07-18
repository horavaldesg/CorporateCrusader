using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatSelection : MonoBehaviour
{
    private Hat _chosenHat;
    public List<Hat.ChosenHat> chosenHats;

    private void Awake()
    {
        _chosenHat = Resources.Load<Hat>("PlayerStats/ChosenHats");
    }

    private void Start()
    {
        chosenHats = _chosenHat.chosenHats;
    }
}
