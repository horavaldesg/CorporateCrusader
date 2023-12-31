using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;
    private WeaponsList _weaponsList;
    [SerializeField] private List<Transform> weaponPositions = new ();
    [SerializeField] public List<SelectedWeapon> weaponsAdded = new ();
    [SerializeField] public List<Equipment> equipmentAdded = new ();
    [SerializeField] public List<GameObject> localWeapons = new ();
    [SerializeField] public List<GameObject> localEquipment = new ();

    private void Awake()
    {
        Instance = this;
        _weaponsList = Resources.Load<WeaponsList>("Weapons/WeaponsList");
    }
    
    private void OnEnable()
    {
        LevelUpUpgradeManager.UpgradePlayer += LoadWeaponUpgrade;
    }

    private void OnDisable()
    {
        LevelUpUpgradeManager.UpgradePlayer -= LoadWeaponUpgrade;
    }

    private void ChooseWeapon(GameObject weaponObj)
    {
        var weapon = Instantiate(weaponObj, transform, false);
        var chosenPos = weaponPositions[GetRandom(weaponPositions.Count)].position;
        weapon.TryGetComponent(out SelectedWeapon selectedWeapon);
        localWeapons.Add(weapon);
        UpgradeWeapon(selectedWeapon);
        //weapon.transform.position = new Vector3(chosenPos.x, chosenPos.y, 1);
    }

    public bool MaxWeaponLimitReached()
    {
        return weaponsAdded.Count == 6;
    }

    public bool MaxEquipmentLimitReached()
    {
        return equipmentAdded.Count == 6;
    }

    private void UpgradeWeapon(SelectedWeapon selectedWeapon)
    {
        //Upgrade Sequence
        foreach (var weapon in localWeapons)
        {
            weapon.TryGetComponent(out SelectedWeapon currentSelectedWeapon);
            if (selectedWeapon.weaponName == currentSelectedWeapon.weaponName)
            {
                // if(currentSelectedWeapon.level == 5) return;
                currentSelectedWeapon.UpgradeWeapon();
            }
        }
        
        LevelUpUpgradeManager.Instance.IncreaseLevelOfUpgradedWeapon(selectedWeapon);
    }

    private int GetRandom(int i)
    {
        return Random.Range(0, i);
    }
    
    private void LoadWeaponUpgrade(SelectedWeapon selectedWeapon)
    {
        foreach (var weapon in _weaponsList.weaponList)
        {
            weapon.TryGetComponent(out SelectedWeapon selectedWeaponFromList);
            if (selectedWeapon.weaponName != selectedWeaponFromList.weaponName) continue;
            if (WeaponInList(selectedWeaponFromList))
            {
                UpgradeWeapon(selectedWeaponFromList);
            }
            else
            {
                if(MaxWeaponLimitReached()) return; 
                ChooseWeapon(weapon);
                weaponsAdded.Add(selectedWeapon);
            }
        }
    }

    private bool WeaponInList(SelectedWeapon selectedWeapon)
    {
        return weaponsAdded.Any(weaponAdded => weaponAdded.weaponName == selectedWeapon.weaponName);
    }

    public void AddEquipment(Equipment equipment)
    {
        if(equipmentAdded.Contains(equipment)) return;
        equipmentAdded.Add(equipment);
        foreach (var localEquipment in _weaponsList.equipmentList)
        {
            localEquipment.TryGetComponent(out Equipment equipmentComp);
            if (equipmentComp.equipmentName == equipment.equipmentName)
            {
                this.localEquipment.Add(localEquipment);
            }
        }
    }

    public int LevelOfLocalWeapon(SelectedWeapon selectedWeapon)
    {
        var level = 0;
        foreach (var localWeapon in localWeapons)
        {
            localWeapon.TryGetComponent(out SelectedWeapon localSelectedWeapon);
            if (selectedWeapon.weaponName == localSelectedWeapon.weaponName)
            {
                level = localSelectedWeapon.level;
            }
        }

        return level;
    }

    public bool IsWeaponMaxLevel(SelectedWeapon selectedWeapon)
    {
        var level = 0;
        foreach (var localWeapon in localWeapons)
        {
            localWeapon.TryGetComponent(out SelectedWeapon localSelectedWeapon);
            if (selectedWeapon.weaponName == localSelectedWeapon.weaponName)
            {
                level = localSelectedWeapon.level;
            }
        }

        return level == 5;
    }

    public bool IsEquipmentMaxLevel(Equipment selectedEquipment)
    {
        var level = 0;
        foreach (var localWeapon in localEquipment)
        {
            localWeapon.TryGetComponent(out Equipment localSelectedEquipment);
            if (selectedEquipment.equipmentName == localSelectedEquipment.equipmentName)
            {
                level = localSelectedEquipment.level;
            }
        }

        return level == 5;
    }
    
    public bool WeaponCanEvolve(SelectedWeapon selectedWeapon)
    {
        var canEvolve = false;
        foreach (var localWeapon in localWeapons)
        {
            localWeapon.TryGetComponent(out SelectedWeapon localSelectedWeapon);
            if (selectedWeapon.weaponName != localSelectedWeapon.weaponName) continue;
            foreach (var _ in HatSelection.Instance.chosenHats.Where(chosenHat => chosenHat == localSelectedWeapon.hat))
            {
                canEvolve = true;
            }
        }

        return IsWeaponMaxLevel(selectedWeapon) && canEvolve;
    }
    
    public bool EquipmentCanEvolve(Equipment equipment)
    {
        var canEvolve = false;
        foreach (var localEquipment in localEquipment)
        {
            localEquipment.TryGetComponent(out Equipment localSelectedEquipment);
            if (equipment.equipmentName != localSelectedEquipment.equipmentName) continue;
            foreach (var _ in HatSelection.Instance.chosenHats.Where(chosenHat => chosenHat == localSelectedEquipment.hat))
            {
                canEvolve = true;
            }
        }

        //For Testing
       // canEvolve = true;
        return canEvolve && IsEquipmentMaxLevel(equipment) ;
    }
    
    public bool WeaponEvolved(SelectedWeapon selectedWeapon)
    {
        var weaponEvolved = false;
        foreach (var localWeapon in localWeapons)
        {
            localWeapon.TryGetComponent(out SelectedWeapon localSelectedWeapon);
            if (selectedWeapon.weaponName != localSelectedWeapon.weaponName) continue;
            weaponEvolved = localSelectedWeapon.WeaponEvolved;
        }

        return weaponEvolved;
    }
}
