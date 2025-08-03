using UnityEngine;

public class Storm : Spell
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Storm Specific Properties")]
    [SerializeField] private float outerRadius = 10f; // Radius of the storm effect
    [SerializeField] private float innerRadius = 5f; // Damage dealt by the storm
    [SerializeField] private int minNumStrikes = 5; // Number of strikes to spawn
    [SerializeField] private int maxNumStrikes = 7; // Maximum number of strikes to spawn
    [SerializeField] private GameObject strikePrefab; // Prefab for the lightning strike effect
    [Header("Upgrade Scaling")]
    [SerializeField] private float numStrikesUpgrade = 0.5f; // Number of strikes increase per upgrade
    [SerializeField] private float radiusUpgrade = 0.5f; // Radius increase per upgrade

    void Start()
    {
        Init(); // Initialize the spell properties

        // Set spell location to player position
        transform.position = GameObject.FindGameObjectWithTag("Player").transform.position;
        AddUpgrade(); // Apply upgrades to the spell
        SpawnStrikes(); // Spawn the lightning strikes
        Destroy(gameObject);
    }

    void SpawnStrikes()
    {
        int numStrikes = Random.Range(minNumStrikes, maxNumStrikes + 1);
        for (int i = 0; i < numStrikes; i++)
        {
            // Divide 360 by the number of strikes to get the angle between each strike, then +-10 degrees for randomness
            float angle = i * (360f / numStrikes) + Random.Range(-10f, 10f);

            // Get a value on the unit circle, then scale it by a value within inner and outer radius
            Vector3 strikePosition = transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * Random.Range(innerRadius, outerRadius);

            // Instantiate the strike prefab at the calculated position
            GameObject strike = Instantiate(strikePrefab, strikePosition, Quaternion.identity);
        }
    }

    void AddUpgrade()
    {
        int spellLevel = GetSpellLevel(Spells.Storm);
        minNumStrikes += Mathf.RoundToInt(numStrikesUpgrade * spellLevel); // Increase the minimum number of strikes
        maxNumStrikes += Mathf.RoundToInt(numStrikesUpgrade * spellLevel); // Increase the maximum number of strikes
        outerRadius += radiusUpgrade * spellLevel; // Increase the outer radius
    }
}
