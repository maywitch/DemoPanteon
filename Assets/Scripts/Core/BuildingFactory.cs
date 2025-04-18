using UnityEngine;
using System.Collections.Generic;

public class BuildingFactory : MonoBehaviour
{
    public static BuildingFactory Instance { get; private set; }

    [SerializeField] private List<Building> buildingPrefabs = new List<Building>();

    private Dictionary<string, Building> buildingDictionary = new Dictionary<string, Building>();

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

    // Bina prefablarýný sözlüðe ekle
    private void InitializeDictionary()
    {
        foreach (Building prefab in buildingPrefabs)
        {
            BuildingData data = prefab.GetBuildingData();
            buildingDictionary[data.buildingName] = prefab;
        }
    }

    // Bina oluþtur
    public Building CreateBuilding(string buildingType)
    {
        if (buildingDictionary.TryGetValue(buildingType, out Building prefab))
        {
            Debug.LogWarning($"IPEKKBina türü '{buildingType}' Olustu!");

            return Instantiate(prefab);
        }

        Debug.LogWarning($"Bina türü '{buildingType}' bulunamadý!");
        return null;
    }
}