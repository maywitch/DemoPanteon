using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }

    // Sahnede bulunan tüm Tile nesnelerini tutar
    private Tile[] tiles;

    // Hücre boyutu; tile’lar arasýndaki mesafe (örn. 1f)
    public float cellSize = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Tile nesneleri TileManager’ýn child'larý olarak yer almalý.
            tiles = GetComponentsInChildren<Tile>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Verilen world position’a en yakýn tile’ý döndürür.
    /// Eðer týklama ile tile merkezi arasýndaki mesafe cellSize'dan büyükse null döner.
    /// </summary>
    public Tile GetTileAtPosition(Vector3 position)
    {
        Tile closestTile = null;
        float minDistance = Mathf.Infinity;

        foreach (Tile tile in tiles)
        {
            float distance = Vector3.Distance(tile.transform.position, position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTile = tile;
            }
        }

        // Eðer en yakýn tile merkezi ile týklanan konum arasýndaki mesafe,
        // cellSize'dan büyükse, geçerli bir tile olarak kabul etme.
        if (minDistance > cellSize)
            return null;

        return closestTile;
    }

    /// <summary>
    /// Verilen tile’ýn komþularýný (8 yönde) döndürür.
    /// </summary>
    public List<Tile> GetNeighbors(Tile currentTile)
    {
        List<Tile> neighbors = new List<Tile>();

        foreach (Tile tile in tiles)
        {
            if (tile == currentTile)
                continue;

            int dx = Mathf.Abs(tile.gridX - currentTile.gridX);
            int dy = Mathf.Abs(tile.gridY - currentTile.gridY);
            if (dx <= 1 && dy <= 1)
                neighbors.Add(tile);
        }
        return neighbors;
    }
}
