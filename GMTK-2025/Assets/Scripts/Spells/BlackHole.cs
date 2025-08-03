using UnityEngine;
using System.Collections;

public class BlackHole : Spell
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Black Hole Properties")]
    [SerializeField] private float pullDelay = 5f; // Duration of the pull effect
    [SerializeField] private float pullStrength = 10f; // Strength of the pull effect
    [SerializeField] private float pullRadius = 5f; // Radius of the pull effect
    [SerializeField] private float pullInterval = 0.2f; // Interval for pulling enemies towards the black hole
    [SerializeField] private float damageInterval = 0.5f; // Interval between passive damage ticks
    [SerializeField] private float size = 1f; // Size of the black hole

    [Header("Upgrade Scaling")]
    [SerializeField] private float damageUpgrade = 2f; // Damage increase per upgrade
    [SerializeField] private float pullStrengthUpgrade = 1f; // Pull strength increase per upgrade
    [SerializeField] private float sizeUpgrade = 0.2f; // Size increase per upgrade

    private float damageCooldown = 0.5f; // Cooldown for passive damage
    private float cooldownTimer = 0f; // Timer for cooldown
    private float pullCooldown = 0.5f; // Cooldown for pull effect
    private float pullTimer = 0f; // Timer for pull effect
    private bool isPulling = false;

    void Start()
    {
        Init(); // Initialize the black hole properties
        OrientSpell(); // Orient the spell towards the target
        AddUpgrade(); // Apply upgrades to the black hole
        Throw(); // Throw the black hole

        transform.localScale = new Vector3(size, size, 1f); // Set the scale of the black hole based on size
        StartCoroutine(PullDelay());
    }

    void Throw()
    {
        Vector2 direction = rb.linearVelocity.normalized;
        rb.AddForce(direction * speed, ForceMode2D.Impulse);
    }

    void Pull()
    {
        isPulling = true; // Set pulling state to true

        if (pullTimer <= 0f)
        {
            pullTimer = pullInterval; // Reset the pull timer
        } else
        {
            pullTimer -= Time.deltaTime; // Decrease the pull timer
        }

        // Logic to pull objects towards the black hole
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pullRadius);
        //Debug.DrawLine(transform.position, transform.position + Vector3.up * pullRadius, Color.red, 2f);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                // Apply a force towards the black hole
                Vector2 direction = (transform.position - collider.transform.position).normalized;
                //Debug.DrawLine(transform.position, collider.transform.position, Color.green, 2f);
                Debug.Log(collider.GetComponent<Rigidbody2D>());
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
        size += sizeUpgrade * spellLevel; // Increase the size
        damage += damageUpgrade * spellLevel; // Increase the damage
    }
    
    void Update()
    {
        if (isPulling) { Pull(); } // Continuously pull enemies if the black hole is active
    }
}
