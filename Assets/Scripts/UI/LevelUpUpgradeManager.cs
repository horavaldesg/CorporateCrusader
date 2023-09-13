using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelUpUpgradeManager : MonoBehaviour
{
    public static LevelUpUpgradeManager Instance;
    [SerializeField] private GameObject levelUpButton;
    [SerializeField] private Transform levelUpButtonParent;
    [SerializeField] private Sprite coinSprite;
    private WeaponsList _weaponsList;
    private List<Button> _buttons = new();
    [SerializeField]private List<GameObject> _weaponsLists = new();
    private List<GameObject> _equipmentList = new();
    private Dictionary<string, int> equipmentAdded = new Dictionary<string, int>();
    private Dictionary<string, object[]> weaponsAdded = new Dictionary<string, object[]>();
    [SerializeField] private List<int> chosenIndexes = new List<int>(3);
    [SerializeField] private List<int> chosenIndexesEquipment = new List<int>(3);
    public static event Action<SelectedWeapon> UpgradePlayer;
    public static event Action UpgradeEnded;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        _weaponsList = Resources.Load<WeaponsList>("Weapons/WeaponsList");
        LoadWeaponList(WeaponManager.Instance.MaxWeaponLimitReached()
            ? WeaponManager.Instance.localWeapons
            : _weaponsList.weaponList);

        LoadEquipmentList(WeaponManager.Instance.MaxEquipmentLimitReached()
            ? WeaponManager.Instance.localEquipment
            : _weaponsList.equipmentList);

        for (var i = 0; i < 3; i++)
        {
            var r = Random.Range(0, 11);
            (r % 2 == 0 ? (Action)ChooseEquipmentUpgrade : ChooseWeaponUpgrade)();
        }

        foreach (var button in _buttons)
        {
            if (!button) return;
            button.transform.TryGetComponent(out LevelUpLoader levelUpLoader);
            if (levelUpLoader.isGold)
            {
                button.onClick.AddListener(() => GoldClicked(levelUpLoader));
            }
            
            else if (levelUpLoader.isEquipment)
            {
                button.onClick.AddListener(() => EquipmentClicked(levelUpLoader.selectedEquipment));
            }
            else
            {
                button.onClick.AddListener(() => WeaponClicked(levelUpLoader.selectedWeapon));
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
            if (levelUpLoader.isEquipment && !levelUpLoader.isGold)
            {
                button.onClick.RemoveListener(() => EquipmentClicked(levelUpLoader.selectedEquipment));
            }
            else
            {
                button.onClick.RemoveListener(() => WeaponClicked(levelUpLoader.selectedWeapon));
            }
            
            if (levelUpLoader.isGold)
            {
                button.onClick.RemoveListener(() => GoldClicked(levelUpLoader));
            }

            _buttons.Remove(button);
            Destroy(button.transform.gameObject);
        }
        
        _equipmentList.Clear();
        _weaponsLists.Clear();
        chosenIndexes.Clear();
    }

    private void LoadWeaponList([NotNull] List<GameObject> weaponList)
    {
        foreach (var weapon in weaponList)
        {
            weapon.TryGetComponent(out SelectedWeapon selectedWeapon);
            _weaponsLists.Add(weapon);
            var weaponMaxLevel = WeaponManager.Instance.IsWeaponMaxLevel(selectedWeapon);
            var weaponEvolved = WeaponManager.Instance.WeaponEvolved(selectedWeapon);
            var weaponLevel = WeaponManager.Instance.LevelOfLocalWeapon(selectedWeapon);
            object[] weaponStats = { weaponLevel, 
                weaponMaxLevel, 
                weaponEvolved };
            weaponsAdded.TryAdd(selectedWeapon.weaponName, weaponStats);
            var weaponObj = weaponsAdded[selectedWeapon.weaponName];
           //var weaponLevel = weaponsAdded[selectedWeapon.name];
           // Debug.Log("Weapon Name that is Loaded: " + weapon.name + " Is Level: " + weaponLevel);
            if (weaponEvolved)
            {
                _weaponsLists.Remove(weapon);
                Debug.Log(weapon.name + " Has Been Removed from list");
            }
            else if(weaponLevel  == 5 && !WeaponManager.Instance.WeaponCanEvolve(selectedWeapon))
            {
                _weaponsLists.Remove(weapon);
                Debug.Log(weapon.name + " Has Been Removed from list");
            }
        }
    }

    private void LoadEquipmentList([NotNull] List<GameObject> equipmentList)
    {
        foreach (var equipment in equipmentList)
        {
            equipment.TryGetComponent(out Equipment equipmentComp);
            _equipmentList.Add(equipment);
            equipmentAdded.TryAdd(equipmentComp.name, 0);
            if (equipmentAdded[equipmentComp.name] == 5)
            {
                _equipmentList.Remove(equipment);
            }
        }
    }
    
    private void WeaponClicked(SelectedWeapon selectedWeapon)
    {
        UpgradePlayer?.Invoke(selectedWeapon);
    }

    public void IncreaseLevelOfUpgradedWeapon(SelectedWeapon selectedWeapon)
    {
        var weapon = weaponsAdded[selectedWeapon.weaponName];
        var weaponLevel = WeaponManager.Instance.LevelOfLocalWeapon(selectedWeapon);
        weapon[0] = weaponLevel;
        Debug.Log("Weapon Name: " + selectedWeapon.name + " Is Level: " + weaponLevel + " Can Evolve " + (bool)weapon[2]);
        UpgradeEnded?.Invoke();
    }

    private void EquipmentClicked(Equipment equipment)
    {
        var level = equipmentAdded[equipment.name];
        level += 1;
        EquipmentLevel = Mathf.Clamp(level, 0, 5);
        equipmentAdded[equipment.name] = EquipmentLevel;
        equipment.AffectPlayer(EquipmentLevel);
        WeaponManager.Instance.AddEquipment(equipment);
        UpgradeEnded?.Invoke();
    }

    private void GoldClicked([NotNull]LevelUpLoader levelUpLoader)
    {
        levelUpLoader.ClickGold();
        UpgradeEnded?.Invoke();
    }

    private void ChooseWeaponUpgrade()
    {
        switch (CanLoadCoins() || _weaponsLists.Count == 2)
        {
            case true:
                ChooseCoinUpgrade();
                break;
            case false:
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
                break;
        }
    }

    private void ChooseEquipmentUpgrade()
    {
       // var i = GetRandomNumber(_equipmentList);
       switch (CanLoadCoins() || _equipmentList.Count == 0)
       {
           case true:
               ChooseCoinUpgrade();
               break;
           case false:
               if(_equipmentList.Count == 0) return;
               var upgrade = RandomListEquipment(_equipmentList);
        
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
               break;
       }
    }

    private void ChooseCoinUpgrade()
    {
        var go = Instantiate(levelUpButton, levelUpButtonParent, true);
        go.TryGetComponent(out Button button);
        _buttons.Add(button);
        go.TryGetComponent(out LevelUpLoader levelUpLoader);
        levelUpLoader.LoadGold(coinSprite);
        levelUpLoader.isGold = true;
    }

    private bool CanLoadCoins()
    {
        return _equipmentList.Count + _weaponsLists.Count <= 2;
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

    private GameObject RandomListEquipment(List<GameObject> list)
    {
        var i = GetRandomNumber(list);

        foreach (var index in chosenIndexesEquipment)
        {
            while (i == index)
            {
                i = GetRandomNumber(list);
            }
        }

        chosenIndexesEquipment.Add(i);
        return _equipmentList[i];
    }

    private int GetRandomNumber(List<GameObject> list)
    {
        return Random.Range(0, list.Count);
    }
}
