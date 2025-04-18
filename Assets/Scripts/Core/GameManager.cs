using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Eski Grid referansı yerine TileManager kullanacağız.
    [SerializeField] private TileManager tileManager;
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private InformationPanelView informationPanel;
    [SerializeField] private InputManager inputManager;

    private List<Building> buildings = new List<Building>();
    private List<Soldier> units = new List<Soldier>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Eğer Inspector’da tileManager atanmamışsa otomatik bulabiliriz.
            if (tileManager == null)
                tileManager = FindObjectOfType<TileManager>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddBuilding(Building building)
    {
        // Ghost modundan çıkarken collider'ı yeniden etkinleştir
        Collider2D col = building.GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;

        buildings.Add(building);
        Debug.Log($"AddBuilding -> Bina konumu: {building.transform.position}");

        // TileManager'dan bina konumuna en yakın tile'ı al
        Tile nearestTile = TileManager.Instance.GetTileAtPosition(building.transform.position);
        if (nearestTile != null)
        {
            // Bina, tile'ın merkezine hizalanır
            building.transform.position = nearestTile.transform.position;
            // Tile'ı işgalde olarak işaretle
            nearestTile.SetOccupied(true);
        }

        // Grid kullanıyorsanız, ilgili grid hücrelerini güncelleyin
        Grid grid = FindObjectOfType<Grid>();
        if (grid != null)
        {
            grid.UpdateGridNodes(building.transform.position, building.GetBuildingSize(), false);
        }
    }


    public void RemoveBuilding(Building building)
    {
        buildings.Remove(building);
        // Silindikten sonra tile'i yeniden boş kabul edebilirsiniz.
    }

    public void AddUnit(Soldier unit)
    {
        units.Add(unit);
    }

    public void RemoveUnit(Soldier unit)
    {
        units.Remove(unit);
    }

    public void ShowSelectedObject(MonoBehaviour selectedObject)
    {
        //informationPanel.ClearPanel();
        if (selectedObject is Building building)
        {
            informationPanel.ShowBuildingInfo(building);
        }
        else if (selectedObject is Soldier unit)
        {
            informationPanel.ShowUnitInfo(unit);
        }
    }

    private void Update()
    {
        foreach (Soldier unit in units.ToArray())
        {
            if (unit != null)
            {
                unit.UpdateUnit();
            }
        }

        foreach (Building building in buildings.ToArray())
        {
            if (building != null)
            {
                building.UpdateBuilding();
            }
        }
    }
}
