using UnityEngine;

public class ChainLightning : Spell
{
    [Header("Specific Spell Properties")]
    [SerializeField] private float searchRadius = 5f; // Radius to find the closest enemy
    [SerializeField] private int chains = 3; // Number of chains for the lightning spell
    [Tooltip("Damage falloff per chain, e.g., 0.2 means 20% less damage per chain")]
    [SerializeField] private float falloffPerChain = 0.2f; // % Damage falloff per chain
    private GameObject lastTarget = null; // Store the last target hit by the lightning
    private int maxChains;

    void Start()
    {
        Init(); // Initialize the spell properties
        lastTarget = GameObject.FindWithTag("Player"); // Start with the player as the initial target
        maxChains = chains; // Store the initial number of chains

        FindClosestEnemy(transform.position, searchRadius); // Use the radius variable
    }

    private void FindClosestEnemy(Vector2 position, float radius, int maxColliders = 50)
    {
        Collider2D[] hitColliders = new Collider2D[maxColliders];
        int colliderCount = Physics2D.OverlapCircleNonAlloc(position, radius, hitColliders);

        bool enemyFound = false;

        for (int i = 0; i < colliderCount; i++)
        {
            if (hitColliders[i].CompareTag("Enemy") && hitColliders[i].gameObject != lastTarget)
            {
                enemyFound = true;
                Enemy enemy = hitColliders[i].GetComponent<Enemy>();
                if (enemy != null)
                {
                    Vector2 direction = (hitColliders[i].transform.position - transform.position).normalized;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle);

                    rb.linearVelocity = direction * speed; // Set the spell's velocity towards the enemy
                    break; // Exit after finding the first enemy
                }
            }
        }

        if (!enemyFound) { Destroy(gameObject); }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage * (1 - falloffPerChain * (maxChains - chains)));

                if (chains - 1 > 0)
                {
                    lastTarget = collision.gameObject;
                    chains--;
                }
                else
                {
                    Destroy(gameObject); // Destroy the spell if no more chains are left
                }

                // Find the next enemy to chain to
                FindClosestEnemy(collision.transform.position, searchRadius);
            }
        }
    }
}