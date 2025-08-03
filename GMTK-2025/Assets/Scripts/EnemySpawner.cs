using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs;

    [Header("Player Target")]
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnMargin = 2f; // How far outside the camera-circle to spawn
    public int maxWavePopulation = 10;
    private int lastMaxWavePopulation = 10; // To track changes in max population
    private float timer;
    private Camera mainCam;
    
    [Header("Tilemap Validation")]
    [SerializeField] private Tilemap groundTilemap; // Reference to the ground tilemap
    [SerializeField] private int maxSpawnAttempts = 10; // Maximum attempts to find a valid spawn position
    [SerializeField] private bool disableTilemapValidation = false; // Toggle to disable tilemap validation
    
    [Header("Debug Info ")]
    public int currentEnemies = 0; // Current number of enemies spawned
    [SerializeField] private UIManager uIManager;

    void Awake()
    {
        uIManager = FindFirstObjectByType<UIManager>();
    }
    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        mainCam = Camera.main;
        timer = spawnInterval;
        
        // Try to find the ground tilemap if not assigned
        if (groundTilemap == null)
        {
            groundTilemap = GameObject.Find("Floor").GetComponent<Tilemap>();
        }
    }

    public void Restart()
    {
        uIManager.isInShop = false;
        maxWavePopulation = Mathf.RoundToInt(lastMaxWavePopulation * 1.5f); // Increase max population by 50% on restart
        lastMaxWavePopulation = maxWavePopulation; // Update last max population
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f && maxWavePopulation > 0 && player != null)
        {
            SpawnEnemy();
            timer = spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("EnemySpawner: enemyPrefabs is null or empty!");
            return;
        }
        
        if (maxWavePopulation == 0)
        {
            Debug.LogWarning("EnemySpawner: maxWavePopulation is 0, cannot spawn");
            return;
        }
        
        if (player == null)
        {
            Debug.LogError("EnemySpawner: player is null!");
            return;
        }
        
        if (mainCam == null)
        {
            Debug.LogError("EnemySpawner: mainCam is null!");
            return;
        }
            
        Vector3 spawnPos = GetValidSpawnPosition();
        
        // If we couldn't find a valid position, don't spawn
        if (spawnPos == Vector3.zero)
        {
            Debug.LogWarning("EnemySpawner: Could not find valid spawn position for enemy");
            return;
        }
        
        maxWavePopulation--;
        currentEnemies++;
        
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject prefabToSpawn = enemyPrefabs[randomIndex];
        
        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogError($"EnemySpawner: Enemy prefab is null at index {randomIndex}");
        }
    }

    Vector3 GetValidSpawnPosition()
    {
        if (player == null || mainCam == null)
        {
            Debug.LogError("EnemySpawner: Player or Camera is null in GetValidSpawnPosition");
            return Vector3.zero;
        }
        
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Vector3 candidatePos = GetCircularSpawnPosition();
            
            // Check if the position is on a valid tilemap tile
            if (IsValidSpawnPosition(candidatePos))
            {
                return candidatePos;
            }
        }
        
        Debug.LogWarning($"EnemySpawner: Could not find valid spawn position after {maxSpawnAttempts} attempts");
        // If we couldn't find a valid position after max attempts, return zero
        return Vector3.zero;
    }

    bool IsValidSpawnPosition(Vector3 position)
    {
        // If tilemap validation is disabled, assume the position is valid
        if (disableTilemapValidation)
        {
            return true;
        }
        
        // If no tilemap is assigned, assume the position is valid
        if (groundTilemap == null)
        {
            return true;
        }
        
        try
        {
            // Convert world position to tilemap cell position
            Vector3Int cellPosition = groundTilemap.WorldToCell(position);
            
            // Check if there's a tile at this position
            TileBase tile = groundTilemap.GetTile(cellPosition);
            
            // Return true if there's a tile (valid ground to spawn on)
            return tile != null;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"EnemySpawner: Error checking tilemap position {position}: {e.Message}");
            return true; // Fallback to allowing spawn
        }
    }

    Vector3 GetCircularSpawnPosition()
    {
        if (player == null || mainCam == null)
        {
            Debug.LogError("EnemySpawner: Player or Camera is null in GetCircularSpawnPosition");
            return Vector3.zero;
        }
        
        // Get camera size
        float camHeight = 2f * mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;

        // Radius of the circle that contains the camera rectangle
        float radius = 0.5f * Mathf.Sqrt(camWidth * camWidth + camHeight * camHeight);

        // Spawn just outside that radius
        float spawnRadius = radius + spawnMargin;

        // Random angle in radians
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
        Vector3 spawnPos = player.position + (Vector3)offset;

        return spawnPos;
    }
}
