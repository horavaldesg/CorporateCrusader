using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedWeapon : MonoBehaviour
{
    public enum Attributes
    {
        Puncture,
        Blunt,
        Chemical,
    }

    public Attributes attribute;

    public float damage;
    public float level;
}
