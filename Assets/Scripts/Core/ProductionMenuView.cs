// MonoBehaviour'den türetilmiþ ProductionMenuView sýnýfý
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionMenuView : MonoBehaviour
{
    // Unity Inspector'da atanacak bileþenler
    [SerializeField] private RectTransform contentPanel; // Ýçerik paneli
    [SerializeField] private ScrollRect scrollRect;      // Kaydýrma bileþeni
    [SerializeField] private ProductionItemView itemPrefab; // Üretim öðesi prefabý
    [SerializeField] private InputManager inputManager;  // Giriþ yöneticisi

    // Kullanýlabilir bina türleri
    [SerializeField] private string[] buildingTypes = { "Barracks", "PowerPlant" };

    // Aktif ve havuzlanmýþ üretim öðeleri
    private List<ProductionItemView> activeItems = new List<ProductionItemView>();
    private List<ProductionItemView> pooledItems = new List<ProductionItemView>();

    // Oyun baþladýðýnda
    void Start()
    {
        //// Örneðin, contentPanel altýndaki tüm child’larý temizle:
        //foreach (Transform child in contentPanel)
        //{
        //    Destroy(child.gameObject);
        //}
        // Sonra üretim menüsünü initialize edin:
        //InitializeProductionMenu();
    }

    // Üretim menüsünü baþlatma
    private void InitializeProductionMenu()
    {
        // Üretim öðeleri için havuz oluþtur (performans için)
        for (int i = 0; i < 10; i++)
        {
            ProductionItemView item = Instantiate(itemPrefab, contentPanel);
            item.gameObject.SetActive(false);
            pooledItems.Add(item);
        }

        // Her bina türü için menüye bir öðe ekle
        foreach (string buildingType in buildingTypes)
        {
            AddProductionItem(buildingType);
        }
    }

    // Üretim öðesi ekleme
    private void AddProductionItem(string buildingType)
    {
        ProductionItemView item = GetPooledItem(); // Havuzdan bir öðe al
        item.Initialize(buildingType, OnProductionItemClicked); // Öðeyi baþlat
        item.gameObject.SetActive(true); // Öðeyi etkinleþtir
        activeItems.Add(item); // Aktif öðelere ekle
    }

    // Havuzdan bir öðe alma
    private ProductionItemView GetPooledItem()
    {
        if (pooledItems.Count > 0)
        {
            // Havuzda öðe varsa ilkini al
            ProductionItemView item = pooledItems[0];
            pooledItems.RemoveAt(0);
            return item;
        }
        else
        {
            // Havuz boþsa yeni öðe oluþtur
            return Instantiate(itemPrefab, contentPanel);
        }
    }

    // Öðeyi havuza geri döndürme
    private void ReturnItemToPool(ProductionItemView item)
    {
        item.gameObject.SetActive(false);
        activeItems.Remove(item);
        pooledItems.Add(item);
    }

    // Üretim öðesine týklandýðýnda
    public void OnProductionItemClicked(string buildingType)
    {
        // Bina fabrikasýndan seçilen türde bir bina oluþtur
        Building buildingPrefab = BuildingFactory.Instance.CreateBuilding(buildingType);
        if (buildingPrefab != null)
        {
            // Binayý yerleþtirmeye baþla
            inputManager.StartPlacingBuilding(buildingPrefab);
        }
    }
}