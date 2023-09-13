using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private Image[] joysticks;
    private const float MinTransparency = 0.25f;
    private const float MaxTransparency = 1;
    
    private void Start()
    {
        foreach (var joystick in joysticks)
        {
            var newAlpha = joystick.color;
            newAlpha.a = MinTransparency;
            joystick.color = newAlpha;
        }
    }

    private void OnEnable()
    {
        PlayerController.MoveStickAction += ToggleTransparency;
        PlayerController.LookStickAction += ToggleTransparency;
    }

    private void OnDisable()
    {
        PlayerController.MoveStickAction -= ToggleTransparency;
        PlayerController.LookStickAction -= ToggleTransparency;
    }

    private void ToggleTransparency(object[] param)
    {
        var state = (bool)param[0];
        var changeName = (string)param[1];
        var whatImageToChange = changeName == PlayerController.LeftStick ? joysticks[0] : joysticks[1];
        var transparency = state ?  MaxTransparency : MinTransparency;
        
        var newAlpha = whatImageToChange.color;
        newAlpha.a = transparency;
        whatImageToChange.color = newAlpha;
    }
}
