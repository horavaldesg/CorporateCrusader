using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class RoboVacCat : Enemy
{
    //Character Components
    [SerializeField] private GameObject go_spawnPoint_Holder, go_spawnPoint_surroundPlayer_Holder, go_spawnPoint_playerBlocking_Holder, go_defenseField;

    //Player Details
    private GameObject go_player;
    private float f_distanceToPlayer;

    public override void FixedUpdate()
    {
        var playerPosition = PlayerController.Instance.CurrentPlayerTransform().position;
        f_distanceToPlayer = Vector2.Distance(playerPosition, this.gameObject.transform.position);
        AimSpawnPoints();

        if (f_distanceToPlayer >= 10f)
        {
          base.FixedUpdate();
        } else if (f_distanceToPlayer <= 8f)
        {
            _rb.velocity = Vector2.zero;
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

        go_spawnPoint_surroundPlayer_Holder.transform.position = playerPosition;
        go_spawnPoint_surroundPlayer_Holder.transform.eulerAngles += Vector3.forward * 1.75f;

        go_spawnPoint_playerBlocking_Holder.transform.position = playerPosition;
        go_spawnPoint_playerBlocking_Holder.transform.rotation = PlayerController.Instance.GunRotation();
    }

    private void AimSpawnPoints()
    {
        Vector2 aimDirection = new Vector2(go_player.transform.position.x - go_spawnPoint_Holder.transform.position.x, go_player.transform.position.y - go_spawnPoint_Holder.transform.position.y);
        float rotation = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        go_spawnPoint_Holder.transform.eulerAngles = new Vector3(0, 0, rotation);
    }
}
