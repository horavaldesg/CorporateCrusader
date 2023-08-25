using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Enemy
{
    [SerializeField] private GameObject spiderWebsObject;
    
    protected override void Move()
    {
        var playerPos = PlayerController.Instance.CurrentPlayerTransform().position;
        var differenceVector = playerPos - transform.position;
        if (differenceVector.magnitude < attackRange)
        {
            //Cooldown attack
            StopEnemy();
            attackTime += Time.deltaTime;
            if(attackTime > attackCooldown)
                Attack();
        }
        else
        {
            base.Move();
        }
    }

    protected override void Attack()
    {
        var go = Instantiate(spiderWebsObject, null);
        go.transform.position = transform.position;
        go.TryGetComponent(out SpiderWebs spiderWebs);
        spiderWebs.damage = _damage;
        spiderWebs.Shoot(transform);
        base.Attack();
    }
    
}
