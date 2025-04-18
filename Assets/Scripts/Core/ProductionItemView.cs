using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ProductionItemView : MonoBehaviour
{
    [SerializeField] private Image buildingImage;
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private Button productionButton;

    private string buildingType;
    private Action<string> onClickCallback;

    public void Initialize(string buildingType, Action<string> callback)
    {
        this.buildingType = buildingType;
        this.onClickCallback = callback;

        buildingNameText.text = buildingType;

        // Get building data to set image
        Building buildingPrefab = BuildingFactory.Instance.CreateBuilding(buildingType);
        if (buildingPrefab != null)
        {
            BuildingData buildingData = buildingPrefab.GetBuildingData();
            buildingImage.sprite = buildingData.buildingSprite;
            Destroy(buildingPrefab.gameObject); // Clean up the temporary prefab
        }

        productionButton.onClick.RemoveAllListeners();
        productionButton.onClick.AddListener(() => onClickCallback?.Invoke(buildingType));
    }
}