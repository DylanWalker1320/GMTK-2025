using UnityEngine;
using System;

public class FissureFlareBolt : Spell
{
    [Header("Fissure Flare Bolt Settings")]
    [SerializeField] private float waveAmplitude = 0.5f;
    [SerializeField] private float waveFrequency = 2f;
    [SerializeField] private float groundEffectCooldown = 0.5f; // Cooldown for ground effect
    [SerializeField] private GameObject groundEffectPrefab; // Prefab for the ground effect

    private Vector2 direction;
    private Vector2 perpendicular;
    private float timeAlive = 0f;
    private Func<float, float> waveFunction;
    private float groundEffectTimer = 0f; // Timer for ground effect cooldown
    private Vector3 lastPosition;
    private Vector3 initialPosition;

    void Start()
    {
        Init();
        initialPosition = transform.position;
        lastPosition = transform.position;
        Destroy(gameObject, destroyTime); // Destroy the bolt after a certain time
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        perpendicular = new Vector2(-direction.y, direction.x);
        waveFunction = UnityEngine.Random.value > 0.5f ? Mathf.Sin : Mathf.Cos;
    }

    void Update()
    {
        timeAlive += Time.deltaTime;

        // Forward progress along straight path
        Vector3 forward = direction * speed * timeAlive;
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
        if (groundEffectTimer <= 0f)
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}