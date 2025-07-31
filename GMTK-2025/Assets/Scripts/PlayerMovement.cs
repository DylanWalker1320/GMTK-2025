using System.Threading;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownPlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveForce = 50f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float maxMagicBallCooldown;
    [SerializeField] private bool canCastMagic = true;
    [SerializeField] private GameObject magicBallPrefab; // Temporary placeholder until magic system is properly implemented
    [SerializeField] private Transform reticle; // Reference to the reticle script for aiming
    [SerializeField] private float health = 100f;

    public Slider Healthbar;
    private float magicBallCooldown;
    private SpriteRenderer playerSprite; // Reference to the player's sprite renderer for flipping
    private Rigidbody2D rb;
    private Vector2 movement;

    void Awake()
    {
        magicBallCooldown = maxMagicBallCooldown; // Initialize cooldown
        playerSprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        reticle = FindFirstObjectByType<Reticle>().GetComponent<Transform>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

    }

    void FixedUpdate()
    {
        // Only add force if under max speed
        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(movement * moveForce);
        }
        CastMagic();

        // Flip the player to face the movement direction
        if (movement.x > 0)
        {
            playerSprite.flipX = true;
        }
        else if (movement.x < 0)
        {
            playerSprite.flipX = false;
        }
    }

    void CastMagic()
    {
        magicBallCooldown -= Time.deltaTime;
        if (canCastMagic)
        {
            canCastMagic = false;
            Instantiate(magicBallPrefab, reticle.GetChild(0).position, Quaternion.Euler(0f, 180f, 0f));
        }
        else if (magicBallCooldown <= 0f)
        {
            magicBallCooldown = maxMagicBallCooldown; // Reset cooldown
            canCastMagic = true;
        }
    }

    public void TakeDamage()
    {
        health -= 1;

        Healthbar.value = health;

        if(health <= 50)
            Debug.Log("Half HP");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Handle enemy death (e.g., play animation, destroy object)
        Debug.Log("Game Over!");
        Destroy(gameObject);
    }
}
