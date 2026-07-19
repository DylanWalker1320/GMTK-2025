using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class DarkKnight : Boss
{
    private Vector2 movement;
    private Animator slamEffect;
    public float attackRange = 1.5f; // Range for the slam attack

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

    public void SlamAttack()
    {
        if (slamEffect != null)
        {
            slamEffect.Play("DarkKnight-SlamEffect");
        }
    }

    public void StartAttack()
    {
        animator.SetBool("CanAttack", false);
    }

    public void DoneAttack()
    {
        StartCoroutine(AttackCooldown());
    }

    public void DamageNearbyPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D player in hits)
        {
            if (player.CompareTag("Player"))
            {
                player.GetComponent<PlayerMovement>().TakeDamage(stats.damage);
            }
        }
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
}