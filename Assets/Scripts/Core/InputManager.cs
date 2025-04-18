using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask groundLayer;     // Tiles için
    [SerializeField] private LayerMask selectableLayer; // Buildings ve Units için
    [SerializeField] private TileManager tileManager;

    private Building buildingToPlace;
    private MonoBehaviour selectedObject;
    private bool hasPlacedBuilding = false;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Debug.Log("InputManager: mainCamera = " + mainCamera.name);
        if (tileManager != null)
            Debug.Log("InputManager: TileManager bulundu");
        else
            Debug.LogWarning("InputManager: TileManager bulunamadý!");
    }

    private void Update()
    {
        if (!Input.GetMouseButton(0))
            hasPlacedBuilding = false;

        if (buildingToPlace != null)
            HandleBuildingPlacement();
        else
            HandleSelection();
    }

    public void StartPlacingBuilding(Building buildingPrefab)
    {
        if (buildingToPlace != null)
            Destroy(buildingToPlace.gameObject);

        buildingToPlace = Instantiate(buildingPrefab);
        Debug.Log("Ghost oluþturuldu: " + buildingToPlace.name);

        foreach (var col in buildingToPlace.GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        foreach (var sr in buildingToPlace.GetComponentsInChildren<SpriteRenderer>())
        {
            var mat = new Material(sr.material);
            mat.color = new Color(1f, 1f, 1f, 0.5f);
            sr.material = mat;
        }

        SetLayerRecursively(buildingToPlace.gameObject, LayerMask.NameToLayer("Ignore Raycast"));
    }

    public void CancelPlacingBuilding()
    {
        if (buildingToPlace != null)
        {
            Destroy(buildingToPlace.gameObject);
            buildingToPlace = null;
        }
    }

    private void HandleBuildingPlacement()
    {
        if (hasPlacedBuilding)
            return;

        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        var hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, groundLayer);
        if (hit.collider == null) return;

        Debug.Log("Raycast hit ground: " + hit.collider.name);
        Vector3 pos = hit.point;

        var tile = tileManager.GetTileAtPosition(pos);
        if (tile != null)
        {
            pos = tile.transform.position;
            Debug.Log("Tile bulundu: " + tile.name + " at " + pos);
        }
        else
        {
            Debug.Log("Tile bulunamadý, ghost pozisyonu: " + pos);
        }

        buildingToPlace.transform.position = pos;

        bool canPlace = tile != null && tile.IsClear();
        Debug.Log("CanPlace: " + canPlace);

        foreach (var sr in buildingToPlace.GetComponentsInChildren<SpriteRenderer>())
            sr.material.color = canPlace ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);

        if (Input.GetMouseButtonDown(0) && canPlace)
        {
            hasPlacedBuilding = true;

            foreach (var col in buildingToPlace.GetComponentsInChildren<Collider2D>())
                col.enabled = true;

            int selectLayer = LayerMask.NameToLayer("Buildings");
            SetLayerRecursively(buildingToPlace.gameObject, selectLayer);

            foreach (var sr in buildingToPlace.GetComponentsInChildren<SpriteRenderer>())
                sr.material.color = Color.white;

            GameManager.Instance.AddBuilding(buildingToPlace);
            Debug.Log("Bina yerleþtirildi: " + buildingToPlace.name);
            buildingToPlace = null;
        }
    }

    private void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 point2D = new Vector2(worldPoint.x, worldPoint.y);

            var hits = Physics2D.OverlapPointAll(point2D, selectableLayer);
            if (hits != null && hits.Length > 0)
            {
                foreach (var col in hits)
                    Debug.Log($"OverlapPointAll hit: {col.name} (layer: {LayerMask.LayerToName(col.gameObject.layer)})");

                var building = hits[0].GetComponentInParent<Building>();
                if (building != null)
                {
                    selectedObject = building;
                    GameManager.Instance.ShowSelectedObject(building);
                    Debug.Log("Bina seçildi: " + building.name);
                    return;
                }

                var unit = hits[0].GetComponentInParent<Soldier>();
                if (unit != null)
                {
                    selectedObject = unit;
                    GameManager.Instance.ShowSelectedObject(unit);
                    Debug.Log("Ünite seçildi: " + unit.name);
                    return;
                }
            }

            selectedObject = null;
            GameManager.Instance.ShowSelectedObject(null);
            Debug.Log("Seçim sýfýrlandý, boþ alana týklandý.");
        }

        if (Input.GetMouseButtonDown(1) && selectedObject is Soldier selectedSoldier)
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hitSelectable = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, selectableLayer);

            if (hitSelectable.collider != null)
            {
                var building = hitSelectable.collider.GetComponentInParent<Building>();
                if (building != null)
                {
                    selectedSoldier.AttackBuilding(building);
                    Debug.Log($"Asker saldýrý komutu verdi (binaya): {building.GetBuildingData().buildingName}");
                    return;
                }

                var otherSoldier = hitSelectable.collider.GetComponentInParent<Soldier>();
                if (otherSoldier != null && otherSoldier != selectedSoldier)
                {
                    selectedSoldier.AttackUnit(otherSoldier);
                    Debug.Log($"Asker saldýrý komutu verdi (üniteye): {otherSoldier.GetUnitData().unitName}");
                    return;
                }
            }

            RaycastHit2D hitGround = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, groundLayer);
            if (hitGround.collider != null)
            {
                selectedSoldier.MoveTo(hitGround.point);
                Debug.Log($"Asker hareket komutu: {hitGround.point}");
            }
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, newLayer);
    }
}
