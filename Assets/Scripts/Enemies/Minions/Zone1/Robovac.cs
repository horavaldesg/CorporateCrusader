using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robovac : Enemy
{
    [SerializeField] private bool isHelper;
    [SerializeField] private float f_desired_AttackCooldown;
    [SerializeField] private float f_ActivationRange, f_ChargeDuration, f_PrepTime;
    [SerializeField] private GameObject go_Holder_AttackSignal, go_Explosion;
    [SerializeField] private TrailRenderer tr_trailRenderer;
    private float f_timer = 0f, f_actual_AttackCooldown, f_detonationDelay = 15f, f_detonationTimer;
    private bool isCharging = false;

    // Start is called before the first frame update
    void Start()
    {
        if (isHelper == true)
        {
            f_detonationDelay += Random.Range(5f, 10f);
        }

        f_actual_AttackCooldown = f_desired_AttackCooldown + Random.Range(0f, 5f);
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {
        if (isHelper == true)
        {
            f_detonationTimer += 1 * Time.deltaTime;

            if (f_detonationTimer >= f_detonationDelay)
            {
                f_timer = -100f;
                StartCoroutine(Detonation());
            }
        }

        if (f_timer < f_actual_AttackCooldown && isCharging == false)
        {
            f_timer += 1 * Time.deltaTime;
            base.FixedUpdate();
        }

        if (f_timer >= f_actual_AttackCooldown)
        {
            var playerPosition = PlayerController.Instance.CurrentPlayerTransform().position;
            var distanceToPlayer = Vector2.Distance(playerPosition, this.gameObject.transform.position);

            if (distanceToPlayer <= f_ActivationRange && isCharging == false)
            {
                isCharging = true;
                StartCoroutine(Charge(_rb));
            } else
            {
                f_timer = 0f;
            }
        }
    }

    IEnumerator Charge(Rigidbody2D rb)
    {
        rb.velocity = Vector2.zero;
        go_Holder_AttackSignal.SetActive(true);
        tr_trailRenderer.enabled = true;

        yield return new WaitForSeconds(f_PrepTime); //2 for preptime seened good
        var playerPosition = PlayerController.Instance.CurrentPlayerTransform().position;

        yield return new WaitForSeconds(0.25f);
        rb.velocity = (Vector2)(playerPosition + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0) - this.transform.position).normalized * 10;

        yield return new WaitForSeconds(f_ChargeDuration); //1.5 for chargeduration seemed good
        go_Holder_AttackSignal.SetActive(false);
        tr_trailRenderer.enabled = false;
        isCharging = false;
        f_timer = 0f;
    }

    IEnumerator Detonation()
    {
        yield return new WaitForSeconds(2f);
        GameObject newBoom = Instantiate(go_Explosion, this.transform.position, Quaternion.identity);
        Destroy(gameObject);
        yield return newBoom;
    }
}
