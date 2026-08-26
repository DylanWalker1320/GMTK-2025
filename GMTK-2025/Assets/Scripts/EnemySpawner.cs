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
    [SerializeField] private float spawnMargin = 2f;
    public int maxWavePopulation = 10;
    public int lastMaxWavePopulation = 10;
    private float timer;
    private Camera mainCam;

    [Header("Tilemap Validation")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private int maxSpawnAttempts = 10;
    [SerializeField] private bool disableTilemapValidation = false;

    [Header("Debug Info")]
    public int currentEnemies = 0;
    public int mobsKilled = 0;
    public bool spawningPaused = false; // Paused while player is in refresh room
    private GameManager gameManager;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
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

        if (groundTilemap == null)
            groundTilemap = GameObject.Find("Floor").GetComponent<Tilemap>();
    }

    // Pause or resume enemy spawning. Used by GameManager for portal transitions.
    public void SetSpawningPaused(bool paused)
    {
        spawningPaused = paused;
    }

    public void Restart()
    {
        gameManager.ToggleSafeArea(false);
        maxWavePopulation = Mathf.RoundToInt(lastMaxWavePopulation * 1.1f);
        spawnInterval = Mathf.Max(0.2f, spawnInterval * 0.9f); // Decrease spawn interval by 10%, but not below 0.2s
        lastMaxWavePopulation = maxWavePopulation;
        spawningPaused = false; // Ensure spawning resumes on restart
        mobsKilled = 0;
        gameManager.UpdateEnemiesRemaining();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (currentEnemies < 0)
            currentEnemies = 0;

        // Don't spawn if paused, wave is exhausted, or player is missing
        if (spawningPaused || timer > 0f || maxWavePopulation <= 0 || player == null)
            return;

        Debug.Log($"EnemySpawner: Spawning enemy. CurrentEnemies: {currentEnemies}, MaxWavePopulation: {maxWavePopulation}");

        SpawnEnemy();
        timer = spawnInterval;
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
            Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        else
            Debug.LogError($"EnemySpawner: Enemy prefab is null at index {randomIndex}");
    }

    public Vector3 GetValidSpawnPosition()
    {
        if (player == null || mainCam == null)
        {
            Debug.LogError("EnemySpawner: Player or Camera is null in GetValidSpawnPosition");
            return Vector3.zero;
        }

        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Vector3 candidatePos = GetCircularSpawnPosition();
            if (IsValidSpawnPosition(candidatePos))
                return candidatePos;
        }

        Debug.LogWarning($"EnemySpawner: Could not find valid spawn position after {maxSpawnAttempts} attempts");
        return Vector3.zero;
    }

    bool IsValidSpawnPosition(Vector3 position)
    {
        if (disableTilemapValidation) return true;
        if (groundTilemap == null) return true;

        try
        {
            Vector3Int cellPosition = groundTilemap.WorldToCell(position);
            TileBase tile = groundTilemap.GetTile(cellPosition);
            return tile != null;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"EnemySpawner: Error checking tilemap position {position}: {e.Message}");
            return true;
        }
    }

    Vector3 GetCircularSpawnPosition()
    {
        if (player == null || mainCam == null)
        {
            Debug.LogError("EnemySpawner: Player or Camera is null in GetCircularSpawnPosition");
            return Vector3.zero;
        }

        float camHeight = 2f * mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;
        float radius = 0.5f * Mathf.Sqrt(camWidth * camWidth + camHeight * camHeight);
        float spawnRadius = radius + spawnMargin;
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
        return player.position + (Vector3)offset;
    }
}