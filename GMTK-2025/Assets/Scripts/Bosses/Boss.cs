using UnityEngine;

public class Boss : Enemy
{
    [Header("Enemy Settings")]
    [SerializeField] private GameObject damageNumberPrefab; // Prefab for damage numbers
    [SerializeField] private float damageNumberSpawnRadius = 1f; // Radius around enemy to spawn damage numbers
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
        // Spawn damage number
        SpawnDamageNumber(damage);
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

    private void SpawnDamageNumber(float damageAmount)
    {
        if (damageNumberPrefab == null) return;

        // Generate random position around the enemy in a circle
        float randomAngle = Random.Range(0f, 360f);
        float randomRadius = Random.Range(0.5f, damageNumberSpawnRadius);

        Vector3 spawnOffset = new Vector3(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomRadius,
            Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomRadius,
            0f
        );

        Vector3 spawnPosition = transform.position + spawnOffset;

        // Instantiate the damage number
        GameObject damageNumberObj = Instantiate(damageNumberPrefab, spawnPosition, Quaternion.identity);
        
        // Set the sorting layer to UI to ensure it renders on top
        Canvas canvas = damageNumberObj.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingLayerName = "UI";
        }
        
        DamageNumber damageNumber = damageNumberObj.GetComponent<DamageNumber>();
        if (damageNumber != null)
        {
            damageNumber.SetDamageAmount(damageAmount);
            damageNumber.SetColor(Color.red);
        }
    }
}
