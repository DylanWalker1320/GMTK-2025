using UnityEngine;

public class Boss : Enemy
{
    [Header("Boss Settings")]
    [SerializeField] protected float maxHealth = 5000f;
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float maxMoveSpeed = 5f;
    [SerializeField] protected float attackCooldown = 1f;
    [SerializeField] protected float phaseChangeHealth = 2500f;
    [SerializeField] protected ParticleSystem deathEffect;

    [Header("Fight Monitor")]
    [SerializeField] protected float health = 5000f;

    protected float attackCooldownTimer = 0f;
    protected Animator animator;

    void Awake()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();
    }

    public override void TakeDamage(float damage)
    {
        // Handle boss taking damage
        health -= damage;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        // Handle boss death (e.g., play death animation, drop loot)
        Debug.Log("Boss defeated!");
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
