// MonoBehaviour'den t�retilmi� ProductionMenuView s�n�f�
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionMenuView : MonoBehaviour
{
    // Unity Inspector'da atanacak bile�enler
    [SerializeField] private RectTransform contentPanel; // ��erik paneli
    [SerializeField] private ScrollRect scrollRect;      // Kayd�rma bile�eni
    [SerializeField] private ProductionItemView itemPrefab; // �retim ��esi prefab�
    [SerializeField] private InputManager inputManager;  // Giri� y�neticisi

    // Kullan�labilir bina t�rleri
    [SerializeField] private string[] buildingTypes = { "Barracks", "PowerPlant" };

    // Aktif ve havuzlanm�� �retim ��eleri
    private List<ProductionItemView> activeItems = new List<ProductionItemView>();
    private List<ProductionItemView> pooledItems = new List<ProductionItemView>();

    // Oyun ba�lad���nda
    void Start()
    {
        //// �rne�in, contentPanel alt�ndaki t�m child�lar� temizle:
        //foreach (Transform child in contentPanel)
        //{
        //    Destroy(child.gameObject);
        //}
        // Sonra �retim men�s�n� initialize edin:
        //InitializeProductionMenu();
    }

    // �retim men�s�n� ba�latma
    private void InitializeProductionMenu()
    {
        // �retim ��eleri i�in havuz olu�tur (performans i�in)
        for (int i = 0; i < 10; i++)
        {
            ProductionItemView item = Instantiate(itemPrefab, contentPanel);
            item.gameObject.SetActive(false);
            pooledItems.Add(item);
        }

        // Her bina t�r� i�in men�ye bir ��e ekle
        foreach (string buildingType in buildingTypes)
        {
            AddProductionItem(buildingType);
        }
    }

    // �retim ��esi ekleme
    private void AddProductionItem(string buildingType)
    {
        ProductionItemView item = GetPooledItem(); // Havuzdan bir ��e al
        item.Initialize(buildingType, OnProductionItemClicked); // ��eyi ba�lat
        item.gameObject.SetActive(true); // ��eyi etkinle�tir
        activeItems.Add(item); // Aktif ��elere ekle
    }

    // Havuzdan bir ��e alma
    private ProductionItemView GetPooledItem()
    {
        if (pooledItems.Count > 0)
        {
            // Havuzda ��e varsa ilkini al
            ProductionItemView item = pooledItems[0];
            pooledItems.RemoveAt(0);
            return item;
        }
        else
        {
            // Havuz bo�sa yeni ��e olu�tur
            return Instantiate(itemPrefab, contentPanel);
        }
    }

    // ��eyi havuza geri d�nd�rme
    private void ReturnItemToPool(ProductionItemView item)
    {
        item.gameObject.SetActive(false);
        activeItems.Remove(item);
        pooledItems.Add(item);
    }

    // �retim ��esine t�kland���nda
    public void OnProductionItemClicked(string buildingType)
    {
        // Bina fabrikas�ndan se�ilen t�rde bir bina olu�tur
        Building buildingPrefab = BuildingFactory.Instance.CreateBuilding(buildingType);
        if (buildingPrefab != null)
        {
            // Binay� yerle�tirmeye ba�la
            inputManager.StartPlacingBuilding(buildingPrefab);
        }
    }
}