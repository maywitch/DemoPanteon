using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinder : MonoBehaviour
{
    public static Pathfinder Instance { get; private set; }
    [SerializeField] private TileManager tileManager;

    private void Awake()
    {
        Instance = this;
        if (tileManager == null)
            tileManager = FindObjectOfType<TileManager>();
    }

    public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // Ba�lang�� ve hedef pozisyonlar�n� ilgili tile'lara �eviriyoruz.
        Tile startTile = tileManager.GetTileAtPosition(startPos);
        Tile targetTile = tileManager.GetTileAtPosition(targetPos);

        if (startTile == null || targetTile == null)
        {
            return new List<Vector3>();
        }

        // A* algoritmas� i�in open set ve closed set olu�turuyoruz.
        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();

        openSet.Add(startTile);
        startTile.gCost = 0;
        startTile.hCost = GetDistance(startTile, targetTile);

        while (openSet.Count > 0)
        {
            // fCost'e g�re en d���k maliyetli tile'� se�iyoruz.
            Tile currentTile = openSet.OrderBy(t => t.fCost).ThenBy(t => t.hCost).First();
            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            // Hedefe ula�t�ysak yolu geri d�nd�r.
            if (currentTile == targetTile)
            {
                return RetracePath(startTile, targetTile);
            }

            // Kom�u tile'lar� kontrol et.
            foreach (Tile neighbor in tileManager.GetNeighbors(currentTile))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                int newMovementCostToNeighbor = currentTile.gCost + GetDistance(currentTile, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetTile);
                    neighbor.parent = currentTile;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        // Yol bulunamazsa bo� liste d�nd�r.
        return new List<Vector3>();
    }

    private List<Vector3> RetracePath(Tile startTile, Tile endTile)
    {
        List<Vector3> path = new List<Vector3>();
        Tile currentTile = endTile;

        while (currentTile != startTile)
        {
            path.Add(currentTile.transform.position);
            currentTile = currentTile.parent;
        }

        path.Reverse();
        return path;
    }

    private int GetDistance(Tile tileA, Tile tileB)
    {
        int dstX = Mathf.Abs(tileA.gridX - tileB.gridX);
        int dstY = Mathf.Abs(tileA.gridY - tileB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
