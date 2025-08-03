using UnityEngine;

public class Wave : Spell
{
    [Header("Upgrade Scaling")]
    [SerializeField] private float damageUpgrade = 1f; // Damage increase per upgrade
    [SerializeField] private float speedUpgrade = 0.5f; // Speed increase per upgrade
    [SerializeField] private float sizeUpgrade = 0.2f; // Size increase per upgrade
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
            collisionObject.GetComponent<Enemy>().TakeDamage(CalculateDamage(damage, spellType1, spellType2));
        }
    }
    
    void AddUpgrade()
    {
        int spellLevel = GetSpellLevel(Spells.Wave);
        damage += damageUpgrade * spellLevel; // Increase the damage
        speed += speedUpgrade * spellLevel; // Increase the speed
        transform.localScale += new Vector3(sizeUpgrade * spellLevel, sizeUpgrade * spellLevel, 0); // Increase the size of the wave
    }
}
