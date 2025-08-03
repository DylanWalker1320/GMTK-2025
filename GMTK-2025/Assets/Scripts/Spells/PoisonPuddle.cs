using UnityEngine;

public class PoisonPuddle : Spell
{
    [Header("Upgrade Scaling")]
    [SerializeField] private float damageUpgrade = 1f; // Damage increase per upgrade
    [SerializeField] private float durationUpgrade = 0.5f; // Duration increase per upgrade

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

    void AddUpgrade()
    {
        int spellLevel = GetSpellLevel(Spells.PoisonPuddle);
        SetDamage(GetDamage() + damageUpgrade * spellLevel); // Increase the damage based on upgrades
        destroyTime += durationUpgrade * spellLevel; // Increase the duration of the poison puddle
    }
}
