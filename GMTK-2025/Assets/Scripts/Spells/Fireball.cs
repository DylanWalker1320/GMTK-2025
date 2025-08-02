using UnityEngine;

public class Fireball : Spell
{
    void Start()
    {
        Init(); // Initialize the spell properties
        OrientSpell(); // Orient the spell towards the mouse position and set its velocity
        Destroy(gameObject, destroyTime);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        GameObject collisionObject = other.gameObject;
        if (collisionObject.CompareTag("Enemy"))
        {
            // If the fireball collides with an enemy, deal damage
            collisionObject.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject); // Destroy the fireball after dealing damage
        }
    }
}
