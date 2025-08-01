using UnityEngine;

public class Ghostflame : Spell
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init(); // Initialize the spell properties
        OrientSpell(); // Orient the spell towards the mouse position and set its velocity
        Destroy(gameObject, destroyTime);;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            SimpleEnemy enemy = collision.GetComponent<SimpleEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(GetDamage());
            }
        }
    }
}
