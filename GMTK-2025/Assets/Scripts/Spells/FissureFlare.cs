using UnityEngine;

public class FissureFlare : Spell
{
    [Header("Fissure Flare Settings")]
    [SerializeField] private int numberOfBolts = 3;              // Number of bolts to spawn
    [SerializeField] private float boltSpreadAngle = 10f;        // Angle between each bolt (in degrees)
    [SerializeField] private float boltSpreadRange = 5f;         // Random +/- spread angle (in degrees)
    [SerializeField] private FissureFlareBolt spellPrefab;       // Prefab for the bolt spell

    void Start()
    {
        Init();
        OrientSpell(); // Ensure the spell is pointing toward the reticle

        Vector2 baseDirection = transform.right; // Spell's forward direction

        for (int i = 0; i < numberOfBolts; i++)
        {
            // Calculate spread: evenly spaced, centered, plus random offset
            float spreadOffset = i - (numberOfBolts - 1) / 2f;
            float angleOffset = spreadOffset * boltSpreadAngle + Random.Range(-boltSpreadRange, boltSpreadRange);

            // Rotate the base direction by angleOffset degrees around Z
            Vector2 direction = Quaternion.AngleAxis(angleOffset, Vector3.forward) * baseDirection;

            // Debug line to visualize the direction
            //Debug.DrawRay(transform.position, direction * 5f, Color.red, 2f);

            // Instantiate the bolt at the spell's position, facing the calculated direction
            FissureFlareBolt bolt = Instantiate(spellPrefab, transform.position, Quaternion.identity).GetComponent<FissureFlareBolt>();
            bolt.SetDirection(direction);
        }

        Destroy(gameObject);
    }
}
