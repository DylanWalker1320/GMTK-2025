using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
abstract public class Enemy : MonoBehaviour
{
    [System.Serializable]
    public struct EnemyStats
    {
        public float health;
        public float damage;
        public float speed;
    }

    protected Rigidbody2D rb;
    public string targetTag = "Player";
    protected Transform target;
    protected SpriteRenderer spriteRenderer;
    protected EnemySpawner enemySpawner; // Reference to the enemy spawner
    protected GameManager gameManager;
    protected AudioManager audioManager;
    protected Animator animator;
    protected UnityEngine.AI.NavMeshAgent agent;
    protected Vector2 direction;
    [SerializeField] protected ParticleSystem hitParticles;
    [SerializeField] protected ParticleSystem dropExperienceParticles;
    [SerializeField] protected ParticleSystem deathParticles;
    [SerializeField] protected float maxHitSlowPercent = 0.2f; // 20% slow at max
    [SerializeField] protected GameObject damageNumberPrefab; // Prefab for damage numbers
    [SerializeField] protected float damageNumberSpawnRadius = 1f; // Radius around enemy to spawn damage numbers
    [SerializeField] protected float healthScalar = 1f; // Base health scaler, can be adjusted for different enemy types
    [SerializeField] protected float damageScalar = 1f; // Base damage scaler, can be adjusted for different enemy types
    [SerializeField] protected float speedScalar = 1f; // Base speed scaler,
    public EnemyStats stats;

    // Hit flash
    protected float hitFlashTimer = 0f;
    protected float hitFlashDuration = 0.2f;
    protected static readonly int ColorProperty = Shader.PropertyToID("_Color");
    protected MaterialPropertyBlock propertyBlock;
    protected bool isDead = false;

    // Force cooldown
    [SerializeField] protected float applyForceCooldown = 1f; // Cooldown for applying force, to prevent physics issues
    public bool applyForceReady = true; // Flag to check if applying force is ready

    protected virtual void Init()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        audioManager = FindFirstObjectByType<AudioManager>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        propertyBlock = new MaterialPropertyBlock();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Initialize the property block so the hit flash shader starts transparent
        spriteRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(ColorProperty, new Color(1f, 1f, 1f, 0f));
        spriteRenderer.SetPropertyBlock(propertyBlock);

        // Prevent NavMeshAgent from tilting or trying to use Y as up
        agent.updateUpAxis = false;
        agent.updateRotation = false;
         
        stats.health = stats.health * (1f + healthScalar * gameManager.wavesCompleted);
        stats.damage = stats.damage * (1f + damageScalar * gameManager.wavesCompleted);
        stats.speed  = stats.speed  * (1f + speedScalar  * gameManager.wavesCompleted);

        agent.speed = stats.speed;
        agent.acceleration = stats.speed * 2f;

        FindClosestTarget();  
    }
    protected void FindClosestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (GameObject obj in targets)
        {
            float distance = Vector2.Distance(transform.position, obj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = obj.transform;
            }
        }

        target = closestTarget;
    }

    public virtual void TakeDamage(float damage, Color hitParticleColor = default)
    {
        
        hitFlashTimer = hitFlashDuration;
        StartCoroutine(HitFlash(Color.red, hitFlashDuration));
        audioManager.Play("EnemyHurt");


        // Spawn damage objects
        SpawnDamageNumber(damage);
        var main = hitParticles.main;
        main.startColor = hitParticleColor;
        Instantiate(hitParticles, transform.position, Quaternion.identity);

        stats.health -= damage;
        if (stats.health <= 0f && isDead == false)
        {
            Die();
        }

        // Knockback
        if (target != null)
        {
            Vector2 knockbackDirection = (transform.position - target.position).normalized;
            rb.AddForce(knockbackDirection * 5f, ForceMode2D.Impulse);
        }
    }

    private IEnumerator HitFlash(Color hitColor, float duration)
    {
        float elapsed = 0f;
        Color originalColor = Color.white;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float lerpT = Mathf.Clamp01(elapsed / duration);
            Color flashColor = hitColor;
            flashColor.a = 1f - lerpT;

            spriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(ColorProperty, flashColor);
            spriteRenderer.SetPropertyBlock(propertyBlock);

            yield return null;
        }

        // Ensure the color is reset at the end
        spriteRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(ColorProperty, new Color(1, 1, 1, 0));
        spriteRenderer.SetPropertyBlock(propertyBlock);
    }

    protected IEnumerator ForceCooldown()
    {
        applyForceReady = false;
        yield return new WaitForSeconds(applyForceCooldown);
        applyForceReady = true;
    }

    /// <summary>
    /// Applies an outside force to the enemy, used for the black hole. Has a short cooldown to prevent excessive forces being applied in a short time frame, which can cause physics issues.
    /// </summary>
    /// <param name="force">direction and magnitude of the force to apply</param>
    public void ApplyForce(Vector2 force)
    {
        if (applyForceReady)
        {
            rb.AddForce(force, ForceMode2D.Impulse);
            StartCoroutine(ForceCooldown());
        }
    }

    protected void SpawnDamageNumber(float damageAmount)
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
            canvas.sortingLayerName = "DamageNumber";
        }
        
        DamageNumber damageNumber = damageNumberObj.GetComponent<DamageNumber>();
        if (damageNumber != null)
        {
            damageNumber.SetDamageAmount(damageAmount);
            damageNumber.SetDamageObjectSize(damageAmount);
            float t = Mathf.Clamp01(damageAmount / 100f); // Adjust 100f to your max expected damage
            Color gradientColor = Color.Lerp(Color.yellow, new Color(1f, 0.5f, 0f), t); // yellow to orange, interpolates between using t
            damageNumber.SetColor(gradientColor);
        }
    }

    protected void Die()
    {
        isDead = true;
        Instantiate(dropExperienceParticles, transform.position, Quaternion.identity);
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        gameManager.EnemyKilled();
        Destroy(gameObject);
    }
}
