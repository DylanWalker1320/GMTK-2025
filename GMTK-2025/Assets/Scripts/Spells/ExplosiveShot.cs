using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplosiveShot : Spell
{
    [Header("Explosive Shot Properties")]
    [SerializeField] private float explosionSize = 1f; // Size multiplier of the explosion
    [SerializeField] private float explosionDuration = 5f; // Duration for the explosion visual to fade out
    [SerializeField] private float explosionDamageTickRate = 0.2f; // How often the explosion applies damage to enemies within its radius
    [Header("Upgrade Scaling")]
    [SerializeField] private float explosionDamageUpgrade = 2f; // Damage increase per upgrade
    [SerializeField] private float explosionSizeUpgrade = 0.5f;
    [SerializeField] private float explosionDurationUpgrade = 0.2f; // Duration increase per upgrade
    private Animator animator;
    private bool isExploding = false;
    private float damageTickTimer = 0f;
    private List<Enemy> enemiesInRange = new List<Enemy>(); // Track enemies within the explosion radius
    
    void Start()
    {
        // Initialize the spell properties
        Init(); 
        OrientSpell();
        animator = GetComponent<Animator>();
        transform.localScale = new Vector3(explosionSize, explosionSize, 1f); // Set the scale of the spell based on explosion size

        StartCoroutine(ExplodeDelay()); // Start the coroutine to handle the explosion delay

        AddUpgrade(); // Apply upgrades to the spell
    }

    void Update()
    {
        if (isExploding)
        {
            damageTickTimer += Time.deltaTime;
            if (damageTickTimer >= explosionDamageTickRate)
            {
                damageTickTimer = 0f; // Reset the timer
                DoDamage(); // Apply damage to enemies in range at the specified tick rate
            }
        }
    }

    void Explode()
    {
        if (isExploding) return; // Prevent multiple explosions

        rb.linearVelocity = Vector2.zero; // Stop the spell's movement
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.radius = 1.3f; // 1.3 looks best and will scale
        }

        animator.SetBool("Exploded", true); // Trigger the explosion animation
        isExploding = true;
        Destroy(gameObject, explosionDuration); // Destroy the spell after a certain time
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (!isExploding)
            {
                Explode();
                Debug.Log("ExplosiveShot collided with Enemy and exploded.");
            }

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (!enemiesInRange.Contains(enemy))
                {
                    enemiesInRange.Add(enemy);
                }
            }
        }
        else if (collision.gameObject.CompareTag("Obstacles") || collision.gameObject.CompareTag("Walls"))
        {
            if (!isExploding)
            {
                Explode();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null && enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy); // Remove the enemy from the list when it exits the explosion radius
            }
        }
    }

    void DoDamage()
    {
        List<Enemy> enemiesToDamage = new List<Enemy>(enemiesInRange); // Create a copy of the list to avoid modification issues during iteration

        enemiesToDamage.RemoveAll(enemy => enemy == null); // Remove any null entries from the list

        foreach (Enemy enemy in enemiesToDamage)
        {
            enemy.TakeDamage(CalculateDamage(damage, spellType1, spellType2));
        }
    }

    private IEnumerator ExplodeDelay()
    {
        yield return new WaitForSeconds(destroyTime); // Wait for the explosion duration
        Explode(); // Call the explode method to destroy the spell
    }

    void AddUpgrade()
    {
        int spellLevel = GetSpellLevel(Spells.ExplosiveShot);
        damage += explosionDamageUpgrade * spellLevel;
        explosionSize += explosionSizeUpgrade * spellLevel;
        explosionDuration += explosionDurationUpgrade * spellLevel;
    }
}
