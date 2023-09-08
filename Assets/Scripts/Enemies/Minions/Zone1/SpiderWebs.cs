using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(
    typeof(Rigidbody2D),
    typeof(CircleCollider2D))]
public class SpiderWebs : MonoBehaviour
{
    private Rigidbody2D _rb;
    [HideInInspector] public float damage;
    [SerializeField] private Sprite staticWeb;
    private SpriteRenderer _currentImage;
    private PlayerController _playerController;
    private CircleCollider2D _collider2D;
    public float force;
    private bool _isStatic;
    
    private void Awake()
    {
        TryGetComponent(out _rb);
        TryGetComponent(out _collider2D);
        TryGetComponent(out _currentImage);
        //Destroy(gameObject, 5);
    }

    private void Start()
    {
        StartCoroutine(WaitToBecomeStatic());
    }

    public void Shoot(Transform spiderPos)
    {
        var shootDirection = PlayerController.Instance.CurrentPlayerTransform().position - spiderPos.position;

        var shootForce = shootDirection * (force * Time.deltaTime);
        _rb.AddForce(shootForce * 100);
    }

    private IEnumerator WaitToBecomeStatic()
    {
        yield return new WaitForSeconds(2);
        StaticWeb();
        Destroy(gameObject, 15);
    }

    private void StaticWeb()
    {
        //scale 0.25f
        // col radius 2.3
        transform.localScale = new Vector3(0.25f, 0.25f, 1);
        _collider2D.radius = 2.3f;
        _currentImage.sprite = staticWeb;
        _rb.velocity = Vector2.zero;
        _isStatic = true;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        other.TryGetComponent(out PlayerController playerController);
        if (!playerController)return;
        _playerController = playerController;
        (_isStatic ? (Action)SlowPlayer : DamagePlayer)();
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        other.TryGetComponent(out PlayerController playerController);
        if (!playerController)return;
        RestoreSpeed();
    }

    private void DamagePlayer()
    {
        _playerController.TakeDamage(damage);
        Destroy(gameObject);
    }
    
    private void SlowPlayer()
    {
        var slowDownSpeed = _playerController.GetCurrentSpeed() / 2;
        _playerController.SlowDownPlayer(slowDownSpeed);
    }

    private void RestoreSpeed()
    {
        _playerController.RestoreSpeed();
    }
}
