using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }

    // Sahnede bulunan t�m Tile nesnelerini tutar
    private Tile[] tiles;

    // H�cre boyutu; tile�lar aras�ndaki mesafe (�rn. 1f)
    public float cellSize = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Tile nesneleri TileManager��n child'lar� olarak yer almal�.
            tiles = GetComponentsInChildren<Tile>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Verilen world position�a en yak�n tile�� d�nd�r�r.
    /// E�er t�klama ile tile merkezi aras�ndaki mesafe cellSize'dan b�y�kse null d�ner.
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

        // E�er en yak�n tile merkezi ile t�klanan konum aras�ndaki mesafe,
        // cellSize'dan b�y�kse, ge�erli bir tile olarak kabul etme.
        if (minDistance > cellSize)
            return null;

        return closestTile;
    }

    /// <summary>
    /// Verilen tile��n kom�ular�n� (8 y�nde) d�nd�r�r.
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
