using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Authentication;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] private TMP_Text profileNameText_TopBar;

    private void Start()
    {
        AuthenticationService.Instance.SignedIn += InitializeTopBarUI;
    }

    private async void InitializeTopBarUI()
    {
        //get profile name
        string profileName = await AuthenticationService.Instance.GetPlayerNameAsync();
        
        //set profile name text
        profileNameText_TopBar.text = profileName;
    }
}
