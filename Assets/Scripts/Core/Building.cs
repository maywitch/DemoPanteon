using UnityEngine;
using System.Collections.Generic;

public class Building : MonoBehaviour
{
    [SerializeField] private BuildingData buildingData;

    private int currentHealth;
    private float constructionProgress; // 0-1 aras�

    private void Start()
    {
        currentHealth = buildingData.maxHealth;
    }

    // Her framede �a�r�l�r (GameManager taraf�ndan)
    public void UpdateBuilding()
    {
        // �n�aat ilerlemesi (e�er tam in�a edilmediyse)
        if (constructionProgress < 1f)
        {
            constructionProgress += Time.deltaTime / buildingData.constructionTime;
            constructionProgress = Mathf.Clamp01(constructionProgress);

            // �n�aat tamamland� m�?
            if (constructionProgress >= 1f)
            {
                OnConstructionCompleted();
            }
        }
    }

    // �n�aat tamamland���nda
    private void OnConstructionCompleted()
    {
        Debug.Log($"{buildingData.buildingName} in�aat� tamamland�!");
        // �n�aat tamamland���nda, bina alan�n�n grid veya tile �zerinde i�galde oldu�unu kesinle�tirin.
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


    // Bina verisini almak i�in
    public BuildingData GetBuildingData()
    {
        return buildingData;
    }

    // Binan�n boyutunu almak i�in
    public Vector2 GetBuildingSize()
    {
        return buildingData.buildingSize;
    }

    // Mevcut sa�l�k de�erini almak i�in
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    // Maksimum sa�l�k de�erini almak i�in
    public int GetMaxHealth()
    {
        return buildingData.maxHealth;
    }

    // Binan�n birim �retip �retemeyece�ini kontrol et
    public bool CanProduceUnits()
    {
        return buildingData.produceableUnitTypes.Count > 0;
    }

    // �retilebilir birimler listesini al
    public List<UnitData> GetProduceableUnits()
    {
        List<UnitData> units = new List<UnitData>();

        foreach (string unitType in buildingData.produceableUnitTypes)
        {
            Soldier unitPrefab = UnitFactory.Instance.CreateUnit(unitType, Vector3.zero);
            if (unitPrefab != null)
            {
                units.Add(unitPrefab.GetUnitData());
                Destroy(unitPrefab.gameObject); // Ge�ici prefab� temizle
            }
        }

        return units;
    }

    public Vector3 GetSpawnPosition()
    {
        // Bina �evresinde rastgele bir nokta (2D i�in x-y d�zlemi, z = 0)
        float offset = Mathf.Max(buildingData.buildingSize.x, buildingData.buildingSize.y) * 0.75f;
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
        return transform.position + randomDirection * offset;
    }

}