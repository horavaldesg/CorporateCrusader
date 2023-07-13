using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class RoboVacCat : Enemy
{
    //Character Components
    [SerializeField] private GameObject go_spawnPoint_Type1_Holder, go_spawnPoint_Type3_Holder, go_spawnPoint_Type2_Holder, go_defenseField, go_spawnEffect;
    [SerializeField] private Transform[] t_Locations_Type1; //Type1: Spawn points directly infront of enemy
    [SerializeField] private Transform[] t_Locations_Type3; //Type3: Spawn points surrounding the player
    [SerializeField] private Transform[] t_Locations_Type2; //Type2: Spawn points infront of the player's direction of aim
    [SerializeField] private Transform[] t_Locations_Type4; //Type4: Distant spawn points around the enemy
    [SerializeField] private GameObject[] go_SpawnableMinions;
    [SerializeField] private float f_desired_AttackCooldown;
    [SerializeField] private int SpawnCycleLength;  //!!! IMPORTANT: THIS MUST BE DIVISIBLE BY 3 !!!
    private float f_current_AttackCooldown = 0f, f_randomized_AttackCooldown = 0f;
    private int i_cycleCount = 0;

    //Player Details
    private float f_distanceToPlayer;

    private void Start()
    {
        f_randomized_AttackCooldown = f_desired_AttackCooldown + Random.Range(0f, 2f);
    }

    public override void FixedUpdate()
    {
        var playerPosition = PlayerController.Instance.CurrentPlayerTransform().position;
        f_distanceToPlayer = Vector2.Distance(playerPosition, this.gameObject.transform.position);

        AimSpawnPoints(playerPosition);
        DefenseField();
        SurroundPlayer(playerPosition);

        if (f_distanceToPlayer >= 10f)
        {
          base.FixedUpdate();
          //base.FixedUpdate currently only involves moving the character
        } else if (f_distanceToPlayer <= 8f)
        {
            _rb.velocity = Vector2.zero;
        }
        //NOTE: Due to the "floatiness of the NPC when not being moved,
        //a deadzone will be used to simulate a sort of drift with the vacuum

        if (f_current_AttackCooldown < f_randomized_AttackCooldown)
        {
            f_current_AttackCooldown += 1 * Time.deltaTime;
        } else
        {
            i_cycleCount++;
            Attack(i_cycleCount);
            f_randomized_AttackCooldown = f_desired_AttackCooldown + Random.Range(0f, 2f);
            f_current_AttackCooldown = 0f;
        }

    }

    private void DefenseField()                                                                    //Function for RoboVacCat's defense move
    {
        if (f_distanceToPlayer <= 6f)                                                              //If the player is close to the enemy
        {
            Vector3 fieldScale = new Vector3();                                                    //Create a new Vector3 to serve as field size
            fieldScale.x = Mathf.Clamp(go_defenseField.transform.localScale.x + .3f, .1f, 15f);    //Scale components of Vector3 by .3 within given range
            fieldScale.y = Mathf.Clamp(go_defenseField.transform.localScale.y + .3f, .1f, 15f);    
            fieldScale.z = Mathf.Clamp(go_defenseField.transform.localScale.z + .3f, .1f, 15f);
            go_defenseField.transform.localScale = fieldScale;                                     //Set Vector3 as the circle's scale
            go_defenseField.transform.eulerAngles += Vector3.forward * -5f;
        }
        else                                                                                       //If the player is out of range
        {
            Vector3 fieldScale = new Vector3();                                                    //Same as above but reduce scale
            fieldScale.x = Mathf.Clamp(go_defenseField.transform.localScale.x - .2f, .1f, 15f);
            fieldScale.y = Mathf.Clamp(go_defenseField.transform.localScale.y - .2f, .1f, 15f);
            fieldScale.z = Mathf.Clamp(go_defenseField.transform.localScale.z - .2f, .1f, 15f);
            go_defenseField.transform.localScale = fieldScale;
            go_defenseField.transform.eulerAngles += Vector3.forward * -5f;
        }
    }

    private void SurroundPlayer(Vector3 playerPosition)                                                     //Update position for spawn points reliant on player position
    {
        go_spawnPoint_Type3_Holder.transform.position = playerPosition;                            //Set spawn points' holder to player position
        go_spawnPoint_Type3_Holder.transform.eulerAngles += Vector3.forward * 1.75f;               //Give it constant rotation to add randomness

        go_spawnPoint_Type2_Holder.transform.position = playerPosition;                            //Set blocking spawn points' holder to player position
        go_spawnPoint_Type2_Holder.transform.rotation = PlayerController.Instance.GunRotation();   //Rotate in accordance to with player movement direction (gun aim direction)
    }

    private void AimSpawnPoints(Vector3 playerPosition)                                                     //Orient protective spawn points between character and player
    {
        Vector2 aimDirection = new Vector2(playerPosition.x - go_spawnPoint_Type1_Holder.transform.position.x, playerPosition.y - go_spawnPoint_Type1_Holder.transform.position.y);  //Get direction to player
        float rotation = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;                                                                                    //Get a rotation based on direction
        go_spawnPoint_Type1_Holder.transform.eulerAngles = new Vector3(0, 0, rotation);                                                                                        //Apply direction
    }

    private void Attack(int cycleCount)
    {
        if (cycleCount < SpawnCycleLength)
        {
            Instantiate(go_spawnEffect, this.transform.position, Quaternion.identity);
            for (int i = 0; i < 5; i++)
            {
                GameObject newMinion = Instantiate(go_SpawnableMinions[Random.Range(0, go_SpawnableMinions.Length)], t_Locations_Type1[i].position, Quaternion.identity);
                GameObject newEffect = Instantiate(go_spawnEffect, t_Locations_Type1[i].position, Quaternion.identity);
            }
        }

        if (cycleCount % 3 == 0)
        {
            Instantiate(go_spawnEffect, this.transform.position, Quaternion.identity);
            if (Random.Range(0,2) == 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    GameObject newMinion = Instantiate(go_SpawnableMinions[Random.Range(0, go_SpawnableMinions.Length)], t_Locations_Type2[i].position, Quaternion.identity);
                    GameObject newEffect = Instantiate(go_spawnEffect, t_Locations_Type2[i].position, Quaternion.identity);
                }
            } else
            {
                for (int i = 0; i < 8; i++)
                {
                    GameObject newMinion = Instantiate(go_SpawnableMinions[Random.Range(0, go_SpawnableMinions.Length)], t_Locations_Type4[i].position, Quaternion.identity);
                    GameObject newEffect = Instantiate(go_spawnEffect, t_Locations_Type4[i].position, Quaternion.identity);
                }
            }
        }

        if (cycleCount == SpawnCycleLength)
        {
            Instantiate(go_spawnEffect, this.transform.position, Quaternion.identity);
            for (int i = 0; i < 8; i++)
            {
                GameObject newMinion = Instantiate(go_SpawnableMinions[Random.Range(0, go_SpawnableMinions.Length)], t_Locations_Type3[i].position, Quaternion.identity);
                GameObject newEffect = Instantiate(go_spawnEffect, t_Locations_Type3[i].position, Quaternion.identity);
            }

            i_cycleCount = 0;
        }
    }
}
