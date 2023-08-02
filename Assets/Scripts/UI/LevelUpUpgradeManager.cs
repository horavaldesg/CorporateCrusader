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
    private List<GameObject> _weaponsLists = new();
    private List<GameObject> _equipmentList = new();
    private Dictionary<string, int> equipmentAdded = new Dictionary<string, int>();
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

        foreach (var weapon in _weaponsList.equipmentList)
        {
            _equipmentList.Add(weapon);
            equipmentAdded.TryAdd(weapon.name, 0);
            if (equipmentAdded[weapon.name] == 5)
            {
                _equipmentList.Remove(weapon);
            }
        }

        for (var i = 0; i < 2; i++)
        {
            ChooseWeaponUpgrade();
        }

        ChooseEquipmentUpgrade();

        foreach (var button in _buttons)
        {
            if (!button) return;
            button.transform.TryGetComponent(out LevelUpLoader levelUpLoader);
            if (levelUpLoader.isEquipment)
            {
                button.onClick.AddListener(() => EquipmentClicked(levelUpLoader.selectedEquipment));
            }
            else
            {
                button.onClick.AddListener(() => ButtonClicked(levelUpLoader.selectedWeapon));
            }
        }
    }

    private void OnDisable()
    {
        var buttonListCopy = new Button[3];
        _buttons.CopyTo(buttonListCopy);
        foreach (var button in buttonListCopy)
        {
            if (!button) return;
            button.transform.TryGetComponent(out LevelUpLoader levelUpLoader);
            if (levelUpLoader.isEquipment)
            {
                button.onClick.RemoveListener(() => EquipmentClicked(levelUpLoader.selectedEquipment));
            }
            else
            {
                button.onClick.RemoveListener(() => ButtonClicked(levelUpLoader.selectedWeapon));
            }

            _buttons.Remove(button);
            Destroy(button.transform.gameObject);
        }
        
        _equipmentList.Clear();
        _weaponsLists.Clear();
        chosenIndexes.Clear();
    }

    private void ButtonClicked(SelectedWeapon selectedWeapon)
    {
        UpgradePlayer?.Invoke(selectedWeapon);
        UpgradeEnded?.Invoke();
    }

    private void EquipmentClicked(Equipment equipment)
    {
        var level = equipmentAdded[equipment.name];
        level += 1;
        EquipmentLevel = Mathf.Clamp(level, 0, 5);
        equipmentAdded[equipment.name] = EquipmentLevel;
        equipment.AffectPlayer(EquipmentLevel);
       
        UpgradeEnded?.Invoke();
    }

    private void ChooseWeaponUpgrade()
    {
        var upgrade = RandomList(_weaponsLists);
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

    private void ChooseEquipmentUpgrade()
    {
        if(_equipmentList.Count == 0) return;
        var i = GetRandomNumber(_equipmentList);
        SelectedEquipment = i;
        
        var upgrade = _equipmentList[i];
        
        upgrade.TryGetComponent(out Equipment equipment);
        EquipmentLevel = equipmentAdded[equipment.name];

        var go = Instantiate(levelUpButton, levelUpButtonParent, true);
        go.TryGetComponent(out Button button);
        _buttons.Add(button);
        go.TryGetComponent(out LevelUpLoader levelUpLoader);
        levelUpLoader.selectedEquipment = equipment;
        levelUpLoader.level = EquipmentLevel;
        levelUpLoader.isEquipment = true;
        levelUpLoader.LoadEquipmentUpgrade();
    }

    private int SelectedEquipment
    {
        get;
        set;
    }
    
    private int EquipmentLevel
    {
        get;
        set;
    }

    private GameObject RandomList(List<GameObject> list)
    {
        var i = GetRandomNumber(list);

        foreach (var index in chosenIndexes)
        {
            while (i == index)
            {
                i = GetRandomNumber(list);
            }
        }

        chosenIndexes.Add(i);
        return _weaponsLists[i];
    }

    private int GetRandomNumber(List<GameObject> list)
    {
        return Random.Range(0, list.Count);
    }
}
