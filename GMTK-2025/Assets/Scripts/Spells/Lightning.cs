using UnityEngine;

public class Lightning : Spell
{
    [Header("Specific Spell Properties")]
    public float searchRadius = 5f; // Radius to find the closest enemy

    void Start()
    {
        Init(); // Initialize the spell properties

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
                DestroyAfter(destroyTime);
                SimpleEnemy enemy = hitColliders[i].GetComponent<SimpleEnemy>();
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
            SimpleEnemy enemy = collision.GetComponent<SimpleEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}