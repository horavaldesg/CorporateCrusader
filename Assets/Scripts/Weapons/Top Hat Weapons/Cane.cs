using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cane : SelectedWeapon
{
    public float rotateSpeed;
    public float length;
    
    protected override void Start()
    {
        transform.localScale = new Vector3(length, length, length);
    }

    public void UpdateLength(float lengthMultiplier)
    {
        length += lengthMultiplier;
        transform.localScale = new Vector3(length, length, length);
    }

    private void FixedUpdate()
    {
        transform.Rotate(transform.forward * -rotateSpeed);
    }
}
