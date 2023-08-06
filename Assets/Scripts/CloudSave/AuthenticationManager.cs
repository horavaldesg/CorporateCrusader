using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;

#if UNITY_ANDROID
    using GooglePlayGames;
    using GooglePlayGames.BasicApi;
#endif

using AppleAuth;
using AppleAuth.Native;
using AppleAuth.Interfaces;
using AppleAuth.Enums;
using AppleAuth.Extensions;

public class AuthenticationManager : MonoBehaviour
{
    public static AuthenticationManager Instance;

    [SerializeField] private MainMenuManager mainMenuManager;
    [SerializeField] private bool deleteSessionToken = false;
    private const string AppleUserIdKey = "AppleUser";

    [Header("UI References")]
    [SerializeField] private Button googlePlayLoginButton;
    [SerializeField] private Button appleIDLoginButton;
    [SerializeField] private Button guestLoginButton;

    private IAppleAuthManager _appleAuthManager;

    private async void Awake()
    {
        Instance = this;

        await UnityServices.InitializeAsync();
    }

    private void Start()
    {
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {

            // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
            var deserializer = new PayloadDeserializer();
            // Creates an Apple Authentication manager with the deserializer
            this._appleAuthManager = new AppleAuthManager(deserializer);    
        }
        
        InitializeLoginMenu();
        /*
        //setup sign-in event
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In Successfully...");
            // Shows how to get a playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
        };
        */

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
        if(_appleAuthManager != null) _appleAuthManager.Update();
    }
    
    private void InitializeLoginMenu()
    {
        // Check if the current platform supports Sign In With Apple
        if (this._appleAuthManager == null)
        {
            appleIDLoginButton.interactable = false;
            return;
        }
        
        // If at any point we receive a credentials revoked notification, we delete the stored User ID, and go back to login
        this._appleAuthManager.SetCredentialsRevokedCallback(result =>
        {
            Debug.Log("Received revoked callback " + result);
            //this.SetupLoginMenuForSignInWithApple();
            PlayerPrefs.DeleteKey(AppleUserIdKey);
        });

        // If we have an Apple User Id available, get the credential status for it
        if (PlayerPrefs.HasKey(AppleUserIdKey))
        {
            var storedAppleUserId = PlayerPrefs.GetString(AppleUserIdKey);
            this.CheckCredentialStatusForUserId(storedAppleUserId);
        }
        // If we do not have an stored Apple User Id, attempt a quick login
        else
        {
            this.AttemptQuickLogin();
        }
    }
    
    private void CheckCredentialStatusForUserId(string appleUserId)
    {
        // If there is an apple ID available, we should check the credential state
        this._appleAuthManager.GetCredentialState(
            appleUserId,
            state =>
            {
                switch (state)
                {
                    // If it's authorized, login with that user id
                    case CredentialState.Authorized:
                        mainMenuManager.SplashScreenToStageSelect(); //skip login screen
                        return;
                    
                    // If it was revoked, or not found, we need a new sign in with apple attempt
                    // Discard previous apple user id
                    case CredentialState.Revoked:
                    case CredentialState.NotFound:
                       // this.SetupLoginMenuForSignInWithApple();
                        PlayerPrefs.DeleteKey(AppleUserIdKey);
                        return;
                }
            },
            error =>
            {
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                Debug.LogWarning("Error while trying to get credential state " + authorizationErrorCode.ToString() + " " + error.ToString());
                mainMenuManager.SplashScreenToLoginScreen(); //go to login screen
            });
    }
    
    private void AttemptQuickLogin()
    {
        var quickLoginArgs = new AppleAuthQuickLoginArgs();
        
        // Quick login should succeed if the credential was authorized before and not revoked
        this._appleAuthManager.QuickLogin(
            quickLoginArgs,
            credential =>
            {
                // If it's an Apple credential, save the user ID, for later logins
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    PlayerPrefs.SetString(AppleUserIdKey, credential.User);    
                }
            },
            error =>
            {
                // If Quick Login fails, we should show the normal sign in with apple menu, to allow for a normal Sign In with apple
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                Debug.LogWarning("Quick Login Failed " + authorizationErrorCode.ToString() + " " + error.ToString());
            });
    }
    
    public void SignInWithAppleButtonPressed()
    {
        this.SignInWithApple();
    }
    
    private void SignInWithApple()
    {
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
        
        this._appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                // If a sign in with apple succeeds, we should have obtained the credential with the user id, name, and email, save it
                PlayerPrefs.SetString(AppleUserIdKey, credential.User);
               
            },
            error =>
            {
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                Debug.LogWarning("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
            });
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
            //InitializeLoginScreen(); //initialize login screen
            mainMenuManager.SplashScreenToLoginScreen(); //go to login screen
        }
    }

    public async void SignInAsGuestAsync()
    {
        guestLoginButton.enabled = false;
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
            guestLoginButton.enabled = true;
        }
        catch (RequestFailedException ex)
        {
            //compare error code to CommonErrorCodes
            //notify the player with the proper error message
            Debug.LogException(ex);
            guestLoginButton.enabled = true;
        }
    }
/*
    private void InitializeLoginScreen()
    {
#if UNITY_EDITOR
        return; //keep- login buttons disabled if in editor
#endif

#if UNITY_ANDROID
        //if on Android, update interactability of login button
        googlePlayLoginButton.interactable = true;
#endif
        
        //if on IOS, update interactability of login button
        if(AppleAuthManager.IsCurrentPlatformSupported) appleIDLoginButton.interactable = true;
    }

    private void InitializeAppleAuthManager()
    {
        //initialize Apple authentication manager
        var deserializer = new PayloadDeserializer();
        _appleAuthManager = new AppleAuthManager(deserializer);
    }


#if UNITY_ANDROID
    public void GooglePlayLoginButton()
    {
        googlePlayLoginButton.enabled = false;
        string authCode = LoginWithGooglePlay();
        SignInWithGooglePlayAsync(authCode);
    }
#endif

    public void AppleIDLoginButton()
    {
        appleIDLoginButton.enabled = false;
        string idToken = LoginWithAppleID();
        SignInWithAppleAsync(idToken);
    }

#if UNITY_ANDROID
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
            else 
            {
                Debug.Log("Login Unsuccessful - Failed to retrieve Google play games authorization code");
                googlePlayLoginButton.enabled = true;
            }
        });
        return authCode;
    }
#endif

    public string LoginWithAppleID()
    {
        //initialize Apple authentication manager if necessary
        if(_appleAuthManager == null) InitializeAppleAuthManager();

        //set the login arguments
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

        //perform login
        string idToken = null;
        _appleAuthManager.LoginWithAppleId(
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
                else
                {
                    Debug.Log("Sign-in with Apple error. Message: appleIDCredential is null");
                    appleIDLoginButton.enabled = true;
                }
            },
            error => 
            { 
                Debug.Log("Sign-in with Apple error. Message: " + error);
                appleIDLoginButton.enabled = true;
            }
        );
        return idToken;
    }

#if UNITY_ANDROID
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
            googlePlayLoginButton.enabled = true;
        }
        catch (RequestFailedException ex)
        {
            //compare error code to CommonErrorCodes
            //notify the player with the proper error message
            Debug.LogException(ex);
            googlePlayLoginButton.enabled = true;
        }
    }
#endif

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
            appleIDLoginButton.enabled = true;
        }
        catch (RequestFailedException ex)
        {
            //compare error code to CommonErrorCodes
            //notify the player with the proper error message
            Debug.LogException(ex);
            appleIDLoginButton.enabled = true;
        }
    }
    */
}
