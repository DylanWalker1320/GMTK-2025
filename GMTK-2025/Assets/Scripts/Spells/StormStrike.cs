using UnityEngine;

public class StormStrike : Spell
{
    [Header("Upgrade Scaling")]
    [SerializeField] private float damageUpgrade = 1f; // Damage increase per upgrade
    [SerializeField] private float speedUpgrade = 0.5f; // Speed increase per upgrade
    [SerializeField] private float sizeUpgrade = 0.2f; // Size increase per upgrade
    private float size = 1f; // Size of the strike

    void Start()
    {
        // Flip randomly to add variety
        if (Random.value > 0.5f)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }

        AddUpgrade(); // Apply upgrades to the spell
        transform.localScale = new Vector3(size, size, 1f); // Set the scale of the spell
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(CalculateDamage(damage, spellType1, spellType2)); // Deal damage to the enemy
            }
        }
    }

    void AddUpgrade()
    {
        int spellLevel = GetSpellLevel(Spells.Storm);
        damage += damageUpgrade * spellLevel; // Increase the damage
        speed += speedUpgrade * spellLevel; // Increase the speed
        size += sizeUpgrade * spellLevel; // Increase the size
    }
}
