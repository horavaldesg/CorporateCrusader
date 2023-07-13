using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]

public class SpiderVac : Enemy
{
    [Header("Movement Variables")]
    private int i_NextPoint, i_PointsReached;
    private Vector3 v3_MovementOffset;

    [Header("Attack Variables")]
    [SerializeField] private float f_ChargeDelay;
    [SerializeField] private float f_ChargeDuration;
    private bool isCharging;

    // Start is called before the first frame update
    void Start()
    {
        i_NextPoint = 0;
        i_PointsReached = 0;
        //i_CycleStart = 0;
        //i_CycleEnd = 7;
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {
        if (isCharging == false)
        {
            FollowPoints();
        } else
        {
            //StartCoroutine(Charge());
            Invoke("Charge", f_ChargeDelay);
        }
    }

    private void FollowPoints()
    {
        var playerPosition = PlayerController.Instance.CurrentPlayerTransform().position;

        switch (i_NextPoint)
        {
            case 0:
                v3_MovementOffset = playerPosition + new Vector3(0, 15, 0);
                break;
            case 1:
                v3_MovementOffset = playerPosition + new Vector3(11, 11, 0);
                break;
            case 2:
                v3_MovementOffset = playerPosition + new Vector3(15, 0, 0);
                break;
            case 3:
                v3_MovementOffset = playerPosition + new Vector3(11, -11, 0);
                break;
            case 4:
                v3_MovementOffset = playerPosition + new Vector3(0, -15, 0);
                break;
            case 5:
                v3_MovementOffset = playerPosition + new Vector3(-11, -11, 0);
                break;
            case 6:
                v3_MovementOffset = playerPosition + new Vector3(-15, 0, 0);
                break;
            case 7:
                v3_MovementOffset = playerPosition + new Vector3(-11, 11, 0);
                break;
        }

        var movement = (Vector2)(v3_MovementOffset - transform.position).normalized;
        _rb.velocity = movement.normalized * speed;

        var distance = Vector3.Distance(v3_MovementOffset, transform.position);
        if(distance <= 1f && isCharging == false)
        {
            if (i_PointsReached < 7)
            {
                i_PointsReached++;

                if (i_NextPoint < 7)
                {
                    i_NextPoint++;
                }
                else
                {
                    i_NextPoint = 0;
                }
            }
            else
            {
                isCharging = true;
                _rb.velocity = Vector3.zero;
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            isCharging = true;
        }
    }

/*    IEnumerator Charge()
    {
        _rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(f_ChargeDelay);
        var chargePosition = PlayerController.Instance.CurrentPlayerTransform().position;
        _rb.velocity = (Vector2)(chargePosition - this.transform.position).normalized * 10;

        yield return new WaitForSeconds(f_ChargeDuration);

        if (i_NextPoint < 7)
        {
            i_NextPoint++;
        }
        else
        {
            i_NextPoint = 0;
        }

        i_PointsReached = 0;
        isCharging = false;
        StopCoroutine(Charge());
    }*/

    private void Charge()
    {
        var chargePosition = PlayerController.Instance.CurrentPlayerTransform().position;
        _rb.velocity = (Vector2)(chargePosition - this.transform.position).normalized * 10;
        Invoke("Reset", f_ChargeDuration);
    }

    private void Reset()
    {
        i_NextPoint = 0;
        i_PointsReached = 0;
        isCharging = false;
    }
}
