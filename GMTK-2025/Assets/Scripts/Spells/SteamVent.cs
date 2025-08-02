using UnityEngine;

public class SteamVent : Spell
{
    [Header("Steam Vent Settings")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float damageCooldown = 0.5f; // Time between damage applications
    [SerializeField] private float range = 5f; // Damage dealt to enemies
    private float damageCooldownTimer = 0f;

    void Start()
    {
        Init();
        OrientSpell(); // Ensure the spell is pointing toward the reticle

        transform.Rotate(0, 0, 90f);
        transform.localScale = new Vector3(range * 0.6f, range, 1f); // Scale the spell to the desired range
        transform.position += transform.up * -range / 2; // Adjust position to avoid immediate collision

        Destroy(gameObject, destroyTime);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (damageCooldownTimer <= 0f)
                {
                    enemy.TakeDamage(damage);
                    damageCooldownTimer = damageCooldown;
                }

                //Also apply a knockback effect
                Vector2 knockbackDirection = gameObject.transform.up * -1; // Knockback in the opposite direction of the spell's orientation
                Rigidbody2D enemyRb = collision.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }

    void Update()
    {
        damageCooldownTimer -= Time.deltaTime; // Update the cooldown timer
    }
}
