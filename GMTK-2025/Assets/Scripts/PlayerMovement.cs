using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveForce = 50f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float health;
    [SerializeField] private float invincibilityFrames = 1f; // Invincibility frames after taking damage
    [SerializeField] private GameObject damageNumberPrefab; // Prefab for damage numbers
    [SerializeField] private float damageNumberSpawnRadius = 1f; // Radius around player to spawn damage numbers
    public Transform reticle; // Reference to the reticle script for aiming
    public float experience;
    public Slider Healthbar;
    private SpriteRenderer playerSprite; // Reference to the player's sprite renderer for flipping
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool canCastMagic = true;
    private float invincibilityTimer = 0f; // Timer for invincibility frames
    

    void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        reticle = FindFirstObjectByType<Reticle>().GetComponent<Transform>();
        health = maxHealth;
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

    }

    void FixedUpdate()
    {

        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.fixedDeltaTime;
            if (invincibilityTimer <= 0f)
            {
                invincibilityTimer = 0f; // Reset timer
            }
        }

        // Only add force if under max speed
            if (rb.linearVelocity.magnitude < maxSpeed)
            {
                rb.AddForce(movement * moveForce);
            }

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

    public void TakeDamage(float damageAmount)
    {
        if (invincibilityTimer > 0f) return; // Ignore damage if invincibility frames are active

        StartCoroutine(HitEffect(Color.red, 0.5f));

        Debug.Log("Player took " + damageAmount + " damage");

        // Spawn damage number
        SpawnDamageNumber(damageAmount);

        invincibilityTimer = invincibilityFrames;

        health -= damageAmount;

        Healthbar.value = health;

        if(health <= maxHealth / 2)
            Debug.Log("Half HP");

        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator HitEffect(Color hitColor, float duration)
    {
        float elapsed = 0f;
        Color originalColor = playerSprite.color;

        // Fast fade to hit color
        playerSprite.color = hitColor;

        // Lerp back to original color over the duration
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            playerSprite.color = Color.Lerp(hitColor, originalColor, elapsed / duration);
            yield return null; // Wait for the next frame
        }

        playerSprite.color = originalColor;
    }

    private void SpawnDamageNumber(float damageAmount)
    {
        if (damageNumberPrefab == null) return;

        // Generate random position around the player in a circle
        float randomAngle = Random.Range(0f, 360f);
        float randomRadius = Random.Range(0.5f, damageNumberSpawnRadius);
        
        Vector3 spawnOffset = new Vector3(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomRadius,
            Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomRadius,
            0f
        );
        
        Vector3 spawnPosition = transform.position + spawnOffset;
        
        // Instantiate the damage number
        GameObject damageNumberObj = Instantiate(damageNumberPrefab, spawnPosition, Quaternion.identity);
        DamageNumber damageNumber = damageNumberObj.GetComponent<DamageNumber>();
        
        if (damageNumber != null)
        {
            damageNumber.SetDamageAmount(damageAmount);
            damageNumber.SetColor(Color.red);
        }
    }

    void Die()
    {
        // Handle enemy death (e.g., play animation, destroy object)
        Debug.Log("Game Over!");
        Destroy(gameObject);
    }
}
