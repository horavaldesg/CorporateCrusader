using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : SelectedWeapon
{
    [SerializeField] private float throwForce;
    
    protected override void Activate()
    {
        Throw();
        base.Activate();
    }

    private void Throw()
    {
        transform.eulerAngles = GetRandomRotation();
        var go = Instantiate(instantiatedObject);
        go.transform.position = PlayerController.Instance.CurrentPlayerTransform().position;
        go.transform.eulerAngles = transform.eulerAngles;
        go.TryGetComponent(out Rigidbody2D rb);
        go.TryGetComponent(out AnchorThrowable anchorThrowable);
        anchorThrowable.damage = damage;
        Vector3 forceAngle = new Vector3(transform.position.x + Random.Range(-0.7f, 0.7f), transform.position.y + 1, transform.position.z) - transform.position;
        rb.AddForce(forceAngle * (throwForce * 100));
    }

    private void FixedUpdate()
    {
        var playerPos = PlayerController.Instance.CurrentPlayerTransform().position;
        transform.position = playerPos;
    }

    private Vector3 GetRandomRotation()
    {
        return new Vector3(0, 0, Random.Range(-100, 100));
    }
}
