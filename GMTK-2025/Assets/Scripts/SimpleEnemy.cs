using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleEnemy : Enemy
{
    public string targetTag = "Player";
    private Rigidbody2D rb;
    private Transform target;
    private bool touchingTarget = false;
    private SpriteRenderer spriteRenderer;
    private EnemySpawner enemySpawner; // Reference to the enemy spawner
    private GameManager gameManager;
    [SerializeField] private NavMeshAgent agent;
    private AudioManager audioManager;
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private float maxHitSlowPercent = 0.2f; // 20% slow at max
    [SerializeField] private GameObject damageNumberPrefab; // Prefab for damage numbers
    [SerializeField] private float damageNumberSpawnRadius = 1f; // Radius around enemy to spawn damage numbers


    public EnemyStats stats;

    // Hit flash
    private float hitFlashTimer = 0f;
    private float hitFlashDuration = 0.2f;
    private static readonly int ColorProperty = Shader.PropertyToID("_Color");
    private MaterialPropertyBlock propertyBlock;
    private bool isDead = false;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        audioManager = FindFirstObjectByType<AudioManager>();
        enemySpawner = FindFirstObjectByType<EnemySpawner>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        propertyBlock = new MaterialPropertyBlock();
        agent = GetComponent<NavMeshAgent>();

        // Prevent NavMeshAgent from tilting or trying to use Y as up
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        
        InitStats(); // Scaling
        FindClosestTarget();    
    }

    void FindClosestTarget()
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

    void InitStats()
    {
        // health *= gameManager.loopsCompleted + 1;
        // damage = damage * gameManager.loopsCompleted / 1.5f;
        // maxSpeed += gameManager.loopsCompleted;

        //                                   v Scaling factor
        stats.health = stats.health * (1f + 0.75f * gameManager.loopsCompleted);
        stats.damage = stats.damage * (1f + 0.15f * gameManager.loopsCompleted);
        stats.speed = stats.speed * (1f + 0.25f * gameManager.loopsCompleted);

        agent.speed = stats.speed;
        agent.acceleration = stats.speed * 2f;
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;

        // Slow effect based on hit flash timer
        float slowMultiplier = 1f;
        if (hitFlashTimer > 0f)
        {
            float lerpT = Mathf.Clamp01(hitFlashTimer / hitFlashDuration);
            slowMultiplier = 1f - (maxHitSlowPercent * lerpT);
        }

        agent.SetDestination(target.position);

        spriteRenderer.flipX = direction.x > 0;

        if (touchingTarget)
        {
            target.GetComponent<PlayerMovement>().TakeDamage(stats.damage);
        }

        // Flash effect
        if (hitFlashTimer > 0f)
        {
            hitFlashTimer -= Time.fixedDeltaTime;

            float lerpT = Mathf.Clamp01(hitFlashTimer / hitFlashDuration);
            Color flashColor = Color.white;
            flashColor.a = lerpT;

            spriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(ColorProperty, flashColor);
            spriteRenderer.SetPropertyBlock(propertyBlock);
        }
        else
        {
            spriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(ColorProperty, new Color(1, 1, 1, 0));
            spriteRenderer.SetPropertyBlock(propertyBlock);
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

    public override void TakeDamage(float damage)
    {
        
        hitFlashTimer = hitFlashDuration;
        audioManager.Play("EnemyHurt");
        // Spawn damage number
        SpawnDamageNumber(damage);

        stats.health -= damage;
        if (stats.health <= 0f && isDead == false)
        {
            enemySpawner.currentEnemies--; // Debugging: Decrease enemy count in spawner
            isDead = true;
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Die();
        }

        // Knockback
        if (target != null)
        {
            Vector2 knockbackDirection = (transform.position - target.position).normalized;
            rb.AddForce(knockbackDirection * 5f, ForceMode2D.Impulse);
        }
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
            canvas.sortingLayerName = "DamageNumber";
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

    void Die() 
    {
        Destroy(gameObject);
    }
}