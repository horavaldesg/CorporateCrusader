using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Unity.Services.PlayerAccounts.Samples
{
    class PlayerAccountsDemo : MonoBehaviour
    {
        [SerializeField] Text m_AccessTokenText;
        
        [SerializeField] Text m_StatusText;
        
        [SerializeField] GameObject m_TokenPanel;
        
        [SerializeField] Button m_LoginButton;

        [SerializeField] private TMP_InputField playerName;
        
        async void Awake()
        {
            await UnityServices.InitializeAsync();
            if(PlayerAccountService.Instance.IsSignedIn)
                Play();
            PlayerAccountService.Instance.SignedIn += SignInWithUnity;
            PlayerAccountService.Instance.SignedIn +=  UpdateUI;
        }

        private void Start()
        {
            
        }
        
        async void SignInWithUnity()
        {
            try
            {
                await SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
                Debug.Log("SignIn is successful.");
                var playerId = PlayerAccountService.Instance.IdToken;
                Debug.Log(playerId);
                playerId = playerId.Substring(0, 5);
                PlayerName = playerId;
               
                await AuthenticationService.Instance.UpdatePlayerNameAsync(PlayerName);
                Debug.Log(PlayerName);
                UpdatePlayerName();
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
               // Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
               // Debug.LogException(ex);
            }
            
            Debug.Log(AuthenticationService.Instance.GetPlayerNameAsync());
        }
        
        async Task SignInWithUnityAsync(string accessToken)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
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

        public async void StartSignInAsync()
        {
            await PlayerAccountService.Instance.StartSignInAsync();
        }

        public async void RefreshToken()
        {
            await PlayerAccountService.Instance.RefreshTokenAsync();
            UpdateUI();
        }

        public void SignOut()
        {
            PlayerAccountService.Instance.SignOut();
            m_TokenPanel.SetActive(false);
            m_LoginButton.interactable = !m_LoginButton.interactable;
            m_StatusText.text = "";
        }

        public void OpenAccountPortal()
        {
            Application.OpenURL(PlayerAccountSettings.AccountPortalUrl);
        }

        void UpdateUI()
        {
            m_TokenPanel.SetActive(true);
            m_LoginButton.interactable = false;
            m_AccessTokenText.text = "<b>Access Token :</b> \n" + PlayerAccountService.Instance.AccessToken + "\n";
            m_StatusText.text = "<b>Request Successful!</b>";
        }

        void UpdatePlayerName()
        {
            playerName.placeholder.GetComponent<TextMeshProUGUI>().SetText( PlayerName);
        }

        public void Play()
        {
            AuthenticationService.Instance.UpdatePlayerNameAsync(PlayerName);
            SceneManager.LoadScene("Programming Playground");
        }

        public string PlayerName
        {
            get;
            set;
        }
    }
}
