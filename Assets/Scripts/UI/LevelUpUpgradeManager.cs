using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelUpUpgradeManager : MonoBehaviour
{
    [SerializeField] private GameObject levelUpButton;
    [SerializeField] private Transform levelUpButtonParent;
    private WeaponsList _weaponsList;
    private List<Button> _buttons = new();
    private List<GameObject> _weaponsLists = new ();
    [SerializeField] private List<int> chosenIndexes = new List<int>(3);
    public static event Action<SelectedWeapon> UpgradePlayer;
    public static event Action UpgradeEnded;
    
    private void OnEnable()
    {
        _weaponsList = Resources.Load<WeaponsList>("Weapons/WeaponsList");
        foreach (var weapon in _weaponsList.weaponList)
        {
            _weaponsLists.Add(weapon);
        }
        
        for (var i = 0; i < 3; i++)
        {
            ChooseUpgrade();
        }
        
        foreach (var button in _buttons)
        {
            if(!button) return;
            button.transform.TryGetComponent(out LevelUpLoader levelUpLoader);
            button.onClick.AddListener(() =>ButtonClicked(levelUpLoader.selectedWeapon));
        }
    }
    
    private void OnDisable()
    {
        var buttonListCopy = new Button[3];
        _buttons.CopyTo(buttonListCopy);
        foreach (var button in buttonListCopy)
        {
            if(!button) return;
            button.transform.TryGetComponent(out LevelUpLoader levelUpLoader);
            button.onClick.RemoveListener(() =>ButtonClicked(levelUpLoader.selectedWeapon));
            _buttons.Remove(button);
            Destroy(button.transform.gameObject);
        }
        
        _weaponsLists.Clear();
        chosenIndexes.Clear();
    }

    private void ButtonClicked(SelectedWeapon selectedWeapon)
    {
        UpgradePlayer?.Invoke(selectedWeapon);
        UpgradeEnded?.Invoke();
    }

    private void ChooseUpgrade()
    {
        var upgrade = RandomList();
        _weaponsLists.Remove(upgrade);
        upgrade.TryGetComponent(out SelectedWeapon selectedWeapon);
       
        var go = Instantiate(levelUpButton, levelUpButtonParent, true);
        go.TryGetComponent(out Button button);
        _buttons.Add(button);
        go.TryGetComponent(out LevelUpLoader levelUpLoader);
        levelUpLoader.selectedWeapon = selectedWeapon;
        levelUpLoader.level = WeaponManager.Instance.LevelOfLocalWeapon(selectedWeapon);
        Debug.Log(levelUpLoader.level);
        levelUpLoader.LoadUpgrade();
    }

    private GameObject RandomList()
    {
        var i = GetRandomNumber();
       
        foreach (var index in chosenIndexes)
        {
            while (i == index)
            {
                i = GetRandomNumber();
            }
        }
        
        chosenIndexes.Add(i);
        return _weaponsLists[i];
    }

    private int GetRandomNumber()
    {
        return Random.Range(0, _weaponsLists.Count);
    }
}
