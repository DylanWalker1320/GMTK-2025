using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Boss : Enemy
{
    [Header("Enemy Settings")]
    [SerializeField] private GameObject damageNumberPrefab; // Prefab for damage numbers
    [SerializeField] private float damageNumberSpawnRadius = 1f; // Radius around enemy to spawn damage numbers
    [SerializeField] protected NavMeshAgent agent;

    private AudioManager audioManager;
    [Header("Boss Settings")]
    [SerializeField] protected float attackCooldown = 1f;
    [SerializeField] protected ParticleSystem deathEffect;
    [SerializeField] protected EnemyStats stats;
    [SerializeField] protected float phaseChangeHealth;
    protected float attackCooldownTimer = 0f;
    protected Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        phaseChangeHealth = stats.health / 2f;
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    public override void TakeDamage(float damage)
    {
        // Handle boss taking damage
        stats.health -= damage;
        // Spawn damage number
        SpawnDamageNumber(damage);
        if (stats.health <= 0f)
        {
            Die();
        }
    }

    void InitStats()
    {      
        var gameManager = FindFirstObjectByType<GameManager>();
        //                                  v Scaling factor
        stats.health = stats.health * (1f + 0.15f * gameManager.loopsCompleted);
        stats.damage = stats.damage * (1f + 0.15f * gameManager.loopsCompleted);
        stats.speed = stats.speed * (1f + 0.25f * gameManager.loopsCompleted);

        agent.speed = stats.speed;
        agent.acceleration = stats.speed * 2f;
    }

    void Die()
    {
        audioManager.Play("EnemyHurt");
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
            float t = Mathf.Clamp01(damageAmount / 100f); // Adjust 100f to your max expected damage
            Color gradientColor = Color.Lerp(Color.yellow, new Color(1f, 0.5f, 0f), t); // yellow to orange, interpolates between using t
            damageNumber.SetColor(gradientColor);
        }
    }
}
