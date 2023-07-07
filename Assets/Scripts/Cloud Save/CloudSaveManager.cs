using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using UnityEngine;
using UnityEngine.UI;

public class CloudSaveManager : MonoBehaviour
{
    public static CloudSaveManager Instance;
    [SerializeField] private TMP_InputField playerName;
    public static event Action<int> AddKills;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!AuthenticationService.Instance.IsSignedIn) return;
        playerName.placeholder.GetComponent<TextMeshProUGUI>().SetText(AuthenticationService.Instance.PlayerName);
        //LoadSomeData();
    }

    public async void LoadSomeData(string key)
    {
        Dictionary<string, string> savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string>{key});

        int.TryParse(savedData[key], out var kills);
        Debug.Log("Done: " + kills);
        AddKills?.Invoke(kills);
    }

    public async Task SaveData()
    {
        var data = new Dictionary<string, object>{ { "Kills", "100" } };
        await CloudSaveService.Instance.Data.ForceSaveAsync(data);
        LoadSomeData("Kills");
    }
    
    public async Task SaveKills()
    {
        var data = new Dictionary<string, object>{ { KillsKey, KillsAmount } };
        await CloudSaveService.Instance.Data.ForceSaveAsync(data);
        LoadSomeData(KillsKey); 
    }

    public void SetKills()
    {
        SaveKills();
    }

    public string KillsKey
    {
        get;
        set;
    }

    public string KillsAmount
    {
        get;
        set;
    }
}