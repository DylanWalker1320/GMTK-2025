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
    public int maxWavePopulation;
    private float timer;
    private Camera mainCam;
    
    [Header("Tilemap Validation")]
    [SerializeField] private Tilemap groundTilemap; // Reference to the ground tilemap
    [SerializeField] private int maxSpawnAttempts = 10; // Maximum attempts to find a valid spawn position
    
    [Header("Debug Info ")]
    public int currentEnemies = 0; // Current number of enemies spawned

    void Awake()
    {
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
            groundTilemap = FindObjectOfType<Tilemap>();
        }
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
        if (enemyPrefabs == null || enemyPrefabs.Length == 0 || maxWavePopulation == 0 || player == null || mainCam == null)
            return;
            
        Vector3 spawnPos = GetValidSpawnPosition();
        
        // If we couldn't find a valid position, don't spawn
        if (spawnPos == Vector3.zero)
        {
            Debug.LogWarning("Could not find valid spawn position for enemy");
            return;
        }
        
        maxWavePopulation--;
        currentEnemies++;
        GameObject prefabToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Enemy prefab is null at index " + Random.Range(0, enemyPrefabs.Length));
        }
    }

    Vector3 GetValidSpawnPosition()
    {
        if (player == null || mainCam == null)
        {
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
        
        // If we couldn't find a valid position after max attempts, return zero
        return Vector3.zero;
    }

    bool IsValidSpawnPosition(Vector3 position)
    {
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
            Debug.LogWarning("Error checking tilemap position: " + e.Message);
            return true; // Fallback to allowing spawn
        }
    }

    Vector3 GetCircularSpawnPosition()
    {
        if (player == null || mainCam == null)
        {
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

        return player.position + (Vector3)offset;
    }
}
