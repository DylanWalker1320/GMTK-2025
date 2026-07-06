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
    public float aoeRange = 3f; // Area of effect range for the Black Flash
    public GameObject aoeVisual; // Visual representation for the area of effect
    [Header("Black Flash Upgrades")]
    public float damageUpgrade = 2f; // Damage increase per upgrade
    public float forceUpgrade = 1f; // Force increase per upgrade
    public float rangeUpgrade = 0.5f; // Range increase per upgrade


    // Static variables
    public static float staticDamage = 0f; 
    public static float staticForce = 0f;
    public static float staticRange = 0f;

    private GameObject player;
    private PlayerMovement playerMovement;

    void Start()
    {
        Init();
        OrientSpell();
        AddUpgrade();
        UpdateStaticVariables();
        
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.ApplyForce((transform.position - player.transform.position).normalized * force);
                playerMovement.SetBlackDashActive();
                StartCoroutine(WaitForDash());
            }
        }
    }

    private IEnumerator WaitForDash()
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        while (rb.linearVelocity.magnitude > playerMovement.maxSpeed) // While the dash is active (i.e., player is moving faster than max speed), wait
        {
            yield return null;
        }

        //TriggerAoE();
    }

    private void TriggerAoE()
    {
        if (aoeVisual != null)
        {
            aoeVisual.transform.position = player.transform.position;
            aoeVisual.transform.localScale = new Vector3(staticRange * 2, staticRange * 2, 1f);
            aoeVisual.SetActive(true);
        }

        // Implement the AoE effect here, e.g., damage nearby enemies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(player.transform.position, staticRange);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Enemy enemyComponent = enemy.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.TakeDamage(staticDamage * 1.5f);
                    enemyComponent.ApplyForce((enemy.transform.position - player.transform.position).normalized * staticForce);
                }
            }
        }

        Destroy(gameObject, 0.5f); // Destroy the Black Flash spell after the AoE effect
    }

    private void UpdateStaticVariables()
    {
        if (damage != staticDamage || force != staticForce || aoeRange != staticRange)
        {
            staticDamage = damage;
            staticForce = force;
            staticRange = aoeRange;
        }
    }

    public void AddUpgrade()
    {
        int spellLevel = GetSpellLevel(Spells.BlackFlash);
        damage += damageUpgrade * spellLevel; // Increase damage by the upgrade value
        force += forceUpgrade * spellLevel; // Increase force by the upgrade value
        aoeRange += rangeUpgrade * spellLevel; // Increase range by the upgrade value
    }
}
