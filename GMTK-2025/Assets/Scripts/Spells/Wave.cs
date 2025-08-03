using UnityEngine;

public class Wave : Spell
{
    [Header("Wave Properties")]
    [SerializeField] private GameObject visuals;
    [Header("Upgrade Scaling")]
    [SerializeField] private float damageUpgrade = 1f; // Damage increase per upgrade
    [SerializeField] private float speedUpgrade = 0.5f; // Speed increase per upgrade
    [SerializeField] private float sizeUpgrade = 0.2f; // Size increase per upgrade
    
    void Start()
    {
        Init(); // Initialize the spell properties
        OrientSpell(); // Orient the spell towards the mouse position and set its velocity
        AddUpgrade(); // Apply upgrades to the spell

        // If travelling right, flip the visuals
        // Debug.DrawRay(transform.position, rb.linearVelocity, Color.red, 2f);
        if (rb.linearVelocity.x > 0)
        {
            visuals.transform.localScale = new Vector3(visuals.transform.localScale.x, visuals.transform.localScale.y * -1, visuals.transform.localScale.z);
        }

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
