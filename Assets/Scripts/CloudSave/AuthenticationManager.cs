using System.Text;
using System.Threading.Tasks;
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
    private const string AppleUserIdKey = "AppleUserLogin";

    [Header("UI References")]
    [SerializeField] private Button googlePlayLoginButton;
    [SerializeField] private Button appleIDLoginButton;
    [SerializeField] private Button guestLoginButton;

    private IAppleAuthManager _appleAuthManager;
    public string Token { get; private set; }
    public string Error { get; private set; }

    public void Initialize()
    {
        var deserializer = new PayloadDeserializer();
        _appleAuthManager = new AppleAuthManager(deserializer);
    }
    
    private async void Awake()
    {
        Instance = this;

        await UnityServices.InitializeAsync();
    }

    private void Start()
    {
        #if UNITY_IOS
        appleIDLoginButton.interactable = true;

        #endif
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
    
    
    public void SignInWithAppleButtonPressed()
    {
        // Initialize the Apple Auth Manager
        if (_appleAuthManager == null)
        {
            Initialize();
        }

        // Set the login arguments
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

        // Perform the login
        _appleAuthManager?.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                var appleIDCredential = credential as IAppleIDCredential;
                if (appleIDCredential != null)
                {
                    var idToken = Encoding.UTF8.GetString(
                        appleIDCredential.IdentityToken,
                        0,
                        appleIDCredential.IdentityToken.Length);
                    Debug.Log("Sign-in with Apple successfully done. IDToken: " + idToken);
                    Token = idToken;
                    SignInWithApple(Token);
                    SetPlayerName(appleIDCredential);
                    mainMenuManager.LoginScreenToStageSelect();
                }
                else
                {
                    Debug.Log("Sign-in with Apple error. Message: appleIDCredential is null");
                    Error = "Retrieving Apple Id Token failed.";
                }
            },
            error =>
            {
                Debug.Log("Sign-in with Apple error. Message: " + error);
                Error = "Retrieving Apple Id Token failed.";
            }
        );
    }

    private async void SignInWithApple(string idToken)
    {
        await SignInWithAppleAsync(idToken);
    }
    
    private async void SetPlayerName(IAppleIDCredential appleIDCredential)
    {
        await SetPlayerNameAsync(appleIDCredential);
    }
    
    private async Task SignInWithAppleAsync(string idToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithAppleAsync(idToken);
            Debug.Log("SignIn is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
    
    private async Task SetPlayerNameAsync(IAppleIDCredential appleIDCredential)
    {
        await AuthenticationService.Instance.UpdatePlayerNameAsync(appleIDCredential.FullName.GivenName);
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

    #region OldShit

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
    #endregion
}
