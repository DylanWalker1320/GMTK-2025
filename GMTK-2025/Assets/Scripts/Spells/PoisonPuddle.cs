using UnityEngine;

public class PoisonPuddle : Spell
{
    [Header("Poison Puddle Properties")]
    [SerializeField] private float damageCooldown = 0.5f; // Cooldown for damage application

    [Header("Upgrade Scaling")]
    [SerializeField] private float damageUpgrade = 1f; // Damage increase per upgrade
    [SerializeField] private float durationUpgrade = 0.5f; // Duration increase per upgrade
        
    private float damageCooldownTimer = 0f; // Timer to track damage cooldown

    void Start()
    {
        Init(); // Initialize the spell properties
        AddUpgrade(); // Apply upgrades to the spell
        Destroy(gameObject, destroyTime); // Destroy the spell after a certain time
    }

    void OnTriggerStay2D(Collider2D collision)
    {

        damageCooldownTimer += Time.deltaTime; // Increment the cooldown timer
        if (damageCooldownTimer < damageCooldown)
            return; // If cooldown is not over, do nothing
        
        damageCooldownTimer = 0f; // Reset the cooldown timer

        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(CalculateDamage(damage, spellType1, spellType2));
            }
        }
    }

    void AddUpgrade()
    {
        int spellLevel = GetSpellLevel(Spells.PoisonPuddle);
        damage += damageUpgrade * spellLevel; // Increase the damage based on upgrades
        destroyTime += durationUpgrade * spellLevel; // Increase the duration of the poison puddle
    }
}
