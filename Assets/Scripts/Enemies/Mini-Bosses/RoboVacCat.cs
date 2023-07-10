using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class RoboVacCat : Enemy
{
    //Character Components
    [SerializeField] private RectTransform rt_healthBar;
    [SerializeField] private GameObject go_spawnPoint_Holder, go_spawnPoint_surroundPlayer_Holder, go_spawnPoint_playerBlocking_Holder, go_defenseField;
    private Rigidbody2D rb_rigidbody;
    private Collider2D co_collider;

    //Character Details
    [SerializeField] private float f_amountOfXpToDrop, f_speed, f_attackRange, f_attackCooldown;
    public float f_health;
    private float f_baseHealth, f_damage1, f_attack1Cooldown;

    //Player Details
    private GameObject go_player, go_playerWeapon;
    private float f_distanceToPlayer;

    //Misc
    [SerializeField] private GameObject go_xpObject;
    private BossStats bossStats;
    private readonly List<Transform> lst_nearbyEnemies = new List<Transform>();
    private const float f_avoidanceRadius = 0.15f;

    private void Awake()
    {
        bossStats = Resources.Load<BossStats>("EnemyStats/Mini-Bosses/RoboVacCat");

        f_health = bossStats.health;
        f_baseHealth = f_health;
        f_damage1 = bossStats.damage1;
        f_attack1Cooldown = bossStats.cooldown1;

        TryGetComponent(out rb_rigidbody);
        TryGetComponent(out co_collider);

        //Get Player
        go_player = GameObject.FindGameObjectWithTag("Player");
        f_distanceToPlayer = Vector2.Distance(go_player.transform.position, this.gameObject.transform.position);
        go_playerWeapon = go_player.transform.GetChild(0).gameObject;
    }

    void Start()
    {
        StartCoroutine(UpdateNearbyEnemies());
    }

    private IEnumerator UpdateNearbyEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            lst_nearbyEnemies.Clear();
            var colliders = Physics2D.OverlapCircleAll(transform.position, f_avoidanceRadius);

            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Enemy") && collider.transform != transform)
                {
                    lst_nearbyEnemies.Add(collider.transform);
                }
            }
        }
    }

    void FixedUpdate()
    {
        f_distanceToPlayer = Vector2.Distance(go_player.transform.position, this.gameObject.transform.position);
        AimSpawnPoints();

        if (f_distanceToPlayer >= 10f)
        {
           Move();
        } else if (f_distanceToPlayer <= 8f)
        {
            rb_rigidbody.velocity = Vector2.zero;
        }
        //NOTE: Due to the "floatiness of the NPC when not being moved, a deadzone will be used to simulate a sort of drift with the vacuum

        if (f_distanceToPlayer <= 6f)
        {
            Vector3 fieldScale = new Vector3();
            fieldScale.x = Mathf.Clamp(go_defenseField.transform.localScale.x + .3f, .1f, 15f);
            fieldScale.y = Mathf.Clamp(go_defenseField.transform.localScale.y + .3f, .1f, 15f);
            fieldScale.z = Mathf.Clamp(go_defenseField.transform.localScale.z + .3f, .1f, 15f);
            go_defenseField.transform.localScale = fieldScale;
        } else
        {
            Vector3 fieldScale = new Vector3();
            fieldScale.x = Mathf.Clamp(go_defenseField.transform.localScale.x - .2f, .1f, 15f);
            fieldScale.y = Mathf.Clamp(go_defenseField.transform.localScale.y - .2f, .1f, 15f);
            fieldScale.z = Mathf.Clamp(go_defenseField.transform.localScale.z - .2f, .1f, 15f);
            go_defenseField.transform.localScale = fieldScale;
        }

        go_spawnPoint_surroundPlayer_Holder.transform.position = go_player.transform.position;
        go_spawnPoint_surroundPlayer_Holder.transform.eulerAngles += Vector3.forward * 1.75f;

        go_spawnPoint_playerBlocking_Holder.transform.position = go_player.transform.position;
        go_spawnPoint_playerBlocking_Holder.transform.rotation = go_playerWeapon.transform.rotation;
    }

    private void Move()
    {
        var playerPos = PlayerController.Instance.CurrentPlayerTransform().position;
        var movement = (Vector2)(playerPos - transform.position).normalized;

        foreach (var enemy in lst_nearbyEnemies)
        {
            if (!enemy) continue;
            Vector2 avoidanceVector = (transform.position - enemy.position).normalized;
            movement += avoidanceVector;
        }

        if (lst_nearbyEnemies.Count > 0)
        {
            movement /= lst_nearbyEnemies.Count;
        }

        rb_rigidbody.velocity = movement.normalized * f_speed;
    }

    private void AimSpawnPoints()
    {
        Vector2 aimDirection = new Vector2(go_player.transform.position.x - go_spawnPoint_Holder.transform.position.x, go_player.transform.position.y - go_spawnPoint_Holder.transform.position.y);
        float rotation = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        go_spawnPoint_Holder.transform.eulerAngles = new Vector3(0, 0, rotation);
    }
}
