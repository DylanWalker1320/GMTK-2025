using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class DarkKnight : Boss
{

    private Vector2 movement;
    private GameObject target;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator slamEffect;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player");
        sprite = GetComponent<SpriteRenderer>();
        slamEffect = GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector2 direction = (target.transform.position - transform.position).normalized;
            movement = direction * moveSpeed;
            rb.linearVelocity = new Vector2(movement.x, movement.y);
        }

        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.fixedDeltaTime;
        }
        else
        {
            // Implement attack logic here
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

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        StartCoroutine(HitEffect(Color.red, 0.2f));
    }

    private IEnumerator HitEffect(Color hitColor, float duration)
    {
        float elapsed = 0f;
        Color originalColor = sprite.color;

        // Fast fade to hit color
        sprite.color = hitColor;

        // Lerp back to original color over the duration
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            sprite.color = Color.Lerp(hitColor, originalColor, elapsed / duration);
            yield return null; // Wait for the next frame
        }

        sprite.color = originalColor;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Handle player collision (e.g., deal damage)
            other.GetComponent<PlayerMovement>().TakeDamage(10f);
        }
    }   
}