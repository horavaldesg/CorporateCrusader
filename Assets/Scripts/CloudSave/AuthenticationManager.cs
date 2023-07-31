using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;

#if UNITY_ANDROID
    using GooglePlayGames;
    using GooglePlayGames.BasicApi;
#endif

#if UNITY_IOS
    using AppleAuth;
    using AppleAuth.Native;
    using AppleAuth.Interfaces;
    using AppleAuth.Enums;
#endif

public class AuthenticationManager : MonoBehaviour
{
    public static AuthenticationManager Instance;

    [SerializeField] private MainMenuManager mainMenuManager;
    [SerializeField] private bool deleteSessionToken = false;

    [Header("UI References")]
    [SerializeField] private Button googlePlayLoginButton;
    [SerializeField] private Button appleIDLoginButton;
    [SerializeField] private Button guestLoginButton;

#if UNITY_IOS
    private IAppleAuthManager appleAuthManager;
#endif

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
#if UNITY_IOS
        //update Apple authentication manager if possible
        if(appleAuthManager != null) appleAuthManager.Update();
#endif
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

    private void InitializeLoginScreen()
    {
#if UNITY_EDITOR
        return; //keep- login buttons disabled if in editor
#endif

#if UNITY_ANDROID
        //if on Android, update interactability of login button
        googlePlayLoginButton.interactable = true;
#endif
        
#if UNITY_IOS
        //if on IOS, update interactability of login button
        appleIDLoginButton.interactable = true;
#endif
    }

#if UNITY_IOS
    private void InitializeAppleAuthManager()
    {
        //initialize Apple authentication manager
        var deserializer = new PayloadDeserializer();
        appleAuthManager = new AppleAuthManager(deserializer);
    }
#endif

#if UNITY_ANDROID
    public void GooglePlayLoginButton()
    {
        googlePlayLoginButton.enabled = false;
        string authCode = LoginWithGooglePlay();
        SignInWithGooglePlayAsync(authCode);
    }
#endif

#if UNITY_IOS
    public void AppleIDLoginButton()
    {
        appleIDLoginButton.enabled = false;
        string idToken = LoginWithAppleID();
        SignInWithAppleAsync(idToken);
    }
#endif

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

#if UNITY_IOS
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
#endif

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

#if UNITY_IOS
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
#endif
}
