using UnityEngine;

public class Reanimate : Spell
{
    [Header("Reanimate Properties")]
    [Tooltip("How long the reanimated creature will stay alive after being reanimated.")]
    [SerializeField] private float reanimateTime = 3f;
    [Tooltip("The move force multiplier of the reanimated creature.")]
    [SerializeField] private float moveForceMultiplier = 1.2f;
    [Tooltip("The prefabs to use for the reanimated creature.")]
    [SerializeField] private GameObject[] creaturePrefabs;

    void Start()
    {
        ReanimateCreature();
        Destroy(gameObject);
    }

    void ReanimateCreature()
    {
        if (creaturePrefabs.Length > 0)
        {
            GameObject creature = Instantiate(creaturePrefabs[Random.Range(0, creaturePrefabs.Length)], transform.position, Quaternion.identity);
            Destroy(creature, reanimateTime);

            creature.tag = "Player";

            SimpleEnemy simpleEnemy = creature.GetComponent<SimpleEnemy>();
            if (simpleEnemy != null)
            {
                simpleEnemy.moveForce *= moveForceMultiplier;
                simpleEnemy.maxSpeed = speed;

                simpleEnemy.targetTag = "Enemy";
            }
        }
        
    }
}
