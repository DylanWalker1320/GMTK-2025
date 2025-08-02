using UnityEngine;

public class PoisonPuddle : Spell
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init(); // Initialize the spell properties
        Destroy(gameObject, destroyTime); // Destroy the spell after a certain time
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(GetDamage());
            }
        }
    }
}
