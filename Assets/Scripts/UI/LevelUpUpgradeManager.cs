using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
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
    [SerializeField]private List<GameObject> weaponsLists = new();
    private List<GameObject> _equipmentList = new();
    private Dictionary<string, int> _equipmentAdded = new ();
    private Dictionary<string, object[]> _weaponsAdded = new ();
    [SerializeField] private List<int> chosenIndexes = new (3);
    [SerializeField] private List<int> chosenIndexesEquipment = new (3);
    public static event Action<SelectedWeapon> UpgradePlayer;
    public static event Action UpgradeEnded;
    private List<GameObject> _chosenUpgrades = new();
    private List<GameObject> luckyCoinWeapon = new();
    private List<GameObject> luckyCoinEquipment = new();

    private const string Equipment = "Equipment";
    private const string Weapon = "Weapon";
    
    public bool HasLuckCoin
    {
        get;
        set;
    }
    
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
        weaponsLists.Clear();
        chosenIndexes.Clear();
        _chosenUpgrades.Clear();
        luckyCoinWeapon.Clear();
    }

    //Loads Resources List into Local List
    private void LoadWeaponList([NotNull] List<GameObject> weaponList)
    {
        foreach (var weapon in weaponList)
        {
            weapon.TryGetComponent(out SelectedWeapon selectedWeapon);
            weaponsLists.Add(weapon);
            var weaponMaxLevel = WeaponManager.Instance.IsWeaponMaxLevel(selectedWeapon);
            var weaponEvolved = WeaponManager.Instance.WeaponEvolved(selectedWeapon);
            var weaponLevel = WeaponManager.Instance.LevelOfLocalWeapon(selectedWeapon);
            object[] weaponStats = { weaponLevel, 
                weaponMaxLevel, 
                weaponEvolved };
            _weaponsAdded.TryAdd(selectedWeapon.weaponName, weaponStats);
            var weaponObj = _weaponsAdded[selectedWeapon.weaponName];
           //var weaponLevel = weaponsAdded[selectedWeapon.name];
           // Debug.Log("Weapon Name that is Loaded: " + weapon.name + " Is Level: " + weaponLevel);
            if (weaponEvolved)
            {
                weaponsLists.Remove(weapon);
                Debug.Log(weapon.name + " Has Been Removed from list");
            }
            else if(weaponLevel  == 5 && !WeaponManager.Instance.WeaponCanEvolve(selectedWeapon))
            {
                weaponsLists.Remove(weapon);
                Debug.Log(weapon.name + " Has Been Removed from list");
            }
        }
    }
    
    //Loads Resources List into Local List
    private void LoadEquipmentList([NotNull] List<GameObject> equipmentList)
    {
        foreach (var equipment in equipmentList)
        {
            equipment.TryGetComponent(out Equipment equipmentComp);
            _equipmentList.Add(equipment);
            _equipmentAdded.TryAdd(equipmentComp.equipmentName, 0);
            if (_equipmentAdded[equipmentComp.equipmentName] == 5 && !WeaponManager.Instance.EquipmentCanEvolve(equipmentComp))
            {
                _equipmentList.Remove(equipment);
            }
        }
    }
    
    //Click Event
    private void WeaponClicked(SelectedWeapon selectedWeapon)
    {
        UpgradePlayer?.Invoke(selectedWeapon);
    }

    //Increases Level of Weapon when upgraded
    public void IncreaseLevelOfUpgradedWeapon(SelectedWeapon selectedWeapon)
    {
        var weapon = _weaponsAdded[selectedWeapon.weaponName];
        var weaponLevel = WeaponManager.Instance.LevelOfLocalWeapon(selectedWeapon);
        weapon[0] = weaponLevel;
        Debug.Log("Weapon Name: " + selectedWeapon.name + " Is Level: " + weaponLevel + " Can Evolve " + (bool)weapon[2]);
        UpgradeEnded?.Invoke();
    }

    //Click Event
    private void EquipmentClicked(Equipment equipment)
    {
        var level = _equipmentAdded[equipment.equipmentName];
        level += 1;
        EquipmentLevel = Mathf.Clamp(level, 0, 5);
        _equipmentAdded[equipment.equipmentName] = EquipmentLevel;
        equipment.AffectPlayer(EquipmentLevel);
        WeaponManager.Instance.AddEquipment(equipment);
        UpgradeEnded?.Invoke();
    }

    //Click Event
    private void GoldClicked([NotNull]LevelUpLoader levelUpLoader)
    {
        levelUpLoader.ClickGold();
        UpgradeEnded?.Invoke();
    }

    //Picks weapons to show on screen
    private void ChooseWeaponUpgrade()
    {
        switch (CanLoadCoins() || weaponsLists.Count == 2)
        {
            case true:
                ChooseCoinUpgrade();
                break;
            case false:
                var chooseFromLuckyCoin = LuckyCoinRandomizer();

                var upgrade = RandomList(weaponsLists);
                if (HasLuckCoin)
                {
                    upgrade = chooseFromLuckyCoin % 2 == 0 ? ChooseFromLuckyCoin(Weapon) : RandomList(weaponsLists);
                }
                
                if(!_chosenUpgrades.Contains(upgrade))_chosenUpgrades.Add(upgrade);
                weaponsLists.Remove(upgrade);
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
    
     //Picks Equipment to show on screen
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
               var chooseFromLuckyCoin = LuckyCoinRandomizer();
               
               var upgrade = RandomList(_equipmentList);
               if (HasLuckCoin)
               {
                   upgrade = chooseFromLuckyCoin % 2 == 0 ? ChooseFromLuckyCoin(Equipment) : RandomList(_equipmentList);
               }
               
                Debug.Log("Has Lucky Coin " + HasLuckCoin);
               //var upgrade = RandomListEquipment(listToChooseFrom);
        
               upgrade.TryGetComponent(out Equipment equipment);
               EquipmentLevel = _equipmentAdded[equipment.equipmentName];
               if(!_chosenUpgrades.Contains(upgrade))_chosenUpgrades.Add(upgrade);

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

    //Coins show on screen
    private void ChooseCoinUpgrade()
    {
        var go = Instantiate(levelUpButton, levelUpButtonParent, true);
        go.TryGetComponent(out Button button);
        _buttons.Add(button);
        go.TryGetComponent(out LevelUpLoader levelUpLoader);
        levelUpLoader.LoadGold(coinSprite);
        levelUpLoader.isGold = true;
    }


    [NotNull]
    private GameObject ChooseFromLuckyCoin(string whatToChoose)
    {
        var weaponsAddedFromWeaponManager = WeaponManager.Instance.weaponsAdded;
        var equipmentAddedFromWeaponManager = WeaponManager.Instance.equipmentAdded;

        var totalAdded = weaponsAddedFromWeaponManager.Count + equipmentAddedFromWeaponManager.Count;
        var listToChooseFrom = new List<GameObject>();
        
        switch (whatToChoose)
        {
            case Equipment:
                listToChooseFrom = _equipmentList;
                break;
            case Weapon:
                listToChooseFrom = weaponsLists;
                break;
        }
        
        if (totalAdded < 6)
        {
            return RandomList(listToChooseFrom);
        }
        
        listToChooseFrom = _equipmentList.Union(weaponsLists).ToList();
        Debug.Log(listToChooseFrom.Count);
        foreach (var items in listToChooseFrom.ToArray())
        {
            items.TryGetComponent(out SelectedWeapon sw);
            items.TryGetComponent(out Equipment equipment);
            if (sw)
            {
                Debug.Log(sw.weaponName + " Is Selected");
                if (weaponsAddedFromWeaponManager.Contains(sw))
                {
                    if(!luckyCoinWeapon.Contains(items)) luckyCoinWeapon.Add(items);
                    Debug.Log(sw.weaponName + "is now in list");
                }
            }
            else
            {
                if (equipmentAddedFromWeaponManager.Contains(equipment))
                {
                    if(equipment.equipmentName != "Lucky Coin")
                    {
                        if (!luckyCoinEquipment.Contains(items)) luckyCoinEquipment.Add(items);
                        Debug.Log(equipment.equipmentName + "is now in list");
                    }
                }
            }
        }

        var objectToReturn = new GameObject();
        switch (whatToChoose)
        {
            case Equipment:
            {
                objectToReturn = RandomList(luckyCoinEquipment);
                if (luckyCoinEquipment.Count < 1) RandomList(_equipmentList);
            }
                break;
            case Weapon:
            {
                objectToReturn = RandomList(luckyCoinWeapon);
                if (luckyCoinWeapon.Count < 1) RandomList(weaponsLists);
            }
                break;
        }

        return objectToReturn;
    }

    private int LuckyCoinRandomizer()
    {
        var luckCoinList = GameManager.Instance._luckCoinList;
        var returnVar = Random.Range(0, luckCoinList.Count);
        return luckCoinList[returnVar];
    }
        
    
    
    private bool CanLoadCoins()
    {
        return _equipmentList.Count + weaponsLists.Count <= 2;
    }
    
    
    private int EquipmentLevel
    {
        get;
        set;
    }

    private GameObject RandomList(List<GameObject> list)
    {
        foreach (var itemChosen in list.ToArray())
        {
            if (_chosenUpgrades.Contains(itemChosen))
            {
                list.Remove(itemChosen);
            }
        }

        var i = GetRandomNumber(list);
        
        return list[i];
    }
    

    private int GetRandomNumber(List<GameObject> list)
    {
        return Random.Range(0, list.Count- 1);
    }
}
