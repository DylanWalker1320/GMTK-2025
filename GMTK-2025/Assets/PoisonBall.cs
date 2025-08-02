using UnityEngine;

public class PoisonBall : Spell
{
    [Header("Specific Spell Properties")]
    [SerializeField] private Spell poisonPuddlePrefab; // Prefab for the poison puddle
    [SerializeField] private float poisonSpawnTotalDuration = 0f; // Total duration for the poison puddle to exist
    [SerializeField] private float poisonSpawnInterval = 1f; // Interval to spawn poison puddles

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init(); // Initialize the spell properties
        OrientSpell(); // Orient the spell towards the mouse position and set its velocity
        SpawnPoisonPuddle();
        Destroy(gameObject, destroyTime); // Destroy the spell after a certain time
    }

    // Update is called once per frame
    void Update()
    {
        poisonSpawnTotalDuration += Time.deltaTime;
        if (poisonSpawnTotalDuration >= poisonSpawnInterval)
        {
            SpawnPoisonPuddle(); // Spawn a poison puddle at the current position
            poisonSpawnTotalDuration = 0f; // Reset the timer
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        GameObject collisionObject = other.gameObject;
        if (collisionObject.CompareTag("Enemy"))
        {
            // If the fireball collides with an enemy, deal damage
            collisionObject.GetComponent<SimpleEnemy>().TakeDamage(GetDamage());
            SpawnPoisonPuddle();
            Destroy(gameObject); // Destroy the poison ball after dealing damage
        }
    }
    
    void SpawnPoisonPuddle()
    {
        Instantiate(poisonPuddlePrefab, transform.position, Quaternion.identity);
    }
}
