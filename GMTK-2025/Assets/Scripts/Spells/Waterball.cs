using UnityEngine;

public class Waterball : Spell
{
    [Header("Upgrade Scaling")]
    [SerializeField] private float damageUpgrade = 1f; // Damage increase per upgrade
    [SerializeField] private float speedUpgrade = 0.5f; // Speed increase per upgrade

    void Start()
    {
        Init(); // Initialize the spell properties
        OrientSpell(); // Orient the spell towards the mouse position and set its velocity
        Destroy(gameObject, destroyTime); // Destroy the waterball after a certain time
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        GameObject collisionObject = other.gameObject;
        if (collisionObject.CompareTag("Enemy"))
        {
            // If the waterball collides with an enemy, deal damage
            collisionObject.GetComponent<Enemy>().TakeDamage(CalculateDamage(damage, spellType1, spellType2), damageColor);
            Destroy(gameObject); // Destroy the waterball after dealing damage
        }
        else if (collisionObject.CompareTag("Obstacles") || collisionObject.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }
    }

    void AddUpgrade()
    {
        int spellLevel = GetSpellLevel(Spells.Waterball);
        damage += damageUpgrade * spellLevel; // Increase the damage based on the spell level
        speed += speedUpgrade * spellLevel * 0.5f;
    }
}
