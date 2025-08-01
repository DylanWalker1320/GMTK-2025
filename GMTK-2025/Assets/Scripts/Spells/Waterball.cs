using UnityEngine;

public class Waterball : Spell
{
    void Start()
    {
        Init(); // Initialize the spell properties
        OrientSpell(); // Orient the spell towards the mouse position and set its velocity
        Destroy(gameObject, destroyTime);;
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        GameObject collisionObject = other.gameObject;
        if (collisionObject.CompareTag("Enemy"))
        {
            // If the waterball collides with an enemy, deal damage
            collisionObject.GetComponent<SimpleEnemy>().TakeDamage(damage);
            Destroy(gameObject); // Destroy the waterball after dealing damage
        }
    }
}
