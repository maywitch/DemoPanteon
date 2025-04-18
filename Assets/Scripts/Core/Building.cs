using UnityEngine;
using System.Collections.Generic;

public class Building : MonoBehaviour
{
    [SerializeField] private BuildingData buildingData;

    private int currentHealth;
    private float constructionProgress; // 0-1 arasý

    private void Start()
    {
        currentHealth = buildingData.maxHealth;
    }

    // Her framede çaðrýlýr (GameManager tarafýndan)
    public void UpdateBuilding()
    {
        // Ýnþaat ilerlemesi (eðer tam inþa edilmediyse)
        if (constructionProgress < 1f)
        {
            constructionProgress += Time.deltaTime / buildingData.constructionTime;
            constructionProgress = Mathf.Clamp01(constructionProgress);

            // Ýnþaat tamamlandý mý?
            if (constructionProgress >= 1f)
            {
                OnConstructionCompleted();
            }
        }
    }

    // Ýnþaat tamamlandýðýnda
    private void OnConstructionCompleted()
    {
        Debug.Log($"{buildingData.buildingName} inþaatý tamamlandý!");
        // Ýnþaat tamamlandýðýnda, bina alanýnýn grid veya tile üzerinde iþgalde olduðunu kesinleþtirin.
        Grid grid = FindObjectOfType<Grid>();
        if (grid != null)
        {
            grid.UpdateGridNodes(transform.position, GetBuildingSize(), false);
        }
    }


    // Hasar alma
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        GameManager.Instance.RemoveBuilding(this);

        Tile tile = TileManager.Instance.GetTileAtPosition(transform.position);
        if (tile != null)
        {
            tile.SetOccupied(false);
        }

        Grid grid = FindObjectOfType<Grid>();
        if (grid != null)
        {
            grid.UpdateGridNodes(transform.position, GetBuildingSize(), true);
        }

        Destroy(gameObject);
    }


    // Bina verisini almak için
    public BuildingData GetBuildingData()
    {
        return buildingData;
    }

    // Binanýn boyutunu almak için
    public Vector2 GetBuildingSize()
    {
        return buildingData.buildingSize;
    }

    // Mevcut saðlýk deðerini almak için
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    // Maksimum saðlýk deðerini almak için
    public int GetMaxHealth()
    {
        return buildingData.maxHealth;
    }

    // Binanýn birim üretip üretemeyeceðini kontrol et
    public bool CanProduceUnits()
    {
        return buildingData.produceableUnitTypes.Count > 0;
    }

    // Üretilebilir birimler listesini al
    public List<UnitData> GetProduceableUnits()
    {
        List<UnitData> units = new List<UnitData>();

        foreach (string unitType in buildingData.produceableUnitTypes)
        {
            Soldier unitPrefab = UnitFactory.Instance.CreateUnit(unitType, Vector3.zero);
            if (unitPrefab != null)
            {
                units.Add(unitPrefab.GetUnitData());
                Destroy(unitPrefab.gameObject); // Geçici prefabý temizle
            }
        }

        return units;
    }

    public Vector3 GetSpawnPosition()
    {
        // Bina çevresinde rastgele bir nokta (2D için x-y düzlemi, z = 0)
        float offset = Mathf.Max(buildingData.buildingSize.x, buildingData.buildingSize.y) * 0.75f;
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
        return transform.position + randomDirection * offset;
    }

}