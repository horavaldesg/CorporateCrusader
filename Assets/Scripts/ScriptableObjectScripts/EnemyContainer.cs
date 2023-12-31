using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
    public EnemyHolder[] enemyHolder;
    public EnemyHolder[] frequencyEnemy;
    public EnemyHolder[] randomTriggerEnemy;
    public EnemyHolder[] playerLevelEnemy;
    public EnemyHolder[] mixedEnemies;
    
    //Old System
    public GameObject[] allEnemies;
    public GameObject[] phase1Enemies;
    public GameObject firstBoss;
    public GameObject[] phase2Enemies;
    public GameObject secondBoss;
    public GameObject[] phase3Enemies;
}
