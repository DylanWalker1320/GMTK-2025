using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.AI;
using NavMeshPlus.Components;

public class MapGenerator : MonoBehaviour
{
    /* Idea:
     * 1. Generate a set of points around a circle with Perlin noise to create a rough blob shape.
     * 2. Smooth the shape using a Catmull–Rom spline.
     * 3. Rasterize the polygon to fill in tiles.
     * 4. Add walls around the filled area.
     */

    [Header("Tilemaps and Tiles")]
    public Tilemap floor;
    public Tilemap wall;
    public TileBase wallTile;
    public TileBase baseFloorTile;
    public TileBase[] extraFloorTiles;

    [Header("NavMesh")]
    public NavMeshSurface navMeshSurface;

    [Header("Obstacles")]
    public GameObject[] obstaclePrefabs;
    public int baseNumObstacles = 5;
    [Tooltip("Range (+/-) around base number of obstacles")]
    public int numObstacleRange = 3;
    [Tooltip("Number of attempts to place each obstacle")]
    public int obstaclePlacementAttempts = 20;
    [Tooltip("Minimum distance between obstacles")]
    public float obstaclePadding = 5f;
    [Tooltip("If true, overrides baseNumObstacles with number based on area of the map")]
    public bool overrideNumObstaclesByArea = true;
    [Tooltip("Area of map per obstacle (used if overrideNumObstaclesByArea is true)")]
    public float areaPerObstacle = 50f;
    public float playerSafeRadius = 5f; // Safe radius around player spawn
    private List<GameObject> placedObstacles;
    

    [Header("Generation Settings")]
    public int numSamples = 20; // number of angle samples (e.g., 72 = every 5°)
    public float baseRadius = 20f;
    public float radiusVariation = 6f;
    public float noiseScale = 0.3f;
    public int smoothness = 6; // number of interpolated points between samples
    public int extraTileChance = 20;
    public int wallThickness = 3;

    [Header("Debugging")]
    public bool debugMode = false;

    private HashSet<Vector2Int> filledCells = new HashSet<Vector2Int>();
    private string debugPrefixBase = "<color=#00FF00>[MapGenerator]</color>";
    private string debugPrefixFloor => $"{debugPrefixBase}<color=#FFFF00>[Floor]</color>";
    private string debugPrefixWall => $"{debugPrefixBase}<color=#FF0000>[Wall]</color>";
    public string debugPrefixObstacle => $"{debugPrefixBase}<color=#00FFFF>[Obstacle]</color>";


