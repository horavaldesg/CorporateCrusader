using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] private bool _testMode = true;

    private const string IOSGameId = "5323292";
    private const string AndroidGameId = "5323293";

    private string _gameId;

    private void Awake() => InitializeAds();

    private void InitializeAds()
    {
#if UNITY_IOS
        _gameId = IOSGameId;
#elif UNITY_ANDROID
        _gameId = AndroidGameId;
#endif

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}