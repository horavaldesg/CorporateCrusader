using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ZoneManager : ScriptableObject
{
    public enum ChosenZone
    {
        AlleyZone,
        BeachZone,
        ClinicZone,
        FarmZone,
        ForestZone,
        MouseZone,
        WarehouseZone,
        YardZone
    }

    public ChosenZone chosenZone;
}
