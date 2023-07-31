using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public enum DataType { String, Int, Float };

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    { 
        AuthenticationService.Instance.SignedIn += LoadProfileInfo;
    }

    private async void InitializeProfileInfo()
    {
        //initialize data values
        SaveSomeData("ProfileXP", "0"); //start with 0 profile XP
        SaveSomeData("Energy", "30"); //start with 30 energy
        SaveSomeData("Gems", "10"); //start with 10 gems
        SaveSomeData("Coins", "0"); //start with 0 coins
    }

    private async void LoadProfileInfo()
    {
        ProfileInfo profileInfo = new ProfileInfo();
        profileInfo.profileName = await AuthenticationService.Instance.GetPlayerNameAsync();
        profileInfo.profileXP = await LoadSomeInt("ProfileXP");
        profileInfo.energy = await LoadSomeInt("Energy");
        profileInfo.gems = await LoadSomeInt("Gems");
        profileInfo.coins = await LoadSomeInt("Coins");

        ProfileManager.Instance.ProfileInfo = profileInfo;
        ProfileManager.Instance.UpdateTopBarUI();
    }

    private async Task<string> LoadSomeString(string key)
    {
        Dictionary<string, string> savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string>{key});
        return savedData[key];
    }

    private async Task<int> LoadSomeInt(string key)
    {
        try
        {
            Dictionary<string, string> savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string>{key});
            return int.Parse(savedData[key]);
        }
        catch(KeyNotFoundException ex)
        {
            //Debug.LogException(ex);
            return 0;
        }
    }

    public async void SaveSomeData(string key, string data)
    {
        var savedData = new Dictionary<string, object>{{key, data}};
        await CloudSaveService.Instance.Data.ForceSaveAsync(savedData);
    }
}
