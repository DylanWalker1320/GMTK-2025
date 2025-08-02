using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleEnemy : MonoBehaviour
{
    public float moveForce = 30f;
    public float maxSpeed = 3f;
    public float damage = 10f;
    public string targetTag = "Player";
    private Rigidbody2D rb;
    private Transform target;
    private bool touchingTarget = false;
    private float health = 100f;
    private SpriteRenderer spriteRenderer;
    private EnemySpawner enemySpawner; // Reference to the enemy spawner
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private float maxHitSlowPercent = 0.2f; // 20% slow at max


    // Hit flash
    private float hitFlashTimer = 0f;
    private float hitFlashDuration = 0.2f;
    private static readonly int ColorProperty = Shader.PropertyToID("_Color");
    private MaterialPropertyBlock propertyBlock;

    void Start()
    {
        enemySpawner = FindFirstObjectByType<EnemySpawner>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        propertyBlock = new MaterialPropertyBlock();

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

        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(direction * moveForce * slowMultiplier);
        }

        spriteRenderer.flipX = direction.x > 0;

        if (touchingTarget)
        {
            target.GetComponent<PlayerMovement>().TakeDamage(damage);
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

    public void TakeDamage(float damage)
    {
        hitFlashTimer = hitFlashDuration;

        health -= damage;
        if (health <= 0f)
        {
            enemySpawner.currentEnemies--; // Debugging: Decrease enemy count in spawner
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Die();
        }
    }

    void Die() 
    {
        Destroy(gameObject);
    }
}