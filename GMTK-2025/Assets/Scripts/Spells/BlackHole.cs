using UnityEngine;
using System.Collections;

public class BlackHole : Spell
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Black Hole Properties")]
    [SerializeField] private float pullDelay = 5f; // Duration of the pull effect
    [SerializeField] private float pullStrength = 10f; // Strength of the pull effect
    [SerializeField] private float pullRadius = 5f; // Radius of the pull effect
    [SerializeField] private float damageRadius = 3f; // Radius of the damage effect
    [SerializeField] private float damageInterval = 0.5f; // Interval between passive damage ticks
    [SerializeField] private SpriteRenderer damageVisual;

    private float lastDamageTime;
    private bool isPulling = false;
    
    void Start()
    {
        damageVisual.enabled = false; // Ensure the damage visual is initially hidden
        Init(); // Initialize the black hole properties
        OrientSpell(); // Orient the spell towards the target
        Throw(); // Throw the black hole

        StartCoroutine(PullDelay());
    }

    void Throw()
    {
        Vector2 direction = rb.linearVelocity.normalized;

        Debug.Log("Throwing Black Hole with speed: " + speed + " and direction: " + direction);
        rb.AddForce(direction * speed, ForceMode2D.Impulse);
    }

    void Pull()
    {
        // Logic to pull objects towards the black hole
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pullRadius / 6f); // Not sure why 6 works, load bearing coconut ig

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                // Apply a force towards the black hole
                Vector2 direction = (transform.position - collider.transform.position).normalized;
                collider.GetComponent<Rigidbody2D>().AddForce(direction * pullStrength);
            }
        }

        // Apply damage to enemies within the damage radius
        if (Time.time - lastDamageTime >= damageInterval)
        {
            Collider2D[] damageColliders = Physics2D.OverlapCircleAll(transform.position, damageRadius / 6f);
            foreach (Collider2D collider in damageColliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    // Apply damage to the enemy
                    collider.GetComponent<SimpleEnemy>().TakeDamage(damage);
                }
            }
            lastDamageTime = Time.time;
        }
    }

    private IEnumerator PullDelay()
    {
        yield return new WaitForSeconds(pullDelay);
        Destroy(gameObject, destroyTime); // Destroy the black hole after a certain time
        isPulling = true; // Start pulling after the delay

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f); // Make the black hole semi-transparent
        }

        Transform localTransform = transform;
        localTransform.localScale = new Vector3(pullRadius * 2, pullRadius * 2, 1f); // Scale the black hole to its pull radius
        damageVisual.enabled = true; // Show the damage visual
        damageVisual.transform.localScale = new Vector3(damageRadius / pullRadius, damageRadius / pullRadius, 1f); // Divide by pullRadius to convert to a relative scale
    }

    void Update()
    {
        if (rb == null) { return; }

        if (isPulling) { Pull(); }
    }
}
