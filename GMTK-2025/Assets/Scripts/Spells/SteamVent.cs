using UnityEngine;

public class SteamVent : Spell
{
    [Header("Steam Vent Settings")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float damageCooldown = 0.5f; // Time between damage applications
    [SerializeField] private float range = 1f; // Damage dealt to enemies

    [Header("Upgrade Scaling")]
    [SerializeField] private float damageUpgrade = 1f; // Damage increase per upgrade
    [SerializeField] private float rangeUpgrade = 0.2f; // Range increase per upgrade

    private float damageCooldownTimer = 0f;

    void Start()
    {
        Init();
        OrientSpell(); // Ensure the spell is pointing toward the reticle
        AddUpgrade(); // Apply upgrades to the spell

        transform.Rotate(0, 0, 90f);
        transform.localScale = new Vector3(range, range, 1f); // Scale the spell to the desired range

        Destroy(gameObject, destroyTime);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (damageCooldownTimer <= 0f)
                {
                    enemy.TakeDamage(CalculateDamage(damage, spellType1, spellType2));
                    damageCooldownTimer = damageCooldown;
                }

                //Also apply a knockback effect
                Vector2 knockbackDirection = gameObject.transform.up * -1; // Knockback in the opposite direction of the spell's orientation
                Rigidbody2D enemyRb = collision.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }

    void Update()
    {
        damageCooldownTimer -= Time.deltaTime; // Update the cooldown timer
    }

    private void AddUpgrade()
    {
        int spellLevel = GetSpellLevel(Spells.SteamVent);
        damage += damageUpgrade * spellLevel; // Increase the damage based on the spell level
        range += rangeUpgrade * spellLevel; // Increase the range based on the spell level
    }
}
