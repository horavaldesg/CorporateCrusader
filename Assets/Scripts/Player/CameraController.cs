using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float lerpTime;
    private event Action OnEnemyEntered;
    public List<GameObject> enemiesVisible = new();
    private Camera _camera;
    private const float MinZoom = 6;
    private const float MaxZoom = 15;

    private void Awake()
    {
        TryGetComponent(out _camera);
        _camera.orthographicSize = MinZoom;
    }

    private void FixedUpdate()
    {
        FollowPlayer();
    }

    private void OnEnable()
    {
        OnEnemyEntered += ZoomControl;
    }

    private void OnDisable()
    {
        OnEnemyEntered -= ZoomControl;
    }

    private void ZoomControl()
    {
        if (enemiesVisible.Count >= 5)
        {
            var lerpZoom = Mathf.Lerp(_camera.orthographicSize, _camera.orthographicSize + 0.1f, Time.deltaTime * 0.5f);
            _camera.orthographicSize = Mathf.Clamp(lerpZoom, MinZoom, MaxZoom);
        }
        else if (enemiesVisible.Count < 5)
        {
            var lerpZoom = Mathf.Lerp(_camera.orthographicSize, _camera.orthographicSize - 0.1f, Time.deltaTime * 0.5f);
            
            _camera.orthographicSize = Mathf.Clamp(lerpZoom, MinZoom, MaxZoom);
        }
    }

    private void FollowPlayer()
    {
        var playerPosition = PlayerController.Instance.CurrentPlayerTransform().position;
        var lerpedPosX = Mathf.Lerp(transform.position.x, playerPosition.x, Time.deltaTime * lerpTime);
        var lerpedPosY = Mathf.Lerp(transform.position.y, playerPosition.y, Time.deltaTime * lerpTime);
        transform.position = new Vector3(lerpedPosX, lerpedPosY, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Enemy"))return;
        if(enemiesVisible.Contains(other.gameObject)) return;
        OnEnemyEntered?.Invoke();
        enemiesVisible.Add(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(!other.CompareTag("Enemy"))return;
        if (!other.gameObject) return;
        OnEnemyEntered?.Invoke();
        enemiesVisible.Remove(other.gameObject);
    }
}
