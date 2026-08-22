using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossProjectile : MonoBehaviour
{
    private float damage;
    private Vector2 target;
    [SerializeField] private float speed = 5f;
    private State currentState;
    private static GameObject player;
    
    private Rigidbody2D rb;

    private enum State
    {
        Idle,
        Tracking
    }

    void Awake()
    {
        currentState = State.Idle;
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public void Init(float damage, Vector2 target)
    {
        this.damage = damage;
        this.target = target;

        Vector2 direction = (target - (Vector2)transform.position).normalized;
        
        rb.linearVelocity = direction * speed;
    }

    // Called from the boss after spin move ends
    public void TrackingState()
    {
        currentState = State.Tracking;
        target = player.transform.position; // Set target to player's current position

        // Set direction here so it doesn't change every frame, also lets the projectile continue moving in the same direction past the target
        Vector2 direction = (target - (Vector2)transform.position).normalized; 
        rb.linearVelocity = direction * speed * 2; // Move faster in Tracking state

        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (target == null)
        {
            return;
        }

        if (currentState == State.Idle && Vector2.Distance(transform.position, target) < 0.1f)
        {
            // If close to target while in Idle state, wait to transition to Tracking state
            rb.linearVelocity = Vector2.zero;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }
    }
}
