using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InformationPanelView : MonoBehaviour
{
    [SerializeField] private GameObject buildingInfoPanel;
    [SerializeField] private GameObject unitInfoPanel;
    [SerializeField] private GameObject noSelectionPanel;

    [Header("Building Info")]
    [SerializeField] private Image buildingImage;
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private TextMeshProUGUI buildingHealthText;
    [SerializeField] private Transform productsContainer;
    [SerializeField] private Button productButtonPrefab;

    [Header("Unit Info")]
    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private TextMeshProUGUI unitHealthText;
    [SerializeField] private TextMeshProUGUI unitDamageText;

    private Building selectedBuilding;
    private List<Button> productButtons = new List<Button>();

    private void Start()
    {
        ClearPanel();
    }

    public void ShowBuildingInfo(Building building)
    {
        selectedBuilding = building;

        // Hide other panels
        unitInfoPanel.SetActive(false);
        noSelectionPanel.SetActive(false);
        buildingInfoPanel.SetActive(true);

        // Update building info
        BuildingData buildingData = building.GetBuildingData();
        buildingNameText.text = buildingData.buildingName;
        buildingImage.sprite = buildingData.buildingSprite;
        buildingHealthText.text = $"Health: {building.GetCurrentHealth()}/{building.GetMaxHealth()}";

        // Clear existing product buttons
        foreach (Button button in productButtons)
        {
            Destroy(button.gameObject);
        }
        productButtons.Clear();

        // Add product buttons if building can produce units
        if (building.CanProduceUnits())
        {
            List<UnitData> produceableUnits = building.GetProduceableUnits();
            foreach (UnitData unitData in produceableUnits)
            {
                Button productButton = Instantiate(productButtonPrefab, productsContainer);
                productButton.GetComponentInChildren<TextMeshProUGUI>().text = unitData.unitName;

                // Set image if there's an image component
                Image buttonImage = productButton.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.sprite = unitData.unitSprite;
                }

                // Add click event
                productButton.onClick.AddListener(() => ProduceUnit(unitData.unitName));

                productButtons.Add(productButton);
            }
        }
    }

    public void ShowUnitInfo(Soldier unit)
    {
        // Hide other panels
        buildingInfoPanel.SetActive(false);
        noSelectionPanel.SetActive(false);
        unitInfoPanel.SetActive(true);

        // Update unit info
        UnitData unitData = unit.GetUnitData();
        unitNameText.text = unitData.unitName;
        unitImage.sprite = unitData.unitSprite;
        unitHealthText.text = $"Health: {unit.GetCurrentHealth()}/{unit.GetMaxHealth()}";
        unitDamageText.text = $"Damage: {unit.GetDamage()}";
    }

    public void ClearPanel()
    {
        buildingInfoPanel.SetActive(false);
        unitInfoPanel.SetActive(false);
        noSelectionPanel.SetActive(true);
        selectedBuilding = null;
    }

    public void ProduceUnit(string unitType)
    {
        if (selectedBuilding != null)
        {
            // Spawn unit at building's spawn point
            Vector3 spawnPosition = selectedBuilding.GetSpawnPosition();
            Soldier unit = UnitFactory.Instance.CreateUnit(unitType, spawnPosition);

            if (unit != null)
            {
                // Add unit to game manager
                GameManager.Instance.AddUnit(unit);
            }
        }
    }
}