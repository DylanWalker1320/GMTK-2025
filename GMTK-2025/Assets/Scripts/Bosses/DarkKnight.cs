using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class DarkKnight : Boss
{
    private Vector2 movement;
    private Animator slamEffect;

    void Start()
    {
        slamEffect = GetComponentInChildren<Animator>();
        Init();
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

        StartCoroutine(AttackCooldown());
        animator.SetBool("CanAttack", false);
    }

    public void SlamAttack()
    {
        if (slamEffect != null)
        {
            slamEffect.Play("DarkKnight-SlamEffect");
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