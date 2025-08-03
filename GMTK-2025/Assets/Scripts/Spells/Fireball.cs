using UnityEngine;

public class Fireball : Spell
{
    [Header("Upgrade Scaling")]
    [SerializeField] private float damageUpgrade = 1f; // Damage increase per upgrade
    [SerializeField] private float speedUpgrade = 0.5f; // Speed increase per upgrade

    void Start()
    {
        Init(); // Initialize the spell properties
        OrientSpell(); // Orient the spell towards the mouse position and set its velocity
        AddUpgrade(); // Apply upgrades to the spell
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

    void AddUpgrade()
    {
        int spellLevel = GetSpellLevel(Spells.Fireball);
        damage += damageUpgrade * spellLevel; // Increase the damage based on the spell level
        speed += speedUpgrade * spellLevel; // Increase the speed based on the spell level
    }
}
