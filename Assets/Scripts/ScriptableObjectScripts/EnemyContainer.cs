using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyContainer : ScriptableObject
{
    /*
    public GameObject[] basicMeleeEnemies;
    public GameObject[] fastMeleeEnemies;
    public GameObject[] tankMeleeEnemies;
    public GameObject[] rangedEnemies;
    public GameObject miniBoss;
    public GameObject mainBoss;
    */
    
    //Old System
    public GameObject[] phase1Enemies;
    public GameObject firstBoss;
    public GameObject[] phase2Enemies;
    public GameObject secondBoss;
    public GameObject[] phase3Enemies;
}
