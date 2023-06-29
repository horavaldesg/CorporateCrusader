using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
   public static EnemySpawner Instance;
   [SerializeField] private int amountOfEnemiesPerWave;
   [SerializeField] private float timeToSpawn;
   
   private EnemyContainer _enemyContainer;
   
   [SerializeField] private List<GameObject> _enemiesSpawnedList = new();
   private int _phaseIndex;
   private int _enemiesSpawned;
   private bool _enemiesSpawning;
   private float _t;
   
   private void Awake()
   {
      Instance = this;
      _enemyContainer = Resources.Load<EnemyContainer>("EnemyContainer/EnemyContainer");
   }

   private void Start()
   {
      _phaseIndex = 1;
   }

   private void Update()
   {
      _t += Time.deltaTime;
      if (!(_t > timeToSpawn)) return;
      if (_enemiesSpawned < amountOfEnemiesPerWave)
      {
         _enemiesSpawning = true;
         Spawn();
      }
      else
      {
         if (!WaveCheck()) return;
         _enemiesSpawned = 0;
         _t = 0;
      }
   }

   public void RemoveEnemyFromList(GameObject enemy)
   {
      if (enemy)
         _enemiesSpawnedList.Remove(enemy);
   }

   private bool WaveCheck()
   {
      return _enemiesSpawnedList.Count <= 5;
   }

   private void Spawn()
   {
      _t = 0;
      if (GameManager.Instance.ChangePhase())
      {
         _phaseIndex++;
      }
      
      var go = Instantiate(GetRandomEnemy());
      go.transform.position = GetRadius();
      _enemiesSpawnedList.Add(go);
      _enemiesSpawned++;
   }

   [CanBeNull]
   private GameObject GetRandomEnemy()
   {
      return _phaseIndex switch
      {
         1 => _enemyContainer.phase1Enemies[GetRandomRange(_enemyContainer.phase1Enemies.Length)],
         2 => _enemyContainer.phase2Enemies[GetRandomRange(_enemyContainer.phase2Enemies.Length)],
         3 => _enemyContainer.phase3Enemies[GetRandomRange(_enemyContainer.phase3Enemies.Length)],
         _ => null
      };
   }
   
   private int GetRandomRange(int i)
   {
      return Random.Range(0, i);
   }
   
   private Vector3 GetRadius()
   {
      var playerTransform = PlayerController.Instance.CurrentPlayerTransform();
      var playerPos = PlayerController.Instance.CurrentPlayerTransform().position;
      var radius = 7f;
      var randomPos = Random.insideUnitSphere * radius;
      randomPos += playerPos;
      randomPos.y = 0f;
    
      Vector3 direction = randomPos - playerPos;
      direction.Normalize();
    
      var dotProduct = Vector3.Dot(playerTransform.forward, direction);
      var dotProductAngle = Mathf.Acos(dotProduct / playerTransform.forward.magnitude * direction.magnitude);
    
      randomPos.x = Mathf.Cos(dotProductAngle) * radius + playerPos.x;
      randomPos.y = Mathf.Sin(dotProductAngle * (Random.value > 0.5f ? 1f : -1f)) * radius + playerPos.y;
      randomPos.z = playerPos.z;
      return randomPos;
   }
}
