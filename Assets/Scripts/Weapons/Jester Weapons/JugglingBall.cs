using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class JugglingBall : MonoBehaviour
{
    [HideInInspector] public int amountOfBounces;
    [HideInInspector] public float force;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public float damage;
    
    [SerializeField] private Sprite[] ballSprites;
    
    private SpriteRenderer _spriteRenderer;

    private int _currentBounces;
    private Camera _mainCamera;

    private const string SideRight = "sideRight";
    private const string SideLeft = "sideLeft";
    private const string Top = "Top";
    private const string Bottom = "Bottom";
    private Vector3 direction;
    private Vector3 lastDirection;
    public float speed = 5.0f;

    private float _screenWidth;
    private float _screenHeight;
    private bool _isBouncing;
    
    // Define screen safe space as a percentage of the screen width and height.
    public float screenSafeSpacePercentageX = 5.0f; // Adjust this as needed.
    public float screenSafeSpacePercentageY = 10.0f; // Adjust this as needed.

    private float minX, maxX, minY, maxY;
    private void Awake()
    {
        TryGetComponent(out rb);
        TryGetComponent(out _spriteRenderer);
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        var i = Random.Range(0, ballSprites.Length);
        _spriteRenderer.sprite = ballSprites[i];
        _currentBounces = 0;
        direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
        lastDirection = direction;
        _screenWidth = Screen.safeArea.width - 50;
        _screenHeight = Screen.safeArea.height - 100;
        CalculateScreenSafeSpace();

    }
    
    private void CalculateScreenSafeSpace()
    {
        // Calculate the screen safe space boundaries based on the percentage of the screen size.
        var screenSafeSpaceX = Screen.width * (screenSafeSpacePercentageX / 100.0f);
        var screenSafeSpaceY = Screen.height * (screenSafeSpacePercentageY / 100.0f);

        minX = screenSafeSpaceX;
        maxX = Screen.width - screenSafeSpaceX;
        minY = screenSafeSpaceY;
        maxY = Screen.height - screenSafeSpaceY;
    }
    
    private void Update()
    {
        // Move the circle in its current direction.
        transform.position += direction * (speed * Time.deltaTime);

        // Check if the circle has hit the screen edges.
        CheckScreenEdges();
    }

    private void CheckScreenEdges()
    {
        var screenPos = _mainCamera.WorldToScreenPoint(transform.position);
        
        if (screenPos.x <= minX || screenPos.x >= maxX)
        {
            // Bounce off the horizontal edges by reversing the x direction.
            lastDirection = direction;
            direction.x *= -1;
            IncrementBounceCount();
            _isBouncing = true;
        }

        if (screenPos.y <= minY || screenPos.y >= maxY)
        {
            // Bounce off the vertical edges by reversing the y direction.
            lastDirection = direction;
            direction.y *= -1;
            IncrementBounceCount();
            _isBouncing = true;
        }
        else
        {
            // Reset isBouncing when the circle moves away from the screen edge.
            if (screenPos.x > 0 && screenPos.x < _screenWidth && screenPos.y > 0 && screenPos.y < _screenHeight)
            {
                _isBouncing = false;
            }
        }
    }

    void IncrementBounceCount()
    {
        if(_isBouncing) return;
        _currentBounces++;

        if (_currentBounces >= amountOfBounces)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        other.TryGetComponent(out Enemy enemy);
        if(!enemy) return;
        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}
