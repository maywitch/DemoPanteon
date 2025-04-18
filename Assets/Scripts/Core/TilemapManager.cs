using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    public static TilemapManager Instance { get; private set; }

    [SerializeField] private Tilemap tilemap;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (tilemap == null)
        {
            // Eðer Inspector’da atanmamýþsa, sahnede bir Tilemap bulmaya çalýþýn.
            tilemap = FindObjectOfType<Tilemap>();
        }
    }

    // World pozisyonunu, tile konumuna çevirir
    public Vector3Int WorldToTilePosition(Vector3 worldPosition)
    {
        return tilemap.WorldToCell(worldPosition);
    }

    // Tile pozisyonunu, world pozisyonuna çevirir (tile’ýn ortasýna)
    public Vector3 TileToWorldPosition(Vector3Int tilePosition)
    {
        Vector3 worldPos = tilemap.GetCellCenterWorld(tilePosition);
        return worldPos;
    }

    // Belirli bir tile alanýnda deðiþiklik yapmak için örnek metod:
    public void SetTile(Vector3Int tilePos, TileBase tile)
    {
        tilemap.SetTile(tilePos, tile);
    }
}