    void Start()
    {
        Generate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Generate();
        }
    }

    public void Clear()
    {
        if (debugMode) Debug.Log($"{debugPrefixBase} Clearing map...");
        floor.ClearAllTiles();
        wall.ClearAllTiles();
        filledCells.Clear();

        if (placedObstacles != null)
        {
            foreach (var obj in placedObstacles)
            {
                if (obj != null)
                    Destroy(obj);
            }
        }

        placedObstacles = new List<GameObject>();
    }

    public void Generate()
    {
        if (debugMode) Debug.Log($"{debugPrefixBase} Starting map generation...");
        Clear();
        StartCoroutine(GenerateFloorCoroutine());
    }

    public IEnumerator GenerateFloorCoroutine()
    {
        // === 1. Generate noisy circular boundary points ===
        List<Vector2> ringPoints = new List<Vector2>();

        for (int i = 0; i < numSamples; i++)
        {
            float angle = (i / (float)numSamples) * Mathf.PI * 2f;

            float noiseOffsetX = Random.Range(0f, 1000f);
            float noiseOffsetY = Random.Range(0f, 1000f);

            float noise = Mathf.PerlinNoise(
                Mathf.Cos(angle) * noiseScale + noiseOffsetX,
                Mathf.Sin(angle) * noiseScale + noiseOffsetY
            );

            float radius = baseRadius + (noise - 0.5f) * 2f * radiusVariation;
            Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            ringPoints.Add(point);
            if (debugMode) Debug.DrawLine(Vector2.zero, point, Color.red, 1f);
        }

        // === 2. Smooth shape with Catmull–Rom spline ===
        List<Vector2> smoothPoints = SmoothPoints(ringPoints, smoothness);

        // === 3. Fill polygon ===
        FillPolygon(smoothPoints);

        if (debugMode)
            Debug.Log($"{debugPrefixFloor} Floor generation complete. Total tiles placed: {filledCells.Count}");

        GenerateWalls();

        // === 4. Place Obstacles ===
        PlaceObstacles();

        // === CRITICAL: Wait one frame for destroyed objects to be cleaned up ===
        yield return null;

        // === 5. Bake NavMesh AFTER cleanup ===
        if (navMeshSurface != null)
        {
            if (debugMode) Debug.Log($"{debugPrefixBase} Baking NavMesh...");
            navMeshSurface.BuildNavMesh();
        }
    }

    List<Vector2> SmoothPoints(List<Vector2> pts, int smoothness)
    {
        // Graphics jumpscare!!!!!!!!! (Catmull–Rom spline interpolation)

        List<Vector2> smoothed = new List<Vector2>();

        for (int i = 0; i < pts.Count; i++)
        {
            Vector2 p0 = pts[(i - 1 + pts.Count) % pts.Count];
            Vector2 p1 = pts[i];
            Vector2 p2 = pts[(i + 1) % pts.Count];
            Vector2 p3 = pts[(i + 2) % pts.Count];

            for (int j = 0; j < smoothness; j++)
            {
                float t = j / (float)smoothness;
                float t2 = t * t;
                float t3 = t2 * t;

                // Literal magic idk
                Vector2 c = 0.5f * (
                    (2f * p1) +
                    (-p0 + p2) * t +
                    (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                    (-p0 + 3f * p1 - 3f * p2 + p3) * t3
                );

                smoothed.Add(c);
            }
        }

        return smoothed;
    }

    void FillPolygon(List<Vector2> boundary)
    {
        // Compute polygon bounds
        float minX = boundary.Min(p => p.x);
        float maxX = boundary.Max(p => p.x);
        float minY = boundary.Min(p => p.y);
        float maxY = boundary.Max(p => p.y);

        for (int y = Mathf.FloorToInt(minY); y <= Mathf.CeilToInt(maxY); y++)
        {
            for (int x = Mathf.FloorToInt(minX); x <= Mathf.CeilToInt(maxX); x++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                if (IsPointInsidePolygon(p, boundary))
                {
                    Vector2Int cell = new Vector2Int(x, y);
                    if (!filledCells.Contains(cell))
                    {
                        filledCells.Add(cell);
                        floor.SetTile(new Vector3Int(x, y, 0), GetRandomFloorTile());
                    }
                }
            }
        }
    }

    bool IsPointInsidePolygon(Vector2 point, List<Vector2> polygon)
    {
        bool inside = false;
        for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
        {
            bool intersect = ((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                (point.x < (polygon[j].x - polygon[i].x) * 
                (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x);
            if (intersect) inside = !inside;
        }
        return inside;
    }

    void GenerateWalls()
    {
        HashSet<Vector2Int> wallCells = new HashSet<Vector2Int>();

        foreach (var cell in filledCells)
        {
            for (int dx = -wallThickness; dx <= wallThickness; dx++)
            {
                for (int dy = -wallThickness; dy <= wallThickness; dy++)
                {
                    Vector2Int neighbor = new Vector2Int(cell.x + dx, cell.y + dy);
                    // Chebyshev distance check
                    if (Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) <= wallThickness)
                    {
                        if (!filledCells.Contains(neighbor))
                        {
                            wallCells.Add(neighbor);
                        }
                    }
                }
            }
        }

        foreach (var w in wallCells)
        {
            wall.SetTile(new Vector3Int(w.x, w.y, 0), wallTile);
        }

        if (debugMode)
            Debug.Log($"{debugPrefixWall} Generated {wallCells.Count} wall tiles with thickness {wallThickness}");
    }

    void PlaceObstacles()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0 || filledCells.Count == 0)
            return;

        List<Vector2Int> floorCells = filledCells.ToList();

        // === Determine number of obstacles ===
        float area = floorCells.Count;
        int numObstacles;

        if (overrideNumObstaclesByArea)
        {
            numObstacles = Mathf.Max(1, Mathf.FloorToInt(area / Mathf.Max(1f, areaPerObstacle)));
        }
        else
        {
            numObstacles = baseNumObstacles + Random.Range(-numObstacleRange, numObstacleRange + 1);
            numObstacles = Mathf.Max(1, numObstacles);
        }

        // === Track placed obstacles with their radii ===
        List<(Vector3 position, float radius)> placedObstacleData = new List<(Vector3, float)>();

        for (int i = 0; i < numObstacles; i++)
        {
            bool placed = false;

            for (int attempt = 0; attempt < obstaclePlacementAttempts; attempt++)
            {
                if (debugMode)
                    Debug.Log($"{debugPrefixObstacle} Attempting to place obstacle #{i + 1}, attempt {attempt + 1}/{obstaclePlacementAttempts}");

                // Randomly sample a valid cell
                Vector2Int cell = floorCells[Random.Range(0, floorCells.Count)];
                Vector3 worldPos = floor.CellToWorld(new Vector3Int(cell.x, cell.y, 0)) + new Vector3(0.5f, 0.5f, 0);

                // Small random offset
                Vector3 offset = new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f), 0f);
                Vector3 finalPos = worldPos + offset;

                // Randomly select a prefab
                GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

                // === INSTANTIATE FIRST to get accurate bounds ===
                GameObject obstacle = Instantiate(prefab, finalPos, Quaternion.identity, transform);

                // === Get ACTUAL radius from instantiated object ===
                float obstacleRadius = 0.5f; // Fallback
                Collider2D obstacleCollider = obstacle.GetComponentInChildren<Collider2D>();
                
                if (obstacleCollider != null)
                {
                    // Get the actual world-space bounds
                    Bounds bounds = obstacleCollider.bounds;
                    obstacleRadius = Mathf.Max(bounds.extents.x, bounds.extents.y);
                    
                    if (debugMode)
                        Debug.Log($"{debugPrefixObstacle} Actual obstacle radius: {obstacleRadius:F2} (bounds: {bounds.extents})");
                }
                else
                {
                    // Fallback to renderer if no collider
                    Renderer renderer = obstacle.GetComponentInChildren<Renderer>();
                    if (renderer != null)
                    {
                        Bounds bounds = renderer.bounds;
                        obstacleRadius = Mathf.Max(bounds.extents.x, bounds.extents.y);
                        
                        if (debugMode)
                            Debug.Log($"{debugPrefixObstacle} Using renderer radius: {obstacleRadius:F2}");
                    }
                }

                // === Check against already placed obstacles ===
                bool tooCloseToOtherObstacles = false;
                foreach (var (placedPos, placedRadius) in placedObstacleData)
                {
                    float distance = Vector3.Distance(finalPos, placedPos);
                    float minDistance = obstacleRadius + placedRadius + obstaclePadding;
                    
                    if (distance < minDistance)
                    {
                        tooCloseToOtherObstacles = true;
                        if (debugMode)
                            Debug.Log($"{debugPrefixObstacle} Too close to existing obstacle at {placedPos} (distance: {distance:F2}, minRequired: {minDistance:F2})");
                        break;
                    }
                }

                if (tooCloseToOtherObstacles)
                {
                    Destroy(obstacle); // Clean up failed placement
                    continue;
                }

                // === Check wall proximity using tilemap ===
                bool tooCloseToWall = false;
                int checkRadius = Mathf.CeilToInt(obstaclePadding + obstacleRadius) + 1;
                Vector3Int centerCell = wall.WorldToCell(finalPos);

                for (int dx = -checkRadius; dx <= checkRadius; dx++)
                {
                    for (int dy = -checkRadius; dy <= checkRadius; dy++)
                    {
                        Vector3Int checkCell = new Vector3Int(centerCell.x + dx, centerCell.y + dy, 0);
                        if (wall.HasTile(checkCell))
                        {
                            Vector3 wallPos = wall.CellToWorld(checkCell) + new Vector3(0.5f, 0.5f, 0);
                            float distance = Vector3.Distance(finalPos, wallPos);
                            float minWallDistance = obstacleRadius + obstaclePadding;

                            if (distance < minWallDistance)
                            {
                                tooCloseToWall = true;
                                if (debugMode)
                                    Debug.Log($"{debugPrefixObstacle} Too close to wall at {checkCell} (distance: {distance:F2}, minRequired: {minWallDistance:F2})");
                                break;
                            }
                        }
                    }
                    if (tooCloseToWall) break;
                }
                
                // === Check that the obstacle is away from the player spawn point (assumed at (0,0)) ===
                float distanceToPlayer = Vector3.Distance(finalPos, Vector3.zero);

                if (distanceToPlayer < playerSafeRadius)
                {
                    tooCloseToWall = true;
                    if (debugMode)
                        Debug.Log($"{debugPrefixObstacle} Too close to player spawn at (0,0) (distance: {distanceToPlayer:F2}, minRequired: {playerSafeRadius:F2})");
                }

                if (tooCloseToWall)
                {
                    Destroy(obstacle); // Clean up failed placement
                    continue;
                }

                // === All checks passed - keep obstacle ===
                placedObstacles.Add(obstacle);
                placedObstacleData.Add((finalPos, obstacleRadius));
                placed = true;
                
                if (debugMode)
                    Debug.Log($"{debugPrefixObstacle} Successfully placed obstacle #{i + 1} at {finalPos} with radius {obstacleRadius:F2}.");

                break;
            }

            if (!placed && debugMode)
                Debug.Log($"{debugPrefixObstacle} Failed to place obstacle #{i + 1} after {obstaclePlacementAttempts} attempts.");
        }

        if (debugMode)
            Debug.Log($"{debugPrefixObstacle} Placed {placedObstacles.Count}/{numObstacles} obstacles (padding={obstaclePadding}).");
    }
    
    void OnDrawGizmos()
    {
        if (placedObstacles == null) return;
        
        Gizmos.color = Color.red;
        foreach (var obstacle in placedObstacles)
        {
            if (obstacle == null) continue;
            
            Collider2D col = obstacle.GetComponentInChildren<Collider2D>();
            if (col != null)
            {
                float radius = Mathf.Max(col.bounds.extents.x, col.bounds.extents.y);
                Gizmos.DrawWireSphere(obstacle.transform.position, radius + obstaclePadding);
            }
        }
    }

    TileBase GetRandomFloorTile()
    {
        int roll = Random.Range(0, 100);
        if (roll < extraTileChance && extraFloorTiles.Length > 0)
        {
            int index = Random.Range(0, extraFloorTiles.Length);
            return extraFloorTiles[index];
        }
        return baseFloorTile;
    }

    static readonly Vector2Int[] Directions = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };
}
