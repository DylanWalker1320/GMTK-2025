using UnityEngine;

public class PoisonBall : Spell
{
    [Header("Specific Spell Properties")]
    [SerializeField] private GameObject poisonPuddlePrefab; // Prefab for the poison puddle
    [SerializeField] private float poisonSpawnTimer = 0f; // Time since the last poison puddle was spawned
    [SerializeField] private float poisonSpawnInterval = 1f; // Interval to spawn poison puddles
    [SerializeField] private float finalPuddleScale = 3f; // Scale of the final poison puddle

    [Header("Upgrade Scaling")]
    [SerializeField] private float damageUpgrade = 1f; // Damage increase per upgrade
    [SerializeField] private float poisonSpawnIntervalUpgrade = 0.1f; // Interval decrease per upgrade
    [SerializeField] private float finalPuddleScaleUpgrade = 0.5f; // Scale increase per upgrade

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init(); // Initialize the spell properties
        OrientSpell(); // Orient the spell towards the mouse position and set its velocity
        AddUpgrade(); // Apply upgrades to the spell
        Destroy(gameObject, destroyTime); // Destroy the spell after a certain time
    }

    // Update is called once per frame
    void Update()
    {
        poisonSpawnTimer += Time.deltaTime;
        if (poisonSpawnTimer >= poisonSpawnInterval)
        {
            Instantiate(poisonPuddlePrefab, transform.position, Quaternion.identity);
            poisonSpawnTimer = 0f; // Reset the timer
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        GameObject collisionObject = other.gameObject;
        if (collisionObject.CompareTag("Enemy"))
        {
            // If the fireball collides with an enemy, deal damage
            collisionObject.GetComponent<SimpleEnemy>().TakeDamage(CalculateDamage(damage, spellType1, spellType2));
            GameObject puddle = GameObject.Instantiate(poisonPuddlePrefab, transform.position, Quaternion.identity);
            puddle.transform.localScale = new Vector3(puddle.transform.localScale.x * finalPuddleScale, puddle.transform.localScale.y * finalPuddleScale, 1f);
            Destroy(gameObject); // Destroy the poison ball after dealing damage
        }
    }

    void AddUpgrade()
    {
        int spellLevel = GetSpellLevel(Spells.PoisonPuddle);
        damage = damageUpgrade * spellLevel; // Increase the damage based on upgrades
        poisonSpawnInterval -= poisonSpawnIntervalUpgrade * spellLevel; // Decrease the spawn interval of poison puddles
        if (poisonSpawnInterval < 0.1f) // Ensure the interval does not go below a minimum value
        {
            poisonSpawnInterval = 0.1f;
        }
    }
}
