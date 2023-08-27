using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
   public static EnemySpawner Instance;
   [SerializeField] private int amountOfEnemiesPerWave;
   [SerializeField] private float timeToSpawn;
   [SerializeField] private int clusterSpawn;
   
   [SerializeField] private GameObject bossBoundary;
   
   //Separate this by enemy type/zone/level
   private EnemyContainer _enemyContainer;
   
   private int _phaseIndex;
   private int _enemiesSpawned;
   private bool _enemiesSpawning;
   private float _t;
   private bool _enemiesLoaded;
   public bool spawnSpecificEnemy;
   public int enemyIndex;

   [SerializeField]private int _currentEnemyIndex;
   
   private bool BossPhase
   {
      get;
      set;
   }
   
   private void Awake()
   {
      Instance = this;
   }

   private void OnEnable()
   {
      //GameManager.ChangePhase += BossFight;
      Boss.OnBossKilled += ChangePhase;
      GameManager.EnemiesLoaded += StartSpawning;
      GameManager.EnemyIndexIncrease += IncreaseEnemyIndex;
   }

   private void OnDisable()
   {
      //GameManager.ChangePhase -= BossFight;
      Boss.OnBossKilled -= ChangePhase;
      GameManager.EnemiesLoaded -= StartSpawning;
      GameManager.EnemyIndexIncrease -= IncreaseEnemyIndex;
   }

   private void StartSpawning(EnemyContainer enemyContainer)
   {
      _enemyContainer = enemyContainer;
     
      if (spawnSpecificEnemy)
      {
         Spawn();
      }
      else
      {
          _enemiesLoaded = true;
      }
   }
   
   private void Start()
   {
      _phaseIndex = 1;
      _currentEnemyIndex = 0;
   }
   

   private void Update()
   {
      if(!_enemiesLoaded) return;
      if (BossPhase) return;
      LinearSpawn();
   }

   private void BossFight()
   {
      BossPhase = true;
      SpawnBossBoundary();
   }

   private void SpawnBossBoundary()
   {
      var boundary = Instantiate(bossBoundary);
      var playerPosition = PlayerController.Instance.CurrentPlayerTransform().position;
      
      boundary.transform.position = new Vector3(playerPosition.x, playerPosition.y + 1, 1);
      SpawnBoss();
   }

   private void SpawnBoss()
   {
      //Spawns Boss
      // var go = Instantiate()
      // go.TryGetComponent(Boss bossComp);
   }

   private void ChangePhase()
   {
      _phaseIndex++;
      BossPhase = false;
   }

   private void LinearSpawn()
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

   private void IncreaseEnemyIndex()
   {
      _currentEnemyIndex++;
   }
   
   public void RemoveEnemyFromList(GameObject enemy)
   {
      if (enemy)
         GameManager.Instance.enemiesSpawnedList.Remove(enemy);
   }

   private bool WaveCheck()
   {
      return GameManager.Instance.enemiesSpawnedList.Count <= 5; // change to percentage
   }

   private void Spawn()
   {
      for(var i = 0; i < clusterSpawn; i++)
      {
         _t = 0;
         if (MixEnemies() && i >= 3)
         {
            var go = Instantiate(
               GetRandomEnemy(
                  CheckIfEnemyIsValid(_currentEnemyIndex + 1) ?
                     _currentEnemyIndex + 1 :
                     _currentEnemyIndex));
            go.transform.position = GetRadius();
            GameManager.Instance.enemiesSpawnedList.Add(go);
         }
         else
         {
            var go = Instantiate(GetRandomEnemy(_currentEnemyIndex));
            go.transform.position = GetRadius();
            GameManager.Instance.enemiesSpawnedList.Add(go);
         }
      }
      
      _enemiesSpawned++;
   }

   [CanBeNull]
   private GameObject GetRandomEnemy(int currentEnemyIndex)
   {
      //Change to use different categories of enemies depending on time
      if (spawnSpecificEnemy)
      {
         return _enemyContainer.allEnemies[enemyIndex];
      }
       
      var enemyThatSpawnsIndex = _enemyContainer.enemyHolder[currentEnemyIndex].enemy.Length;
      return _enemyContainer.enemyHolder[currentEnemyIndex].enemy[GetRandomRange(enemyThatSpawnsIndex)];
      return _phaseIndex switch
      {
         1 => _enemyContainer.phase1Enemies[GetRandomRange(_enemyContainer.phase1Enemies.Length)],
         2 => _enemyContainer.phase2Enemies[GetRandomRange(_enemyContainer.phase2Enemies.Length)],
         3 => _enemyContainer.phase3Enemies[GetRandomRange(_enemyContainer.phase3Enemies.Length)],
         _ => _enemyContainer.phase3Enemies[GetRandomRange(_enemyContainer.phase3Enemies.Length)]
      };
   }

   private bool MixEnemies()
   {
      var seconds = GameManager.Instance.GetSeconds();
      return seconds is > 14 and < 25;
   }
   
   private int GetRandomRange(int i)
   {
      return i == 1 ? 0 : Random.Range(0, i);
   }

   private bool CheckIfEnemyIsValid(int i)
   {
      return _enemyContainer.allEnemies.Length < i;
   }
    
   
   private Vector3 GetRadius()
   {
      var playerTransform = PlayerController.Instance.CurrentPlayerTransform();
      var playerPos = PlayerController.Instance.CurrentPlayerTransform().position;
      var radius = 20f;
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
