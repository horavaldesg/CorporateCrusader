using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BossStats : ScriptableObject
{
    public enum Weakness
    {
        Puncture,
        Blunt,
        Chemical,
        None
    }

    public Weakness weakness;
    public float health;
    public float damage;
    public float cooldown;
    public float movespeed;
}
