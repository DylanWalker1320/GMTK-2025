using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class DarkKnight : Boss
{
    private Vector2 movement;
    private Animator slamEffect;
    [SerializeField] private float attackRange;
    [SerializeField] private float dashForce;
    [SerializeField] private GameObject projectilePrefab; 

    private List<GameObject> activeProjectiles = new List<GameObject>();
    private float spriteOffset = 0.75f;

    void Start()
    {
        Init();

        slamEffect = GetComponentInChildren<Animator>();
        animator.SetBool("CanAttack", true);
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            agent.SetDestination(target.transform.position);
        }

        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.fixedDeltaTime;
        }
        else
        {
            Attack();
            attackCooldownTimer = attackCooldown;
        }
    }

    void Attack()
    {
        if (target != null)
        {
            int randomAttack = Random.Range(0, 2);
            animator.SetInteger("Attack", randomAttack);
        }
    }

    public void StartAttack()
    {
        animator.SetBool("CanAttack", false);
    }

    public void DoneAttack()
    {
        if (activeProjectiles.Count > 0){
            foreach (GameObject projectile in activeProjectiles)
            {
                if (projectile != null)
                {
                    projectile.GetComponent<BossProjectile>().TrackingState(); // Transition to Tracking state after spin ends
                }
            }

            activeProjectiles.Clear(); // Clear the list after transitioning to Tracking state
        }

        StartCoroutine(AttackCooldown());
    }

    public void DamageNearbyPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position + Vector3.right * spriteOffset, attackRange);
        foreach (Collider2D player in hits)
        {
            if (player.CompareTag("Player"))
            {
                player.GetComponent<PlayerMovement>().TakeDamage(stats.damage);
            }
        }
    }

    private void SpawnProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        
        // Get target location some distance away from boss at a random angle
        float distance = 5f; // Distance from boss to target location
        Vector2 direction = Random.insideUnitCircle.normalized; // Random direction
        Vector2 targetPosition = (Vector2)transform.position + direction * distance;

        projectile.GetComponent<BossProjectile>().Init(stats.damage / 2, targetPosition);
        activeProjectiles.Add(projectile);
    }

    private void Dash()
    {
        Vector2 direction = (target.transform.position - transform.position).normalized;
        rb.AddForce(direction * dashForce, ForceMode2D.Impulse);
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        animator.SetBool("CanAttack", true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>().TakeDamage(stats.damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.right * spriteOffset, attackRange);
    }
}