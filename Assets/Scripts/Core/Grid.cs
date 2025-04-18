using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private Vector2 gridSize = new Vector2(50, 50);
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private bool showGrid = false;

    private Node[,] nodes;
    private int gridSizeX, gridSizeY;

    public void InitializeGrid()
    {
        // Calculate grid dimensions
        gridSizeX = Mathf.RoundToInt(gridSize.x / cellSize);
        gridSizeY = Mathf.RoundToInt(gridSize.y / cellSize);

        // Create grid of nodes
        nodes = new Node[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPosition = GridToWorldPosition(x, y);
                bool walkable = true; // Default to walkable, will update when buildings are placed
                nodes[x, y] = new Node(walkable, worldPosition, x, y);
            }
        }
    }

    public bool IsWithinGridBounds(Vector2 worldPosition, Vector2 size)
    {
        // Convert to grid coordinates
        Vector2Int gridPos = WorldToGridPosition(worldPosition);
        Vector2Int sizeInCells = new Vector2Int(Mathf.CeilToInt(size.x / cellSize), Mathf.CeilToInt(size.y / cellSize));

        // Check if position + size is within grid bounds
        if (gridPos.x < 0 || gridPos.y < 0 ||
            gridPos.x + sizeInCells.x > gridSizeX ||
            gridPos.y + sizeInCells.y > gridSizeY)
        {
            return false;
        }

        return true;
    }

    public void UpdateGridNodes(Vector2 position, Vector2 size, bool walkable)
    {
        // Convert to grid coordinates
        Vector2Int gridPos = WorldToGridPosition(position);
        Vector2Int sizeInCells = new Vector2Int(Mathf.CeilToInt(size.x / cellSize), Mathf.CeilToInt(size.y / cellSize));

        // Update nodes
        for (int x = gridPos.x; x < gridPos.x + sizeInCells.x; x++)
        {
            for (int y = gridPos.y; y < gridPos.y + sizeInCells.y; y++)
            {
                if (x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY)
                {
                    nodes[x, y].walkable = walkable;
                }
            }
        }
    }

    public Vector2Int WorldToGridPosition(Vector2 worldPosition)
    {
        float percentX = (worldPosition.x + gridSize.x / 2) / gridSize.x;
        float percentY = (worldPosition.y + gridSize.y / 2) / gridSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorldPosition(int x, int y)
    {
        float worldX = x * cellSize - gridSize.x / 2 + cellSize / 2;
        float worldY = y * cellSize - gridSize.y / 2 + cellSize / 2;

        return new Vector3(worldX, worldY, 0);
    }

    public Node NodeFromWorldPosition(Vector3 worldPosition)
    {
        Vector2Int gridPos = WorldToGridPosition(worldPosition);

        if (gridPos.x >= 0 && gridPos.x < gridSizeX && gridPos.y >= 0 && gridPos.y < gridSizeY)
        {
            return nodes[gridPos.x, gridPos.y];
        }

        return null;
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        // Check 8 directions
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(nodes[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    private void OnDrawGizmos()
    {
        if (!showGrid || nodes == null)
            return;

        foreach (Node node in nodes)
        {
            Gizmos.color = node.walkable ? Color.white : Color.red;
            Gizmos.DrawCube(node.worldPosition, Vector3.one * cellSize * 0.9f);
        }
    }
}

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost; // Cost from start
    public int hCost; // Heuristic cost to end
    public Node parent;

    public int fCost => gCost + hCost;

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }
}