using UnityEngine;
using UnityEngine.Tilemaps;
public class TilemapGenerator : MonoBehaviour
{

    /* IDEA:
    * Start with one tile in the center
    * Randomly pick a tile and expand in a random direction
    * Repeat until desired number of tiles is reached
    */

    public Tilemap floor;
    public Tilemap wall;
    public TileBase[] floorTiles;
    public TileBase[] wallTiles;

}