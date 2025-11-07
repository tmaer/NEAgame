using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;

/*
 * TO-DO
 * make injured skin
 * 
 * NEXT:
 * make the switch which angers and creates more monsters?
 * If got time make list of classes for monster
 */

public class Monster : NetworkBehaviour
{
    // Variables
    public int health;
    private readonly float knifeHitCooldown = 1f;
    private float lastHitTime;

    public float moveSpeed;
    public float collisionDistance;
    private readonly float separationStrength = 5f;
    private bool dashing = false;

    private Coroutine dashCheck;
    private readonly float dashSpeed = 75f;
    private readonly float dashDuration = .75f;
    private readonly float dashCooldown = 3f;
    private readonly float attackSpeed = 1.6f;
    private readonly float attackDistance = 5f;
    private float attackDuration;

    private readonly float curiosityStrikeDistance = 9f;
    private float curiosityTimer;
    private float curiosityReStrike = 0;
    // GameObjects
    private GameObject player;
    // GameObject accessors
    private Rigidbody2D rigidBody;
    private SpriteRenderer sprite;
    // Other files
    public Sprite monster;
    public Sprite angyMonster;
    public Sprite injuredMonster; // MAKE IT LIKE 20-30% OF HEALTH SHOW

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rigidBody = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        float randomScale = Random.Range(0.021f, 0.21f);
        transform.localScale = new Vector3(randomScale, randomScale, 0);
        collisionDistance = randomScale * 7f;
        moveSpeed = Mathf.Lerp(2.5f, .5f, Mathf.InverseLerp(0.021f, 0.21f, randomScale));
        health += Mathf.RoundToInt(Mathf.Lerp(20f, 200f, Mathf.InverseLerp(0.021f, 0.21f, randomScale)));

        curiosityTimer = Time.time - curiosityReStrike;

        PushAwayNearbyMonsters();
    }

    private void PushAwayNearbyMonsters()
    {
        Collider2D[] nearbyMonsters = Physics2D.OverlapCircleAll(transform.position, collisionDistance);
        foreach (Collider2D collider in nearbyMonsters)
        {
            if (collider.gameObject != gameObject && collider.CompareTag("Monster"))
            {
                Vector3 direction = transform.position - collider.transform.position;
                direction.Normalize();

                // Apply a small push force to move the monster away
                rigidBody.AddForce(direction * separationStrength, ForceMode2D.Impulse);
            }
        }
    }

    public IEnumerator Attack() // on touch does 10 dmg
    {
        yield return new WaitForSeconds(1f);

        float attackStartTime = Time.time;
        float previousMoveSpeed = moveSpeed;

        attackDuration = Random.Range(6f, 15f);
        sprite.sprite = angyMonster;
        moveSpeed = attackSpeed;

        while (Time.time < attackStartTime + attackDuration)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer < attackDistance && dashCheck == null)
            {
                dashCheck = StartCoroutine(DashAttack()); // dash when in range for 1 second
            }
            yield return null;
        }

        sprite.sprite = monster;
        moveSpeed = previousMoveSpeed;
        // Debug.Log("Attack finished!");
    }

    public IEnumerator DashAttack() // a hit does 15/20 dmg
    {
        // Debug.Log("Dash attack!");
        dashing = true;
        Vector3 dashDirection = (player.transform.position - transform.position).normalized;
        float dashStartTime = Time.time;

        // Dashes at last player position for dashDuration
        while (Time.time < dashStartTime + dashDuration)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDirection, 0.1f, LayerMask.GetMask("Map"));
            if (hit.collider == null)
            {
                rigidBody.MovePosition(transform.position + dashSpeed * Time.deltaTime * dashDirection);
            }
            else
            {
                rigidBody.velocity = Vector2.zero;
                break;  // Stop dash if there's a wall
            }
            yield return null;
        }

        rigidBody.velocity = Vector3.zero;
        dashing = false;

        yield return new WaitForSeconds(dashCooldown);
        dashCheck = null;
    }

    private void FixedUpdate()
    {
        if (health <= 0)
        {
            // Destroy(gameObject);
        }

        if (!dashing)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;

            Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, collisionDistance);
            Vector3 separationForce = Vector3.zero;

            // Detecting game objects nearby
            foreach (Collider2D collider in nearbyObjects)
            {
                if (collider != null && collider.gameObject != gameObject)
                {
                    if (collider.CompareTag("Monster") || collider.CompareTag("Player"))
                    {
                        Vector3 difference = transform.position - collider.transform.position;
                        separationForce += difference.normalized / difference.magnitude;
                    }
                    else if (collider.CompareTag("Bullet"))
                    {
                        health -= 15;
                        Destroy(collider.gameObject);
                        Debug.Log("Health: " + health);
                    }
                    else if (collider.CompareTag("PlayerKnife"))
                    {
                        Knife knifeScript = collider.gameObject.GetComponent<Knife>();
                        if (knifeScript.isStabbing && Time.time >= lastHitTime + knifeHitCooldown)
                        {
                            health -= 30;
                            lastHitTime = Time.time;
                            Debug.Log("Health: " + health);
                        }
                    }
                }
            }

            // Move monster towards the player
            Vector3 finalDirection = (direction + separationStrength * separationForce).normalized;
            rigidBody.MovePosition(transform.position + moveSpeed * Time.deltaTime * finalDirection);

            // Face the monster towards the player
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            // Checks distance of player and on chance attacks
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < curiosityStrikeDistance && Time.time >= curiosityTimer + curiosityReStrike)
            {
                curiosityTimer = Time.time;
                if (Random.Range(1, 6) == 1)
                {
                    StartCoroutine(Attack());
                    curiosityReStrike = 30;
                }
                else
                {
                    curiosityReStrike = 5;
                }
            }
        }
    }
        
    private void OnDrawGizmosSelected()
    {
        // Draws virtual circle to detect other monsters
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, collisionDistance);
    }
}