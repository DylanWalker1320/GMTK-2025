
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TrailRenderer))]
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
    public float invincibilityFrames = 1; // Invincibility frames after taking damage
    public float dashStrength;
    public float dashCooldown;
    private bool canDash = true;

    [Header("Base Stats")]

    public static float baseDashStrength = -1; // Initialize to -1 to indicate it hasn't been set yet
    public static float baseXpPullRange = -1;

    [Header("Multipliers")]
    public float experienceGainMultiplier;
    public float soulGainMultiplier;
    public float lifeStealMultiplier;
    public float castBoostMultiplier;
    public float speedBoostMultiplier;
    public float dashBoostMultiplier;
    public float dropLengthMultiplier;
    private float baseExperienceGainMultiplier = 1;
    private int baseSoulGainMultiplier = 1;
    private float baseLifeStealMultiplier = 0;
    private float baseCastBoostMultiplier = 1;
    private float baseSpeedBoostMultiplier = 1;
    private float baseDashBoostMultiplier = 1;
    private float baseDropLengthMultiplier = 1;

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

    [Header("Drop/Relic Timers")]
    private float baseDropEffectLength = 5f;

    [Header("UI Elements")]
    [SerializeField] private GameObject damageNumberPrefab; // Prefab for damage numbers
    [SerializeField] private float damageNumberSpawnRadius = 1f; // Radius around player to spawn damage numbers
    [SerializeField] private Slider dashBar;
    public Transform reticle; // Inspector reference to the reticle script for aiming
    public UnityEvent<float, float> updateHealthUI;
    [Header("Movement/Animation")]
    public Vector2 movement;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator shadowAnimator;
    public bool facingRight = true;
    public static PlayerInput _playerInput;
    [Header("Effects")]
    public ParticleSystemForceField xpParticleSystem; // Particle system for soul collection effect
    [SerializeField] private  ParticleSystem dashParticles;
    [SerializeField] private TrailRenderer dashTrail;
    private SpriteRenderer playerSprite; // Reference to the player's sprite renderer for 1ipping
    private Rigidbody2D rb;
    private AudioManager audioManager;
    private UIManager uiManager;
    private float invincibilityTimer = 0f; // Timer for invincibility frames


    void Awake()
    {
        if(_playerInput != null && _playerInput != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        if (baseDashStrength < 0) baseDashStrength = dashStrength; // Set base dash strength if not already set
        if (baseXpPullRange < 0) baseXpPullRange = xpParticleSystem.endRange; // Set base XP pull range if not already set

        audioManager = FindFirstObjectByType<AudioManager>();
        uiManager = FindFirstObjectByType<UIManager>();
        playerSprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        dashTrail = GetComponent<TrailRenderer>();

        health = maxHealth;
        experiencePitchTimer = experiencePitchChangeInterval;

        experienceGainMultiplier = baseExperienceGainMultiplier;
        soulGainMultiplier = baseSoulGainMultiplier;
        lifeStealMultiplier = baseLifeStealMultiplier;
        castBoostMultiplier = baseCastBoostMultiplier;
        speedBoostMultiplier = baseSpeedBoostMultiplier;
        dashBoostMultiplier = baseDashBoostMultiplier;
        dropLengthMultiplier = baseDropLengthMultiplier;
    }

    void Start()
    {
        updateHealthUI.Invoke(health, maxHealth);
    }

    void Update()
    {

        if(isGainingExperience)
        {
            experiencePitchTimer -= Time.deltaTime;
            if(experiencePitchTimer <= 0f)
            {
                isGainingExperience = false;
            }
        }

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
        if (rb.linearVelocity.magnitude < maxSpeed * speedBoostMultiplier)
        {
            rb.AddForce(movement * moveForce * speedBoostMultiplier);
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

    private void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
        movement = movement.normalized;
    }

    private void OnDash(InputValue value)
    {
        if (!canDash) { return; }

        canDash = false;
        StartCoroutine(DashCooldown());
        StartCoroutine(TrailEmissionCooldown());

        if (value.isPressed && uiManager.isInUI == false)
        {
            // Dash
            dashTrail.emitting = true;
            dashParticles.Play();
            audioManager.Play("DASH");
            CinemachineShake.Instance.ShakeCamera(0.65f + (dashBoostMultiplier * baseDashStrength * 0.01f), .175f);
            rb.AddForce(movement * dashStrength * dashBoostMultiplier, ForceMode2D.Impulse);
        }
    }

    private IEnumerator DashCooldown()
    {
        float remainingCooldown = dashCooldown;
        while (remainingCooldown > 0f)
        {
            remainingCooldown = Mathf.Max(0f, remainingCooldown - Time.deltaTime);
            dashBar.value = Mathf.Lerp(0f, 1f, (dashCooldown - remainingCooldown) / dashCooldown) * 100f;
            yield return null;
        }
        canDash = true;
    }

    private IEnumerator TrailEmissionCooldown()
    {
        float remainingTrailEmission = dashCooldown / 2;
        while (remainingTrailEmission > 0f) // Trail Emission
        {
            remainingTrailEmission = Mathf.Max(0f, remainingTrailEmission - Time.deltaTime);
            yield return null;
        }
        dashTrail.emitting = false;
    
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
        

        experience += 1 * experienceGainMultiplier;
        souls += (int) (1 * soulGainMultiplier);

        GameResultsTracker._instance.IncrementSoulsEarned();

        SpawnSoulNumber();

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

    public void StealLife(float damageAmount)
    {
        if(lifeStealMultiplier > 0)
        {
            Heal(damageAmount * lifeStealMultiplier);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (invincibilityTimer > 0f) {
            return; // Ignore damage if invincibility frames are active
        }

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

    // Utility Functions

    public float GetTrueCastStrength()
    {
        return castStrength * castBoostMultiplier;
    }

    public float GetTrueCastSpeed()
    {
        return castSpeed * castBoostMultiplier;
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

    private void SpawnSoulNumber()
    {
        if (damageNumberPrefab == null) return; // reusing damage number object

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
            damageNumber.SetDamageAmount(souls); // Display experience gained

            Color gradientColor = Color.Lerp(new Color(0, 0.7173f, 1), new Color(0, 1, 0.6822f), Mathf.Clamp01(experience / nextLevelExperience)); //interpolates via exp left to next level
            damageNumber.SetColor(gradientColor);
        }
    }

    // Death and UI Function

    void Die()
    {
        Time.timeScale = 0f;

        // Record Information for Game Results Tracker
        GameResultsTracker._instance.RecordFinalPlayerStats(level, (int)maxHealth, (int)maxSpeed, castStrength, castSpeed, dashStrength, xpParticleSystem.endRange);
        GameResultsTracker._instance.ActivateResultsMenu();

        // Handle enemy death (e.g., play animation, destroy object)
        Destroy(gameObject);
    }

    public void UpdateUI()
    {
        updateHealthUI.Invoke(health, maxHealth);
        uiManager.soulsText.text = souls.ToString();
    }

    // Pick Up Behaviour Logic
    
    public void PickUpDrop(DroppableObject.DropType dropType, float multiplierBoost)
    {
        // For performance, multipliers MUST stack and shouldn't repeat asynchronous coroutines to avoid rapid garbage instancing
        // Game design wise this is much better ^^
        switch(dropType)
        {
            case DroppableObject.DropType.ExperienceBoost:

                if(experienceGainMultiplier <= baseExperienceGainMultiplier)
                {
                    StartCoroutine(DropEffectCountdown(dropType));
                }
                experienceGainMultiplier *= multiplierBoost;
                break;
            case DroppableObject.DropType.SoulBoost:

                if(soulGainMultiplier <= baseSoulGainMultiplier)
                {
                    StartCoroutine(DropEffectCountdown(dropType));
                }
                soulGainMultiplier *= multiplierBoost;
                break;
            case DroppableObject.DropType.LifeSteal:

                if(lifeStealMultiplier <= baseLifeStealMultiplier)
                {
                    StartCoroutine(DropEffectCountdown(dropType));
                }
                lifeStealMultiplier += multiplierBoost; // lifesteal is additive here
                break;
            case DroppableObject.DropType.CastBoost:

                if(castBoostMultiplier <= baseCastBoostMultiplier)
                {
                    StartCoroutine(DropEffectCountdown(dropType));
                }
                castBoostMultiplier *= multiplierBoost;
                break;
            case DroppableObject.DropType.SpeedBoost:

                if(speedBoostMultiplier <= baseSpeedBoostMultiplier)
                {
                    StartCoroutine(DropEffectCountdown(dropType));
                }
                speedBoostMultiplier *= multiplierBoost;
                break;
            case DroppableObject.DropType.DashBoost:
            
                if(dashBoostMultiplier <= baseDashBoostMultiplier)
                {
                    StartCoroutine(DropEffectCountdown(dropType));
                }
                dashBoostMultiplier *= multiplierBoost;
                break;
            case DroppableObject.DropType.DropLength:

                if(dropLengthMultiplier <= baseDropLengthMultiplier)
                {
                    StartCoroutine(DropEffectCountdown(dropType));
                }
                dropLengthMultiplier += multiplierBoost; // additive otherwise we'll have buffs lasting 5 minutes lol
                break;
        }
    }

    IEnumerator DropEffectCountdown(DroppableObject.DropType dropType)
    {
        Debug.Log("Starting countdown");
        float elapsed = 0f;
        float duration = baseDropEffectLength * dropLengthMultiplier; // given how DropEffectCountdown starts immediately, the duration for drop length will be max of this while other buffs can peak higher for time
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // visual timers here possibly?
            yield return null;
        }

        Debug.Log("Countdown finished");

        switch(dropType)
        {
            case DroppableObject.DropType.ExperienceBoost:
                experienceGainMultiplier = baseExperienceGainMultiplier;
                break;
            case DroppableObject.DropType.SoulBoost:
                soulGainMultiplier = baseSoulGainMultiplier;
                break;
            case DroppableObject.DropType.LifeSteal:
                lifeStealMultiplier = baseLifeStealMultiplier;
                break;
            case DroppableObject.DropType.CastBoost:
                castBoostMultiplier = baseCastBoostMultiplier;
                break;
            case DroppableObject.DropType.SpeedBoost:
                speedBoostMultiplier = baseSpeedBoostMultiplier;
                break;
            case DroppableObject.DropType.DashBoost:
                dashBoostMultiplier = baseDashBoostMultiplier;
                break;
            case DroppableObject.DropType.DropLength:
                dropLengthMultiplier = baseDropLengthMultiplier;
                break;
        }

    }
}
