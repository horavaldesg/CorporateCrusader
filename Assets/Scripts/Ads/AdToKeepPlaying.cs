using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class AdToKeepPlaying : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private Button _watchAdButton;
    [SerializeField] private string _iOSAdUnitId = "Rewarded_iOS";
    [SerializeField] private string _androidAdUnitId = "Rewarded_Android";

    private string _adUnitId = null; //this will remain null for unsupported platforms
 
    private void Awake()
    {   
        //get the Ad Unit ID for the current platform
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif
    }

    //load ad to keep playing
    public void LoadAdToKeepPlaying()
    {
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    //once the ad loads, show it
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);
        if (adUnitId.Equals(_adUnitId))
        {
            _watchAdButton.interactable = false; //disable button
            Advertisement.Show(_adUnitId, this); //show ad
        }
    }

    //once ad is finished, 
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded - Ad Completed");
            UIManager.Instance.WatchedAd();
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }
 
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
}
