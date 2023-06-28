using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
   private EnemyContainer _enemyContainer;

   private void Awake()
   {
      _enemyContainer = Resources.Load<EnemyContainer>("EnemyContainer/EnemyContainer");
   }

   private void Start()
   {
      StartCoroutine(Spawn());
   }

   private IEnumerator Spawn()
   {
      var go = Instantiate(_enemyContainer.phase1Enemies[GetRandomRange(_enemyContainer.phase1Enemies.Length)]);
      go.transform.position = GetRadius();
      yield return new WaitForSeconds(1.5f);
      StartCoroutine(Spawn());
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
