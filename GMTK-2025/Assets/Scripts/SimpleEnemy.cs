using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleEnemy : MonoBehaviour
{
    [SerializeField] private float moveForce = 30f;
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private bool touchingPlayer = false;

    private Rigidbody2D rb;
    private Transform player;
    private float health = 100f; // Example health value
    private SpriteRenderer spriteRenderer; // Reference to the sprite renderer for flipping
    [SerializeField] private ParticleSystem deathEffect; // EXP death effect

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;

        // Only accelerate if below max speed
        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(direction * moveForce);
        }

        // Flip the enemy to face the player
        if (direction.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x < 0)
        {
            spriteRenderer.flipX = false;
        }

        if(touchingPlayer)
        {
            player.GetComponent<PlayerMovement>().TakeDamage();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            touchingPlayer = true;
            if (health <= 0f)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
                Die();
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            touchingPlayer = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Die();
        }
    }

    void Die() 
    {
        // Handle enemy death (e.g., play animation, destroy object)
        Destroy(gameObject);
    }
}
