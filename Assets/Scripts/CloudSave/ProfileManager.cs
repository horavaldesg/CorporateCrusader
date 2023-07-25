using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;
using AppleAuth;
using GooglePlayGames.OurUtils;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] private GameObject profileScreenBG;

    [Header("Top Bar UI References")]
    [SerializeField] private TMP_Text profileNameText_TB;

    [Header("Profile Screen UI References")]
    [SerializeField] private TMP_InputField profileNameInputField;
    [SerializeField] private Button editProfileNameButton;
    [SerializeField] private CanvasGroup nameChangeErrorText;
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

    private async void UpdateProfileScreen()
    {
        profileNameInputField.text = profileName; //set profile name text

        //get player info to check if account is linked to any platforms
        PlayerInfo info = await AuthenticationService.Instance.GetPlayerInfoAsync();

        //enable/disable link account buttons based on whether the player is on the correct platform and has previously signed in or not
        if(PlatformUtils.Supported && info.GetGooglePlayGamesId() == null) linkGooglePlayButton.interactable = true;
        if(AppleAuthManager.IsCurrentPlatformSupported && info.GetAppleId() == null) linkAppleIDButton.interactable = true;
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
            try { profileName = await AuthenticationService.Instance.UpdatePlayerNameAsync(name); }
            catch (AuthenticationException ex)
            {
                //compare error code to AuthenticationErrorCodes
                //notify the player with the proper error message
                ShowNameChangeError();
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                //compare error code to CommonErrorCodes
                //notify the player with the proper error message
                ShowNameChangeError();
                Debug.LogException(ex);
            } 
            
            profileNameInputField.text = profileName;
            profileNameText_TB.text = profileName;
        }
    }

    private void ShowNameChangeError() => StartCoroutine(NameChangeError());

    private IEnumerator NameChangeError()
    {
        //set alpha to 0
        float a = 0;
        nameChangeErrorText.alpha = a;

        //increase alpha to 1
        while(a < 1)
        {
            a += Time.deltaTime * 0.9f;
            nameChangeErrorText.alpha = a;
            yield return null;
        }

        //set alpha to 1
        a = 1;
        nameChangeErrorText.alpha = a;

        //decrease alpha to 0
        while(a > 0)
        {
            a -= Time.deltaTime * 0.9f;
            nameChangeErrorText.alpha = a;
            yield return null;
        }
    }

    public async void LinkGooglePlayButton()
    {
        string authCode = AuthenticationManager.Instance.LoginWithGooglePlay();
        await LinkWithGooglePlayAsync(authCode);
    }

    public async void LinkAppleIDButton()
    {
        string idToken = AuthenticationManager.Instance.LoginWithAppleID();
        await LinkWithAppleAsync(idToken);
    }

    private async Task LinkWithGooglePlayAsync(string authCode)
    {
        try
        {
            //link with Google Play Games and disable link button afterwards
            await AuthenticationService.Instance.LinkWithGooglePlayGamesAsync(authCode);
            linkGooglePlayButton.interactable = false;
        }
        catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            //prompt the player with an error message.
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

    private async Task LinkWithAppleAsync(string idToken)
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
