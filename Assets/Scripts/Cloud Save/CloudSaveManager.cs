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
        if (AuthenticationService.Instance.IsSignedIn)
            playerName.placeholder.GetComponent<TextMeshProUGUI>().SetText(AuthenticationService.Instance.PlayerName);
    }

    public async Task SaveData()
    {
        var data = new Dictionary<string, object>{ { "MySaveKey", "HelloWorld" } };
        await CloudSaveService.Instance.Data.ForceSaveAsync(data);
    }
}