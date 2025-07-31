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
    [SerializeField] private ParticleSystem deathEffect; // EXP death effect

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

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
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if(touchingPlayer)
        {
            player.GetComponent<TopDownPlayerMovement>().TakeDamage();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name + " (" + collision.gameObject.tag + ")");
        if (collision.gameObject.CompareTag("Spell"))
        {
            Debug.Log("Enemy hit by spell!");
            health -= 100f; // TODO: Grab damage value from spell
            if (health <= 0f)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
                Die();
            }
        }

        else if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("In contact with Player");
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

    void Die() 
    {
        // Handle enemy death (e.g., play animation, destroy object)
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }
}
