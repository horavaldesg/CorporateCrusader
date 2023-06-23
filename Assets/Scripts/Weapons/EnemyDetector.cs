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
    private bool _collided;

    private void Awake()
    {
        Instance = this;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Enemy")) return;
        enemies.Add(other.gameObject);
        if(!_collided)
            StartCheck?.Invoke();
        _collided = true;
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
