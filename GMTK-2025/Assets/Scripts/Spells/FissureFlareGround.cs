using UnityEngine;

public class FissureFlareGround : Spell
{
    [Header("Upgrade Scaling")]
    [SerializeField] private float damageUpgrade = 0.5f; // Damage increase per upgrade
    [SerializeField] private float durationUpgrade = 0.5f; // Duration increase per upgrade

    void Start()
    {
        AddUpgrade(); // Apply upgrades to the spell
        Destroy(gameObject, destroyTime); // Destroy the ground effect after 5 seconds
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Apply damage to the enemy
            }
        }
    }

    void AddUpgrade()
    {
        // Increase damage and duration based on spell level
        int spellLevel = GetSpellLevel(Spells.FissureFlare);
        damage += damageUpgrade * spellLevel;
        destroyTime += durationUpgrade * spellLevel;
    }
}
