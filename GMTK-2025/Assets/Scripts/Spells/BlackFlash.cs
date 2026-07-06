using UnityEngine;
using System.Collections;

public class BlackFlash : Spell
{
    // [Header("Black Flash Settings")]
    // [SerializeField] private float range;
    // [Header("Black Flash Upgrades")]
    // [SerializeField] private float rangeUpgrade = 0.5f; // Range increase per upgrade
    // [SerializeField] private float damageUpgrade = 2;

    // void Start()
    // {
    //     Init();
    //     OrientSpell();
    //     AddUpgrade();
    //     DestroySpell();
    // }


    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Enemy"))
    //     {
    //         Enemy enemy = other.GetComponent<Enemy>();
    //         if (enemy != null)
    //         {
    //             enemy.TakeDamage(CalculateDamage(damage, spellType1, spellType2));
    //         }
    //     }
    // }

    // public void AddUpgrade()
    // {
    //     int spellLevel = GetSpellLevel(Spells.BlackFlash);
    //     damage += damageUpgrade * spellLevel; // Increase damage by the upgrade value
    //     range += rangeUpgrade * spellLevel; // Increase range by the upgrade value
    //     transform.localScale = new Vector3(range, range, 1f); // Scale the spell based on the new range
    // }

    // public void DestroySpell()
    // {
    //     Destroy(gameObject, destroyTime);
    // }

    [Header("Black Flash Settings")]
    public float force = 5f; // Base force applied to enemies
    [Header("Black Flash Upgrades")]
    public float damageUpgrade = 2f; // Damage increase per upgrade
    public float forceUpgrade = 1f; // Force increase per upgrade


    // Static variables
    public static float staticDamage = 0f; 
    public static float staticForce = 0f;

    void Start()
    {
        Init();
        OrientSpell();
        AddUpgrade();
        UpdateStaticVariables();
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.ApplyForce((transform.position - player.transform.position).normalized * force);
                playerMovement.SetBlackDashActive();
            }
        }

        Destroy(gameObject);
    }

    private void UpdateStaticVariables()
    {
        if (damage != staticDamage || force != staticForce)
        {
            staticDamage = damage;
            staticForce = force;
        }
    }

    public void AddUpgrade()
    {
        int spellLevel = GetSpellLevel(Spells.BlackFlash);
        damage += damageUpgrade * spellLevel; // Increase damage by the upgrade value
        force += forceUpgrade * spellLevel; // Increase force by the upgrade value
    }
}
