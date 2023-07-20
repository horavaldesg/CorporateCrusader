using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class AuthenticationManager : MonoBehaviour
{
    public static AuthenticationManager Instance;

    [SerializeField] private MainMenuManager mainMenuManager;
    [SerializeField] private bool deleteSessionToken = false;

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

    public void SplashScreenButton()
    {
        //sign in cached player if session token exists
        if(AuthenticationService.Instance.SessionTokenExists)
        {
            SignInCachedUserAsync();
            mainMenuManager.SplashScreenToStageSelect(); //skip login screen
        }
        else mainMenuManager.SplashScreenToLoginScreen(); //go to login screen
    }

    public async void SignInAsGuestAsync()
    {
        //sign in anonymously as guest
        try
        { 
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            mainMenuManager.LoginScreenToStageSelect();
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
}
