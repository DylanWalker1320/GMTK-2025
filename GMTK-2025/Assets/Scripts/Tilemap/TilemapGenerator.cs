using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.AI;
using NavMeshPlus.Components;

public class TilemapGenerator : MonoBehaviour
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

    void Start()
    {
        GenerateFloor();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (debugMode) Debug.Log("Generating blob...");
            floor.ClearAllTiles();
            wall.ClearAllTiles();
            filledCells.Clear();
            GenerateFloor();    
        }
    }

    public void GenerateFloor()
    {
        // === 1. Generate noisy circular boundary points ===
        List<Vector2> ringPoints = new List<Vector2>();

        for (int i = 0; i < numSamples; i++)
        {
            float angle = (i / (float)numSamples) * Mathf.PI * 2f;

            // Simulate 'rolling' edges by sampling Perlin noise with a circular pattern
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
            Debug.Log($"Floor generation complete. Total tiles placed: {filledCells.Count}");

        GenerateWalls();

        // === 4. Bake NavMesh ===
        if (navMeshSurface != null)
        {
            if (debugMode) Debug.Log("Baking NavMesh...");
            navMeshSurface.BuildNavMesh();
        }
    }

    List<Vector2> SmoothPoints(List<Vector2> pts, int smoothness)
    {
        // Graphics jumpscare!!!!!!!!! 

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
            Debug.Log($"Generated {wallCells.Count} wall tiles with thickness {wallThickness}");
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
