
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Player Stats")]
    public int level = 1;
    public float moveForce = 50f;
    public float maxSpeed = 5f;
    public float maxHealth = 100f;
    public float castSpeed;
    public float castStrength;
    public float health;
    public int invincibilityFrames = 1; // Invincibility frames after taking damage
    public float dashStrength; // Strength of the dash
    [Header("Currency")]
    public int souls;
    [Header("Experience")]
    public float experience;
    public float nextLevelExperience = 50f;
    public float newExperiencePerLevel;
    public float experiencePerLevelMultiplier;
    private bool isGainingExperience = false;
    private float experiencePitchTimer = 0f;
    [SerializeField] private float experiencePitchChangeInterval = 0.5f;
    // public float experiencePerSoul = 1f; // could be used as a stat modifier where players gain more experience per soul collected
    [Header("UI Elements")]
    [SerializeField] private GameObject damageNumberPrefab; // Prefab for damage numbers
    [SerializeField] private float damageNumberSpawnRadius = 1f; // Radius around player to spawn damage numbers
    public Transform reticle; // Reference to the reticle script for aiming
    public UnityEvent<float, float> updateHealthUI;
    [Header("Movement/Animation")]
    public Vector2 movement;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator shadowAnimator;
    public bool facingRight = true;
    private SpriteRenderer playerSprite; // Reference to the player's sprite renderer for flipping
    private Rigidbody2D rb;
    private AudioManager audioManager;
    private UIManager uiManager;
    private float invincibilityTimer = 0f; // Timer for invincibility frames
    public bool blackDashActive = false; // Flag to check if Black Dash is active


    void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        audioManager = FindFirstObjectByType<AudioManager>();
        uiManager = FindFirstObjectByType<UIManager>();
        rb = GetComponent<Rigidbody2D>();
        reticle = FindFirstObjectByType<Reticle>().GetComponent<Transform>();
        health = maxHealth;
        experiencePitchTimer = experiencePitchChangeInterval;
    }

    void Start()
    {
        updateHealthUI.Invoke(health, maxHealth);
    }

    void Update()
    {

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        if (Input.GetKeyDown(KeyCode.Space) && uiManager.isInUI == false)
        {
            // Dash
            ApplyForce(movement * dashStrength);
        }

        if(isGainingExperience)
        {
            experiencePitchTimer -= Time.deltaTime;
            if(experiencePitchTimer <= 0f)
            {
                isGainingExperience = false;
            }
        }
    }

    public void ApplyForce(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
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

        // Animator update
        if (animator != null)
        {
            animator.SetBool("IsMoving", movement.magnitude > 0.1f);
            shadowAnimator.SetBool("IsMoving", movement.magnitude > 0.1f);
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
            facingRight = true;
        }
        else if (movement.x < 0)
        {
            playerSprite.flipX = false;
            facingRight = false;
        }
    }

    public void GainExperience()
    {
        if(!isGainingExperience)
        {
            audioManager.Play("ExperienceParticle");
            audioManager.SetPitch("ExperienceParticle", 1f); // Reset pitch after interval
            isGainingExperience = true;
        }
        else
        {
            audioManager.Play("ExperienceParticle");
            audioManager.IncreasePitch("ExperienceParticle", 0.01f); // Increase pitch for experience gain sound
        }
        experiencePitchTimer = experiencePitchChangeInterval; // Reset timer
        

        experience += 1;
        souls += 1;

        if (experience >= nextLevelExperience)
        {
            level++;
            experience -= nextLevelExperience;
            nextLevelExperience = Mathf.Round(nextLevelExperience + newExperiencePerLevel * experiencePerLevelMultiplier);
            newExperiencePerLevel *= experiencePerLevelMultiplier;
            audioManager.Play("LevelUp!");
            FindAnyObjectByType<UIManager>().SetActiveScrollUI();
        }
        uiManager.UpdateExperienceUI(experience, nextLevelExperience, level, souls);
        
    }

    public void TakeDamage(float damageAmount)
    {
        if (invincibilityTimer > 0f) return; // Ignore damage if invincibility frames are active
        damageAmount = Mathf.Round(damageAmount);

        CinemachineShake.Instance.ShakeCamera(3f, .1f);

        StartCoroutine(HitEffect(Color.red, 0.5f));
        audioManager.Play("PlayerHurt");

        // Spawn damage number
        SpawnDamageNumber(damageAmount, Color.red);

        invincibilityTimer = invincibilityFrames;
        health -= damageAmount;

        UpdateUI();

        if (health <= 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        health = Mathf.Min(health + healAmount, maxHealth);
        UpdateUI();

        SpawnDamageNumber(healAmount, Color.green);

        //StartCoroutine(HitEffect(Color.green, 0.5f));
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

    private void SpawnDamageNumber(float damageAmount, Color color)
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
        
        // Set the sorting layer to UI to ensure it renders on top
        Canvas canvas = damageNumberObj.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingLayerName = "DamageNumber";
        }
        
        DamageNumber damageNumber = damageNumberObj.GetComponent<DamageNumber>();
        if (damageNumber != null)
        {
            damageNumber.SetDamageAmount(damageAmount);

            // float interpolate = Mathf.Clamp01(damageAmount / maxHealth); // Adjust 100f to your max expected damage
            // Color gradientColor = Color.Lerp(new Color(128, 0, 0), Color.red, interpolate); // marooon to red, interpolates between using t
            damageNumber.SetColor(color);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && blackDashActive)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.ApplyForce((enemy.transform.position - transform.position).normalized * BlackFlash.staticForce / 2); // Apply force away from player
            enemy.TakeDamage(BlackFlash.staticDamage); // Deal damage to the enemy
        }
    }

    void Die()
    {
        // Handle enemy death (e.g., play animation, destroy object)
        Destroy(gameObject);
    }

    public void UpdateUI()
    {
        updateHealthUI.Invoke(health, maxHealth);
        uiManager.soulsText.text = souls.ToString();
    }
}
