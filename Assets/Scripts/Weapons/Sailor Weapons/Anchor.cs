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
        rb.AddForce(go.transform.right * (throwForce * 100));
    }

    private void FixedUpdate()
    {
        var playerPos = PlayerController.Instance.CurrentPlayerTransform().position;
        transform.position = playerPos;
    }

    private Vector3 GetRandomRotation()
    {
        return new Vector3(0, 0, Random.Range(45, 100));
    }
}
