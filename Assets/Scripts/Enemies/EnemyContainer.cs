using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyContainer : ScriptableObject
{
    public GameObject[] phase1Enemies;
    public GameObject firstBoss;
    public GameObject[] phase2Enemies;
    public GameObject secondBoss;
    public GameObject[] phase3Enemies;
}
