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
   private int _mixEnemyIndex;
   private int _levelEnemyIndex;
   private int _randomTriggerIndex;
   
   private int _mixIndex;
   private int _mixedMinEnemies;
   
   private const string MixedEnemies = "mix";
   private const string FrequencyEnemies = "frequency";
   private const string LevelEnemies = "levelEnemies";
   private const string RandomTriggerEnemies = "randomTriggerEnemies";
   
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
      GameManager.LevelIncreased += LevelIncreased;
   }

   private void OnDisable()
   {
      //GameManager.ChangePhase -= BossFight;
      Boss.OnBossKilled -= ChangePhase;
      GameManager.EnemiesLoaded -= StartSpawning;
      GameManager.EnemyIndexIncrease -= IncreaseEnemyIndex;
      GameManager.LevelIncreased -= LevelIncreased;
   }

   private void StartSpawning(EnemyContainer enemyContainer)
   {
      _enemyContainer = enemyContainer;
     
      if (spawnSpecificEnemy)
      {
         SpawnFrequencyEnemy();
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
      _mixEnemyIndex = 0;
      _levelEnemyIndex = 0;
      _mixedMinEnemies = 10;
   }
   

   private void Update()
   {
      if(!_enemiesLoaded) return;
      if (BossPhase) return;
      LinearSpawn();
      MixEnemySpawnCheck();
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
      SpawnFrequencyEnemy();
      MixEnemySpawnCheck();
      PlayerLevelEnemyCheck();
       _t = 0;
   }

   private void MixEnemySpawnCheck()
   {
      if(!TimeCheck()) return;
      
      if (MixedMinEnemiesCheck())
      {
         SpawnMixEnemies();
      }
   }

   private void PlayerLevelEnemyCheck()
   {
      if(!LevelCheck()) return;
      SpawnLevelEnemies();
   }

   private void IncreaseEnemyIndex()
   {
      //_currentEnemyIndex++;
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

   private bool LevelCheck()
   {
      var playerLevel = GameManager.Instance.CurrentLevel;
      return playerLevel % 2 == 0 && playerLevel != 1;
   }

   private void LevelIncreased(int currentLevel)
   {
      if(currentLevel % 4 != 0) return;
      var levelIndexIncrease = _levelEnemyIndex + 1;
      levelIndexIncrease = Mathf.Clamp(levelIndexIncrease, 0, GetLength(LevelEnemies) -1);
      _levelEnemyIndex = levelIndexIncrease;
   }

   private void CheckTime(float minutes, float seconds)
   {
      if (minutes == 0 || minutes % 4 != 0 || seconds != 0) return;
      var mixEnemyIndex = _mixEnemyIndex + 1;
      mixEnemyIndex = Mathf.Clamp(mixEnemyIndex, 0,GetLength(MixedEnemies));
      _mixEnemyIndex = mixEnemyIndex;
   }
   
   private bool MixedMinEnemiesCheck()
   {
      return GameManager.Instance.enemiesSpawnedList.Count <= _mixedMinEnemies; // change to percentage
   }

   private bool TimeCheck()
   {
      var minutes = GameManager.Instance.GetMinutes();
      var seconds = GameManager.Instance.GetSeconds();
      CheckTime(minutes, seconds);
      return minutes % 2 == 1 && minutes != 0 && seconds < 45;
   }

   private void SpawnFrequencyEnemy()
   {
      for (var i = 0; i < clusterSpawn; i++)
      {
         var go = Instantiate(GetRandomEnemy(FrequencyEnemies,_currentEnemyIndex));
         go.transform.position = GetRadius();
         GameManager.Instance.enemiesSpawnedList.Add(go);
         _enemiesSpawned++;
      }
   }

   private void SpawnMixEnemies()
   {
      StartCoroutine(WaitToSpawn(MixedEnemies, _mixEnemyIndex));
   }

   private void SpawnLevelEnemies()
   {
      var playerLevel = GameManager.Instance.CurrentLevel;
      var randomEnemy = playerLevel % 10 == 0 ? Random.Range(
         0, Mathf.Clamp(
            _levelEnemyIndex + 2, 0, GetLength(LevelEnemies) -1)) : 
         _levelEnemyIndex;
      StartCoroutine(WaitToSpawn(LevelEnemies, randomEnemy));
   }

   private IEnumerator WaitToSpawn(string typeOfEnemy,int index)
   {
      var r = Random.Range(2, 6);
      for (var i = 0; i < r; i++)
      {
         var go = Instantiate(GetRandomEnemy(typeOfEnemy,index));
         go.transform.position = GetRadius();
         GameManager.Instance.enemiesSpawnedList.Add(go);
         _enemiesSpawned++;
      }
      
      yield return new WaitForSeconds(timeToSpawn);
      switch (typeOfEnemy)
      {
         case MixedEnemies:
            MixEnemySpawnCheck();
            break;
         case LevelEnemies:
            PlayerLevelEnemyCheck();
            break;
         case RandomTriggerEnemies:
            break;
      }
   }

   private void RandomTriggerSpawn()
   {
      for (var i = 0; i < clusterSpawn; i++)
      {
         var go = Instantiate(GetRandomEnemy(RandomTriggerEnemies, _randomTriggerIndex));
         go.transform.position = GetRadius();
         GameManager.Instance.enemiesSpawnedList.Add(go);
         _enemiesSpawned++;
      }
   }

   /*private void Spawn()
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
   }*/

   private GameObject EnemyThatSpawns(int currentEnemyIndex)
   {
      var enemyThatSpawnsIndex = _enemyContainer.enemyHolder[currentEnemyIndex].enemy.Length;
      return _enemyContainer.enemyHolder[currentEnemyIndex].enemy[GetRandomRange(enemyThatSpawnsIndex)];
   }

   [CanBeNull]
   private GameObject GetRandomEnemy(string typeOfSpawn, int currentEnemyIndex)
   {
      //Change to use different categories of enemies depending on time
      if (spawnSpecificEnemy)
      {
         return _enemyContainer.allEnemies[enemyIndex];
      }

      switch (typeOfSpawn)
      {
         case FrequencyEnemies:
         {
            var enemyLength = _enemyContainer.frequencyEnemy[currentEnemyIndex].enemy.Length;
            return _enemyContainer.frequencyEnemy[currentEnemyIndex].enemy[GetRandomRange(enemyLength)];
         }
         case MixedEnemies:
         {
            var enemyLength = _enemyContainer.mixedEnemies[currentEnemyIndex].enemy.Length;
            return _enemyContainer.mixedEnemies[currentEnemyIndex].enemy[GetRandomRange(enemyLength)];
         }
         case LevelEnemies:
         {
            var enemyLength = _enemyContainer.playerLevelEnemy[currentEnemyIndex].enemy.Length;
            return _enemyContainer.playerLevelEnemy[currentEnemyIndex].enemy[GetRandomRange(enemyLength)];
         }
         case RandomTriggerEnemies:
         {
            var enemyLength = _enemyContainer.randomTriggerEnemy[currentEnemyIndex].enemy.Length;
            return _enemyContainer.randomTriggerEnemy[currentEnemyIndex].enemy[GetRandomRange(enemyLength)];
         }
         default:
            return null;
      }
      
      //var enemyThatSpawnsIndex = _enemyContainer.enemyHolder[currentEnemyIndex].enemy.Length;
      //return _enemyContainer.enemyHolder[currentEnemyIndex].enemy[GetRandomRange(enemyThatSpawnsIndex)];
      return _phaseIndex switch
      {
         1 => _enemyContainer.phase1Enemies[GetRandomRange(_enemyContainer.phase1Enemies.Length)],
         2 => _enemyContainer.phase2Enemies[GetRandomRange(_enemyContainer.phase2Enemies.Length)],
         3 => _enemyContainer.phase3Enemies[GetRandomRange(_enemyContainer.phase3Enemies.Length)],
         _ => _enemyContainer.phase3Enemies[GetRandomRange(_enemyContainer.phase3Enemies.Length)]
      };
   }
    
   private int GetRandomRange(int i)
   {
      return i == 1 ? 0 : Random.Range(0, i);
   }

   private bool CheckIfEnemyIsValid(int i)
   {
      return _enemyContainer.allEnemies.Length < i;
   }

   private int GetLength(string typeOfSpawn)
   {
      switch (typeOfSpawn)
      {
         case FrequencyEnemies:
         {
           return  _enemyContainer.frequencyEnemy.Length;
         }
         case MixedEnemies:
         {
            return _enemyContainer.mixedEnemies.Length;
         }
         case LevelEnemies:
         {
            return _enemyContainer.playerLevelEnemy.Length;
         }
         case RandomTriggerEnemies:
         {
            return _enemyContainer.randomTriggerEnemy.Length;
         }
         default:
            return 0;
      }
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
