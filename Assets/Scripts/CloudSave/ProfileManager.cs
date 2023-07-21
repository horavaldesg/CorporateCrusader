using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;
using AppleAuth;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] private GameObject profileScreenBG;

    [Header("Top Bar UI References")]
    [SerializeField] private TMP_Text profileNameText_TB;

    [Header("Profile Screen UI References")]
    [SerializeField] private TMP_InputField profileNameInputField;
    [SerializeField] private Button editProfileNameButton;
    [SerializeField] private Button linkGooglePlayButton;
    [SerializeField] private Button linkAppleIDButton;

    private string profileName;

    private void Start()
    {
        AuthenticationService.Instance.SignedIn += InitializeTopBarUI;
    }

    private async void InitializeTopBarUI()
    {
        //get profile name
        profileName = await AuthenticationService.Instance.GetPlayerNameAsync();
        
        //set profile name text
        profileNameText_TB.text = profileName;
    }

    public void ToggleProfileScreen()
    {
        profileScreenBG.SetActive(!profileScreenBG.activeSelf);
        if(profileScreenBG.activeSelf) UpdateProfileScreen();
    }

    public void EditProfileNameButton()
    {
        //get rid of # and numbers if they exist
        string[] s = profileNameInputField.text.Split('#');
        profileNameInputField.text = s[0];

        //update UI interactability
        profileNameInputField.interactable = true;
        profileNameInputField.Select();
        editProfileNameButton.interactable = false;
    }

    public async void EndProfileNameEdit(string name)
    {
        //update UI interactability
        profileNameInputField.interactable = false;
        editProfileNameButton.interactable = true;

        //update and set profile name if given a new name
        if(name != profileName)
        {
            profileName = await AuthenticationService.Instance.UpdatePlayerNameAsync(name);
            profileNameInputField.text = profileName;
            profileNameText_TB.text = profileName;
        }
    }

    public void LinkAppleIDButton()
    {
        string idToken = AuthenticationManager.Instance.LoginWithAppleID();
        LinkWithAppleAsync(idToken);
    }

    private async void UpdateProfileScreen()
    {
        profileNameInputField.text = profileName; //set profile name text

        //get player info to check if account is linked and enable/disable link account buttons
        PlayerInfo info = await AuthenticationService.Instance.GetPlayerInfoAsync();

        if(AppleAuthManager.IsCurrentPlatformSupported && info.GetAppleId() == null) linkAppleIDButton.interactable = true;
    }

    private async void LinkWithAppleAsync(string idToken)
    {
        try
        {
            //link with Apple ID and disable link button afterwards
            await AuthenticationService.Instance.LinkWithAppleAsync(idToken);
            linkAppleIDButton.interactable = false;
        }
        catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            //prompt the player with an error message
            Debug.LogError("This user is already linked with another account. Log in instead.");
        }
        catch (AuthenticationException ex)
        {
            //compare error code to AuthenticationErrorCodes
            //notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            //compare error code to CommonErrorCodes
            //notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
}
