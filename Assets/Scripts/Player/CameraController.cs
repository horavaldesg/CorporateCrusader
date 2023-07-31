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
    private const float PortraitZoom = 10;

    private void Awake()
    {
        TryGetComponent(out _camera);
        ChangeZoom(MinZoom);
    }

    private void FixedUpdate()
    {
        FollowPlayer();
        CheckOrientation();
    }

    private bool IsPortrait()
    {
        return Screen.orientation == ScreenOrientation.Portrait;
    }

    private void CheckOrientation()
    {
        Debug.Log(Screen.orientation);
        switch (Screen.orientation)
        {
            case ScreenOrientation.Portrait:
            case ScreenOrientation.PortraitUpsideDown:
                ChangeZoom(PortraitZoom);
                break;
            case ScreenOrientation.LandscapeLeft:
            case ScreenOrientation.LandscapeRight:
                ChangeZoom(MinZoom);
                break;
        }
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
        if (IsPortrait()) return;
        if (enemiesVisible.Count >= 5)
        {
            var lerpZoom = Mathf.Lerp(_camera.orthographicSize, _camera.orthographicSize + 0.5f, Time.deltaTime * 0.5f);
            var zoomVal = Mathf.Clamp(lerpZoom, MinZoom, MaxZoom);
            ChangeZoom(zoomVal);
        }
        else if (enemiesVisible.Count < 5)
        {
            var lerpZoom = Mathf.Lerp(_camera.orthographicSize, _camera.orthographicSize - 0.5f, Time.deltaTime * 0.5f);
            var zoomVal = Mathf.Clamp(lerpZoom, MinZoom, MaxZoom);
            ChangeZoom(zoomVal);
        }
    }

    private void ChangeZoom(float zoomVal)
    {
        _camera.orthographicSize = zoomVal;
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
