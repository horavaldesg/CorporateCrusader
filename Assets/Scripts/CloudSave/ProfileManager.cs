using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;

#if UNITY_IOS
    using AppleAuth;
#endif

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance;

    [SerializeField] private MainMenuManager mainMenuManager;

    public ProfileInfo ProfileInfo;

    [Header("Profile XP Settings")]
    [SerializeField] private float xpReqIncreaseRate = 0.25f;

    [Header("Top Bar UI References")]
    [SerializeField] private TMP_Text profileNameText;
    [SerializeField] private Image profileXPBar_TB;
    [SerializeField] private TMP_Text profileXPReqText_TB;
    [SerializeField] private TMP_Text profileLevelText_TB;
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private TMP_Text gemsText;
    [SerializeField] private TMP_Text coinsText;

    [Header("Profile Screen UI References")]
    [SerializeField] private TMP_InputField profileNameInputField;
    [SerializeField] private Button editProfileNameButton;
    [SerializeField] private CanvasGroup nameChangeErrorText;
    [SerializeField] private Image profileXPBar_PS;
    [SerializeField] private TMP_Text profileXPReqText_PS;
    [SerializeField] private TMP_Text profileLevelText_PS;
    [SerializeField] private Button linkGooglePlayButton;
    [SerializeField] private Button linkAppleIDButton;
    [SerializeField] private GameObject profileScreenBG;
    [SerializeField] private GameObject logoutWarningBG;

    private void Awake() => Instance = this;

    private void Start()
    {
        #if UNITY_IOS
        linkAppleIDButton.interactable = true;
        #endif
    }

    public void UpdateTopBarUI()
    {
        //set profile name
        profileNameText.text = ProfileInfo.profileName;
        
        //get profile level and set text
        int totalXP = ProfileInfo.profileXP;
        ProfileInfo.profileLevel = Mathf.FloorToInt((xpReqIncreaseRate * Mathf.Sqrt(totalXP)) + 1);
        profileLevelText_TB.text = "Lvl " + ProfileInfo.profileLevel;

        //get profile xp requirements and set text and xp bar
        int xpForCurrentLevel = (int) Mathf.Pow((ProfileInfo.profileLevel - 1) / xpReqIncreaseRate, 2);
        int xpForNextLevel = (int) Mathf.Pow(ProfileInfo.profileLevel / xpReqIncreaseRate, 2);
        int levelXP = totalXP - xpForCurrentLevel;
        int levelXPNeeded = xpForNextLevel - xpForCurrentLevel;
        profileXPReqText_TB.text = levelXP + "/" + levelXPNeeded;
        profileXPBar_TB.fillAmount = (float) levelXP / levelXPNeeded;

        //set energy, gems, and coins
        energyText.text = ProfileInfo.energy + "/30";
        gemsText.text = ProfileInfo.gems.ToString();
        coinsText.text = ProfileInfo.coins.ToString();
    }

    private async void UpdateProfileScreen()
    {
        //set profile name
        profileNameInputField.text = ProfileInfo.profileName;

        //get profile level and set text
        int totalXP = ProfileInfo.profileXP;
        ProfileInfo.profileLevel = Mathf.FloorToInt((xpReqIncreaseRate * Mathf.Sqrt(totalXP)) + 1);
        profileLevelText_PS.text = "Lvl " + ProfileInfo.profileLevel;

        //get profile xp requirements and set text and xp bar
        int xpForCurrentLevel = (int) Mathf.Pow((ProfileInfo.profileLevel - 1) / xpReqIncreaseRate, 2);
        int xpForNextLevel = (int) Mathf.Pow(ProfileInfo.profileLevel / xpReqIncreaseRate, 2);
        int levelXP = totalXP - xpForCurrentLevel;
        int levelXPNeeded = xpForNextLevel - xpForCurrentLevel;
        profileXPReqText_PS.text = levelXP + "/" + levelXPNeeded;
        profileXPBar_PS.fillAmount = (float) levelXP / levelXPNeeded;

#if UNITY_EDITOR
        return; //skip link buttons if in editor
#endif

        //get player info to check if account is linked to any platforms
        PlayerInfo info = await AuthenticationService.Instance.GetPlayerInfoAsync();

        //enable/disable link account buttons based on whether the player is on the correct platform and has previously signed in or not
#if UNITY_ANDROID
        if(info.GetGooglePlayGamesId() == null) linkGooglePlayButton.interactable = true;
#endif

#if UNITY_IOS
        if(info.GetAppleId() == null) linkAppleIDButton.interactable = true;
#endif
    }

    public void AddProfileXP(int amount)
    {
        int totalXP = ProfileInfo.profileXP + amount; //get and change amount of profile XP
        ProfileInfo.profileXP = totalXP; //set new profile XP value
        SaveManager.Instance.SaveSomeData("ProfileXP", totalXP.ToString()); //save new profile XP value
        UpdateTopBarUI(); //update top bar UI
    }

    public void ChangeNumEnergy(int amount)
    {
        int energy = ProfileInfo.energy + amount; //get and change amount of energy
        ProfileInfo.energy = energy; //set new energy value
        SaveManager.Instance.SaveSomeData("Energy", energy.ToString());
        energyText.text = energy + "/30";
    }

    public void ChangeNumGems(int amount)
    {
        int gems = ProfileInfo.gems + amount; //get and change amount of gems
        ProfileInfo.gems = gems; //set new gems value
        SaveManager.Instance.SaveSomeData("Gems", gems.ToString()); //save new gems value
        gemsText.text = gems.ToString(); //update top bar UI
    }

    public void ChangeNumCoins(int amount)
    {
        int coins = ProfileInfo.coins + amount; //get and change amount of coins
        ProfileInfo.coins = coins; //set new coins value
        SaveManager.Instance.SaveSomeData("Coins", coins.ToString()); //save new coins value
        coinsText.text = coins.ToString(); //update top bar UI
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
        if(name != ProfileInfo.profileName)
        {
            try { ProfileInfo.profileName = await AuthenticationService.Instance.UpdatePlayerNameAsync(name); }
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
            
            profileNameInputField.text = ProfileInfo.profileName;
            profileNameText.text = ProfileInfo.profileName;
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

#if UNITY_ANDROID
    public async void LinkGooglePlayButton()
    {
      //  string authCode = AuthenticationManager.Instance.LoginWithGooglePlay();
       // await LinkWithGooglePlayAsync(authCode);
    }
#endif

//#if UNITY_IOS
    public async void LinkAppleIDButton()
    {
        var idToken = AuthenticationManager.Instance.GetIDToken();
        await LinkWithAppleAsync(idToken);
    }
//#endif

    public void LogoutButton() => logoutWarningBG.SetActive(true);

    public void Logout()
    {
        logoutWarningBG.SetActive(false);
        ToggleProfileScreen();
        mainMenuManager.StageSelectToLoginScreen();
        AuthenticationService.Instance.SignOut(true);
        PlayerPrefs.SetInt("InitialEnergyGiven", -1);
    }

    public void CancelLogout() => logoutWarningBG.SetActive(false);

#if UNITY_ANDROID
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
#endif

//#if UNITY_IOS
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
//#endif
}
