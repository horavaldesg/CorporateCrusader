using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyStats : ScriptableObject
{
    public enum Weakness
    {
        Puncture,
        Blunt,
        Chemical,
        None
    }

    public GameObject xpObject;
    public GameObject goldObject;
    
    public Weakness weakness;
    public float health;
    public float damage;
    public float speed;
    public float attackTime;
    public int xpToDrop;
    public int goldToDrop;
}
