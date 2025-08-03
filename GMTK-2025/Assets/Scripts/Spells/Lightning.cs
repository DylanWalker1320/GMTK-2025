using UnityEngine;

public class Lightning : Spell
{
    [Header("Specific Spell Properties")]
    public float searchRadius = 5f; // Radius to find the closest enemy

    [Header("Upgrade Scaling")]
    [SerializeField] private float speedUpgrade = 1f; // Speed increase per upgrade
    [SerializeField] private float damageUpgrade = 1f; // Damage increase per upgrade
    [SerializeField] private float searchRadiusUpgrade = 0.5f; // Search radius increase per upgrade

    void Start()
    {
        Init(); // Initialize the spell properties
        AddUpgrade(); // Apply upgrades to the spell
        transform.position = GameObject.FindGameObjectWithTag("Player").transform.position;
        FindClosestEnemy(transform.position, searchRadius); // Use the radius variable
    }

    private void FindClosestEnemy(Vector2 position, float radius, int maxColliders = 50)
    {
        Collider2D[] hitColliders = new Collider2D[maxColliders];
        int colliderCount = Physics2D.OverlapCircleNonAlloc(position, radius, hitColliders);

        bool enemyFound = false;

        for (int i = 0; i < colliderCount; i++)
        {
            if (hitColliders[i].CompareTag("Enemy"))
            {

                enemyFound = true;
                Enemy enemy = hitColliders[i].GetComponent<Enemy>();
                if (enemy != null)
                {
                    Vector2 direction = (hitColliders[i].transform.position - transform.position).normalized;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle);

                    rb.linearVelocity = direction * speed; // Set the spell's velocity towards the enemy
                    //Debug.DrawLine(transform.position, hitColliders[i].transform.position, Color.red, 2f); // Debug line to visualize the spell's path
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
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    void AddUpgrade()
    {
        int spellLevel = GetSpellLevel(Spells.Lightning);
        speed += speedUpgrade * spellLevel; // Increase the speed
        damage += damageUpgrade * spellLevel; // Increase the damage
        searchRadius += searchRadiusUpgrade * spellLevel; // Increase the search radius
    }
}