using UnityEngine;
using System.Collections;

public class ExplosiveShot : Spell
{
    [Header("Explosive Shot Properties")]
    [SerializeField] private float explosionSize = 1f; // Size multiplier of the explosion
    [SerializeField] private float explosionDuration = 1f; // Duration for the explosion visual to fade out
    [Header("Upgrade Scaling")]
    [SerializeField] private float explosionDamageUpgrade = 2f; // Damage increase per upgrade
    [SerializeField] private float explosionSizeUpgrade = 0.5f;
    [SerializeField] private float explosionDurationUpgrade = 0.2f; // Duration increase per upgrade
    private Animator animator;
    private bool isExploding = false;
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

    void Explode()
    {
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
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (!isExploding)
                {
                    Explode();
                }

                enemy.TakeDamage(CalculateDamage(damage, spellType1, spellType2));
            }
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
