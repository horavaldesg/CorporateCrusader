using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RewardsManager : MonoBehaviour
{
    public static RewardsManager Instance;

    public int CoinsCollected = 0;
    public int ProfileXPEarned = 0;

    private bool isInitialized = false;

    private void Awake() => Instance = this;

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    //when scene loads, if initialized in main menu scene, give player the rewards
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) { if(isInitialized) GiveRewards(); }

    public void InitializeRewards()
    {
        isInitialized = true;
        DontDestroyOnLoad(gameObject); //make sure object persists to main menu
    }

    //give rewards to player before destroying this object
    private void GiveRewards()
    {
        ProfileManager.Instance.ChangeNumCoins(CoinsCollected);
        ProfileManager.Instance.AddProfileXP(ProfileXPEarned);
        Destroy(gameObject);
    }
}
