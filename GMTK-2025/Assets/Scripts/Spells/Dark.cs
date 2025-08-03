using UnityEngine;

public class Dark : Spell
{
    [Header("Upgrade Scaling")]
    [SerializeField] private float damageUpgrade = 1f; // Damage increase per upgrade
    [SerializeField] private float speedUpgrade = 0.5f; // Speed increase per upgrade

    void Start()
    {
        Init(); // Initialize the spell properties
        OrientSpell(); // Spawn the spell at the reticle position
        AddUpgrade(); // Apply upgrades to the spell
        Destroy(gameObject, destroyTime); // Destroy the spell after a certain time
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    void AddUpgrade()
    {
        int spellLevel = GetSpellLevel(Spells.Dark);
        damage += spellLevel * damageUpgrade; // Increase the damage based on the spell level
        speed += spellLevel * speedUpgrade; // Increase the speed based on the spell level
    }
}