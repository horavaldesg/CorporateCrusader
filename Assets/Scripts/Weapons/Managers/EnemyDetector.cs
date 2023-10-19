using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyDetector : MonoBehaviour
{
    public static EnemyDetector Instance;
    public List<GameObject> enemies = new List<GameObject>();
    public event Action StartCheck;
    public event Action EndShoot;
    public static event Action<int> EnemyDied;
    private bool _collided;
    private int _enemiesKilled;

    private void Awake()
    {
        Instance = this;
        _enemiesKilled = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Enemy") || other.CompareTag("Crate")) return;
        if(other.CompareTag("Enemy")) enemies.Add(other.gameObject);
        if(!_collided)
            StartCheck?.Invoke();
        _collided = true;
    }

    public void EnemyKilled(GameObject enemy)
    {
        enemies.Remove(enemy);
        _enemiesKilled++;
        EnemyDied?.Invoke(_enemiesKilled);
        Destroy(enemy);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        if (enemies.Contains(other.gameObject))
            enemies.Remove(other.gameObject);
        EndShoot?.Invoke();
        _collided = false;
    }
}
