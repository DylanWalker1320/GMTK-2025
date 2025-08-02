using UnityEngine;

public class Wave : Spell
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            // If the fireball collides with an enemy, deal damage
            collisionObject.GetComponent<Enemy>().TakeDamage(GetDamage());
        }
    }
}
