using UnityEngine;
using System;
using UnityEngine.Tilemaps;

public class FissureFlareBolt : Spell
{
    [Header("Fissure Flare Bolt Settings")]
    [SerializeField] private float waveAmplitude = 0.5f;
    [SerializeField] private float waveFrequency = 2f;
    [SerializeField] private float groundEffectCooldown = 0.2f; // Cooldown for ground effect
    [SerializeField] private GameObject groundEffectPrefab; // Prefab for the ground effect

    [Header("Upgrade Scaling")]
    [SerializeField] private float groundEffectFrequencyUpgrade = 0.025f; // Frequency increase per upgrade
    [SerializeField] private float damageUpgrade = 1f; // Damage increase per upgrade

    [Header("Ground Detection")]
    [SerializeField] private Tilemap groundTilemap; // Reference to the ground tilemap

    private Vector2 boltDirection;
    private Vector2 perpendicular;
    private float timeAlive = 0f;
    private Func<float, float> waveFunction;
    private float groundEffectTimer = 0f; // Timer for ground effect cooldown
    private Vector3 lastPosition;
    private Vector3 initialPosition;

    void Start()
    {
        Init();
        AddUpgrade(); // Apply upgrades to the spell
        initialPosition = transform.position;
        lastPosition = transform.position;
        Destroy(gameObject, destroyTime); // Destroy the bolt after a certain time

        if (groundTilemap == null)
        {
            groundTilemap = GameObject.Find("Floor").GetComponent<Tilemap>();
        }
    }

    public void SetDirection(Vector2 dir)
    {
        boltDirection = dir.normalized;
        perpendicular = new Vector2(-boltDirection.y, boltDirection.x);
        waveFunction = UnityEngine.Random.value > 0.5f ? Mathf.Sin : Mathf.Cos;
    }

    void Update()
    {
        timeAlive += Time.deltaTime;

        // Forward progress along straight path
        Vector3 forward = boltDirection * speed * timeAlive;
        //Debug.DrawRay(transform.position, forward, Color.green);

        // Perpendicular wave offset
        float wave = waveFunction(timeAlive * waveFrequency) * waveAmplitude;
        Vector3 waveOffset = perpendicular * wave;

        // Final position = forward + wave offset
        transform.position = initialPosition + forward + waveOffset;

        // Handle ground effect cooldown
        if (groundEffectTimer > 0f)
        {
            groundEffectTimer -= Time.deltaTime;
            if (groundEffectTimer <= 0f)
            {
                groundEffectTimer = 0f; // Reset timer
            }
        }

        // Check for ground effect
        if (groundEffectTimer <= 0f && IsValidGroundPosition(transform.position))
        {
            // Instantiate ground effect at the current position
            Instantiate(groundEffectPrefab, transform.position, Quaternion.identity);
            groundEffectTimer = groundEffectCooldown; // Reset cooldown timer
        }

        // Calculate direction the projectile is moving in
        Vector2 movementDirection = (transform.position - lastPosition).normalized;
        lastPosition = transform.position;

        // Calculate angle and rotate the object
        float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private bool IsValidGroundPosition(Vector3 position)
    {
        // Convert world position to tilemap cell position
        Vector3Int cellPosition = groundTilemap.WorldToCell(position);
        
        // Check if there's a tile at this position
        TileBase tile = groundTilemap.GetTile(cellPosition);
        
        // Return true if there's a tile (valid ground to spawn on)
        return tile != null;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(CalculateDamage(damage, spellType1, spellType2));
            }
        }
    }

    void AddUpgrade()
    {
        // Increase the flare frequency based on spell level
        int spellLevel = GetSpellLevel(Spells.FissureFlare);
        groundEffectCooldown -= groundEffectFrequencyUpgrade * spellLevel; // Decrease cooldown for ground effect
        damage += damageUpgrade * spellLevel; // Increase damage based on upgrades
        if (groundEffectCooldown < 0.1f) // Ensure cooldown doesn't go below a minimum threshold
        {
            groundEffectCooldown = 0.1f;
        }
    }
}