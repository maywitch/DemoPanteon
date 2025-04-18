using UnityEngine;
using System.Collections.Generic;

public class UnitFactory : MonoBehaviour
{
    public static UnitFactory Instance { get; private set; }

    [SerializeField] private List<Soldier> unitPrefabs = new List<Soldier>();

    private Dictionary<string, Soldier> unitDictionary = new Dictionary<string, Soldier>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Birim prefablar�n� s�zl��e ekle
    private void InitializeDictionary()
    {
        foreach (Soldier prefab in unitPrefabs)
        {
            UnitData data = prefab.GetUnitData();
            unitDictionary[data.unitName] = prefab;
        }
    }

    // Birim olu�tur
    public Soldier CreateUnit(string unitType, Vector3 position)
    {
        if (unitDictionary.TryGetValue(unitType, out Soldier prefab))
        {
            Soldier newUnit = Instantiate(prefab, position, Quaternion.identity);
            return newUnit;
        }

        Debug.LogWarning($"Birim t�r� '{unitType}' bulunamad�!");
        return null;
    }
}