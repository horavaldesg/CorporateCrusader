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
    [SerializeField] private List<SelectedWeapon> weaponsAdded = new ();
    [SerializeField] private List<Equipment> equipmentAdded = new ();
    [SerializeField] private List<GameObject> localWeapons = new ();

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

    private void UpgradeWeapon(SelectedWeapon selectedWeapon)
    {
        //Upgrade Sequence
        foreach (var weapon in localWeapons)
        {
            weapon.TryGetComponent(out SelectedWeapon currentSelectedWeapon);
            if (selectedWeapon.weaponName == currentSelectedWeapon.weaponName)
            {
                if(currentSelectedWeapon.level == 5) return;
                currentSelectedWeapon.UpgradeWeapon();
            }
        }
    }

    private int GetRandom(int i)
    {
        return Random.Range(0, i);
    }

    private void LoadWeaponUpgrade(SelectedWeapon selectedWeapon)
    {
        if(weaponsAdded.Count == 6) return; 
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
    
    public int LevelOfLocalEquipment(Equipment selectedEquipment)
    {
        var level = 0;
        foreach (var localWeapon in localWeapons)
        {
            localWeapon.TryGetComponent(out Equipment equipment);
            if (selectedEquipment.equipmentName == equipment.equipmentName)
            {
                level = equipment.level;
            }
        }

        return level;
    }
}
