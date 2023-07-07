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
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!AuthenticationService.Instance.IsSignedIn) return;
        playerName.placeholder.GetComponent<TextMeshProUGUI>().SetText(AuthenticationService.Instance.PlayerName);
        LoadSomeData();
    }

    public async void LoadSomeData()
    {
        Dictionary<string, string> savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string>{"Gold"});

        int.TryParse(savedData["Gold"], out var kills);
        Debug.Log("Done: " + kills);
    }

    public async Task SaveData()
    {
        var data = new Dictionary<string, object>{ { "Gold", "100" } };
        await CloudSaveService.Instance.Data.ForceSaveAsync(data);
        LoadSomeData();
    }
}