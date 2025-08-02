using UnityEngine;

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
        if (enemyPrefabs.Length == 0 || maxWavePopulation == 0 || player == null || mainCam == null)
            return;
        maxWavePopulation--;
        currentEnemies++;
        GameObject prefabToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Vector3 spawnPos = GetCircularSpawnPosition();
        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
    }

    Vector3 GetCircularSpawnPosition()
    {
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
