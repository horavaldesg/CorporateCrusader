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

    //Multiple damage stats for various attacks
    public float damage1;
    public float damage2;
    public float damage3;
    public float damage4;
    public float damage5;

    //Different attacks have various cooldowns
    public float cooldown1;
    public float cooldown2;
    public float cooldown3;
    public float cooldown4;
    public float cooldown5;

    public float movespeed;
}
