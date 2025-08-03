using UnityEngine;

public class Ghostflame : Spell
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Ghost Flame Properties")]
    [SerializeField] private float scaleSpeed = 0.1f; // Speed at which the spell scales up
    [Header("Upgrade Scaling")]
    [SerializeField] private float damageUpgrade = 1f; // Damage increase per upgrade
    [SerializeField] private float scaleSpeedUpgrade = 0.05f; // Scale speed increase per upgrade
    void Start()
    {
        Init(); // Initialize the spell properties
        OrientSpell(); // Orient the spell towards the mouse position and set its velocity
        Destroy(gameObject, destroyTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
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

    void Update()
    {
        // Continuously scale the spell based on the size upgrade
        float spellLevel = GetSpellLevel(Spells.Ghostflame);
        transform.localScale += new Vector3(scaleSpeed + scaleSpeedUpgrade * spellLevel, scaleSpeed + scaleSpeedUpgrade * spellLevel, 0) * Time.deltaTime;
    }
    
    void AddUpgrade()
    {
        int spellLevel = GetSpellLevel(Spells.Ghostflame);
        damage += damageUpgrade * spellLevel; // Increase the damage based on the upgrade level
    }
}