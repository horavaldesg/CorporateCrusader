using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using AppleAuth;
using AppleAuth.Native;
using AppleAuth.Interfaces;
using AppleAuth.Enums;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.OurUtils;

public class AuthenticationManager : MonoBehaviour
{
    public static AuthenticationManager Instance;

    [SerializeField] private MainMenuManager mainMenuManager;
    [SerializeField] private bool deleteSessionToken = false;

    [Header("UI References")]
    [SerializeField] private Button googlePlayLoginButton;
    [SerializeField] private Button appleIDLoginButton;

    private IAppleAuthManager appleAuthManager;

    private async void Awake()
    {
        Instance = this;

        await UnityServices.InitializeAsync();
    }

    private void Start()
    {
        //setup sign-in event
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In Successfully...");
            // Shows how to get a playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
        };

        //dev option to delete session token
        if(deleteSessionToken) AuthenticationService.Instance.ClearSessionToken();
    }

    private async void SignInCachedUserAsync()
    {
        //sign in cached user
        try { await AuthenticationService.Instance.SignInAnonymouslyAsync(); }
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

    private void Update()
    {
        //update Apple authentication manager if possible
        if(appleAuthManager != null) appleAuthManager.Update();
    }

    public void SplashScreenButton()
    {
        //sign in cached player if session token exists
        if(AuthenticationService.Instance.SessionTokenExists)
        {
            SignInCachedUserAsync();
            mainMenuManager.SplashScreenToStageSelect(); //skip login screen
        }
        else
        {
            InitializeLoginScreen(); //initialize login screen
            mainMenuManager.SplashScreenToLoginScreen(); //go to login screen
        }
    }

    public async void SignInAsGuestAsync()
    {
        try
        {
            //sign in anonymously as guest
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            mainMenuManager.LoginScreenToStageSelect();
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

    private void InitializeLoginScreen()
    {
        //check if on a Google Play supported platform and update interactability of login button
        //NOTE: FIX THIS EVENTUALLY

        //check if on an Apple supported platform and update interactability of login button
        if(AppleAuthManager.IsCurrentPlatformSupported) appleIDLoginButton.interactable = true;
        else googlePlayLoginButton.interactable = true;
    }

    private void InitializeAppleAuthManager()
    {
        //initialize Apple authentication manager
        var deserializer = new PayloadDeserializer();
        appleAuthManager = new AppleAuthManager(deserializer);
    }

    public void GooglePlayLoginButton()
    {
        string authCode = LoginWithGooglePlay();
        SignInWithGooglePlayAsync(authCode);
    }

    public void AppleIDLoginButton()
    {
        string idToken = LoginWithAppleID();
        SignInWithAppleAsync(idToken);
    }

    public string LoginWithGooglePlay()
    {
        //initialize Google Play Games platform
        PlayGamesPlatform.Activate();

        //perform login
        string authCode = null;
        PlayGamesPlatform.Instance.Authenticate((success) =>
        {
            if (success == SignInStatus.Success)
            {
                //if login successful, get authorization code to return
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code => { authCode = code; });
            }
            else Debug.Log("Login Unsuccessful - Failed to retrieve Google play games authorization code");
        });
        return authCode;
    }

    public string LoginWithAppleID()
    {
        //initialize Apple authentication manager if necessary
        if(appleAuthManager == null) InitializeAppleAuthManager();

        //set the login arguments
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

        //perform login
        string idToken = null;
        appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential => 
            {
                //get credential as an AppleIDCredential
                var appleIDCredential = credential as IAppleIDCredential;
                if(appleIDCredential != null)
                {
                    //if AppleIDCredential exists, get idToken and return it
                    idToken = Encoding.UTF8.GetString(
                        appleIDCredential.IdentityToken,
                        0,
                        appleIDCredential.IdentityToken.Length);
                }
                else Debug.Log("Sign-in with Apple error. Message: appleIDCredential is null");
            },
            error => { Debug.Log("Sign-in with Apple error. Message: " + error); }
        );
        return idToken;
    }

    private async void SignInWithGooglePlayAsync(string authCode)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
            mainMenuManager.LoginScreenToStageSelect();
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

    private async void SignInWithAppleAsync(string idToken)
    {
        try
        {
            //sign in using Apple ID
            await AuthenticationService.Instance.SignInWithAppleAsync(idToken);
            mainMenuManager.LoginScreenToStageSelect();
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
