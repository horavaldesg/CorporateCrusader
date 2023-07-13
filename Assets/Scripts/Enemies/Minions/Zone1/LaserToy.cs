using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserToy : Enemy
{
    [SerializeField] private bool isHelper;
    [SerializeField] private float f_desired_AttackCooldown;
    [SerializeField] private float f_Range;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject body, go_Explosion;
    private float f_timer = 0, f_actual_AttackCooldown, f_detonationDelay = 15f, f_detonationTimer;

    private void Start()
    {
        if (isHelper == true)
        {
            f_detonationDelay += Random.Range(5f, 10f);
        }

        f_actual_AttackCooldown = f_desired_AttackCooldown + Random.Range(0f, 2f);
    }

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

        if (f_timer < f_actual_AttackCooldown)
        {
            f_timer += 1 * Time.deltaTime;

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("laserToyMove"))
            {
/*                if (_rb.velocity.x > 0)
                {
                    body.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                }

                if (_rb.velocity.x < 0)
                {
                    body.transform.localScale = new Vector3(-0.25f, 0.25f, 0.25f);
                }*/

                base.FixedUpdate();
            }
        } else if (f_timer > f_actual_AttackCooldown)
        {
            var playerPosition = PlayerController.Instance.CurrentPlayerTransform().position;
            var distanceToPlayer = Vector2.Distance(playerPosition, this.gameObject.transform.position);

            if (distanceToPlayer <= f_Range)
            {
                _rb.velocity = Vector2.zero;
                BeamAttack();
            } else
            {
                f_actual_AttackCooldown = f_desired_AttackCooldown + Random.Range(0f, 2f);
                f_timer = 0;
            }
        }
    }

    private void BeamAttack()
    {
        body.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        var playerPosition = PlayerController.Instance.CurrentPlayerTransform().position;
        Vector2 aimDirection = new Vector2(playerPosition.x - this.transform.position.x, playerPosition.y - this.transform.position.y);                //Get direction to player
        float rotation = (Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg) + 9;  // + 9 Needed to account for animation offset                                                                                  //Get a rotation based on direction
        this.transform.eulerAngles = new Vector3(0, 0, rotation);
        animator.Play("LaserToy_Fire");
        f_timer = 0;
    }

    IEnumerator Detonation()
    {
        yield return new WaitForSeconds(2f);
        GameObject newBoom = Instantiate(go_Explosion, this.transform.position, Quaternion.identity);
        Destroy(gameObject);
        yield return newBoom;
    }
}
