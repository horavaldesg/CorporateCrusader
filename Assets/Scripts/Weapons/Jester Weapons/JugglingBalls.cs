using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JugglingBalls : SelectedWeapon
{
    [SerializeField] private float force;
    
    private int AmountOfBounces
    {
        get;
        set;
    }

    protected override void Start()
    {
        AmountOfBounces = 5;
        base.Start();
    }

    protected override void Activate()
    {
        ShootBalls();
        base.Activate();
    }

    private void ShootBalls()
    {
        var go = Instantiate(instantiatedObject);
        instantiatedObject.transform.position = transform.position;
        go.TryGetComponent(out JugglingBall jugglingBall);
        jugglingBall.amountOfBounces = AmountOfBounces;
        var forceDir = transform.right * (force * 10);
        jugglingBall.damage = damage;
        jugglingBall.attributes = attribute;
        //jugglingBall.rb.AddForce(forceDir);
        jugglingBall.force = force;
    }

    #region Level Upgrades

    protected override void Level1Upgrade()
    {
        AmountOfBounces = 5;
        base.Level1Upgrade();
    }

    protected override void Level2Upgrade()
    {
        AmountOfBounces = 7;
        base.Level2Upgrade();
    }

    protected override void Level3Upgrade()
    {
        AmountOfBounces = 8;
        base.Level3Upgrade();
    }
    
    protected override void Level4Upgrade()
    {
        AmountOfBounces = 9;
        base.Level3Upgrade();
    }
    
    protected override void Level5Upgrade()
    {
        AmountOfBounces = 10;
        base.Level3Upgrade();
    }
    
    #endregion
}
