using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Player Target")]
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnMargin = 2f; // How far outside the camera-circle to spawn

    private float timer;
    private Camera mainCam;

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

        if (timer <= 0f)
        {
            SpawnEnemy();
            timer = spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0 || player == null || mainCam == null)
            return;

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
