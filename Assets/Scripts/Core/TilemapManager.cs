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
            // E�er Inspector�da atanmam��sa, sahnede bir Tilemap bulmaya �al���n.
            tilemap = FindObjectOfType<Tilemap>();
        }
    }

    // World pozisyonunu, tile konumuna �evirir
    public Vector3Int WorldToTilePosition(Vector3 worldPosition)
    {
        return tilemap.WorldToCell(worldPosition);
    }

    // Tile pozisyonunu, world pozisyonuna �evirir (tile��n ortas�na)
    public Vector3 TileToWorldPosition(Vector3Int tilePosition)
    {
        Vector3 worldPos = tilemap.GetCellCenterWorld(tilePosition);
        return worldPos;
    }

    // Belirli bir tile alan�nda de�i�iklik yapmak i�in �rnek metod:
    public void SetTile(Vector3Int tilePos, TileBase tile)
    {
        tilemap.SetTile(tilePos, tile);
    }
}
