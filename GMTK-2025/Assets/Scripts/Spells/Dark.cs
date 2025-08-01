using UnityEngine;

public class Dark : Spell
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init(); // Initialize the spell properties
        OrientSpell(); // Spawn the spell at the reticle position
        StartCoroutine(DestroyAfter(destroyTime)); // Destroy the spell after a certain time
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SimpleEnemy enemy = collision.gameObject.GetComponent<SimpleEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
