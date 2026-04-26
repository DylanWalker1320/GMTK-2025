using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleEnemy : Enemy
{
    private bool touchingTarget = false;

    void Start()
    {
        Init();
    }

    void FixedUpdate()
    {
        if (target == null) return;

        direction = (target.position - transform.position).normalized;

        agent.SetDestination(target.position);

        spriteRenderer.flipX = direction.x > 0;

        if (touchingTarget)
        {
            target.GetComponent<PlayerMovement>().TakeDamage(stats.damage);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            touchingTarget = true;
        }        
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
            touchingTarget = false;        
    }
}