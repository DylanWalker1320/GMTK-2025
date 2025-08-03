using UnityEngine;
using System.Collections;

public class BlackHole : Spell
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Black Hole Properties")]
    [SerializeField] private float pullDelay = 5f; // Duration of the pull effect
    [SerializeField] private float pullStrength = 10f; // Strength of the pull effect
    [SerializeField] private float pullRadius = 5f; // Radius of the pull effect
    [SerializeField] private float damageInterval = 0.5f; // Interval between passive damage ticks

    [Header("Upgrade Scaling")]
    [SerializeField] private float pullStrengthUpgrade = 1f; // Pull strength increase per upgrade
    [SerializeField] private float pullRadiusUpgrade = 0.5f; // Pull radius increase per upgrade

    private float damageCooldown = 0.5f; // Cooldown for passive damage
    private float cooldownTimer = 0f; // Timer for cooldown

    void Start()
    {
        Init(); // Initialize the black hole properties
        OrientSpell(); // Orient the spell towards the target
        AddUpgrade(); // Apply upgrades to the black hole
        Throw(); // Throw the black hole

        StartCoroutine(PullDelay());
    }

    void Throw()
    {
        Vector2 direction = rb.linearVelocity.normalized;
        rb.AddForce(direction * speed, ForceMode2D.Impulse);
    }

    void Pull()
    {
        // Logic to pull objects towards the black hole
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pullRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                // Apply a force towards the black hole
                Vector2 direction = (transform.position - collider.transform.position).normalized;
                collider.GetComponent<Rigidbody2D>().AddForce(direction * pullStrength);
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (cooldownTimer <= 0f)
            {
                collision.GetComponent<Enemy>().TakeDamage(CalculateDamage(damage, spellType1, spellType2));
                cooldownTimer = damageCooldown;
            }
            else
            {
                cooldownTimer -= Time.deltaTime; // Decrease the cooldown timer
            }
        }
    }

    private IEnumerator PullDelay()
    {
        yield return new WaitForSeconds(pullDelay);
        Destroy(gameObject, destroyTime); // Destroy the black hole after a certain time
        Pull(); // Start pulling enemies towards the black hole
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("IsPulling", true); // Trigger the pulling animation
        }
    }
    
    void AddUpgrade()
    {
        int spellLevel = GetSpellLevel(Spells.BlackHole);
        pullStrength += pullStrengthUpgrade * spellLevel; // Increase the pull strength
        pullRadius += pullRadiusUpgrade * spellLevel; // Increase the pull radius
    }
}
