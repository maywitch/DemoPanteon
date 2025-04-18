using UnityEngine;
using System.Collections.Generic;

public class Soldier : MonoBehaviour
{
    [SerializeField] private UnitData unitData;

    private int currentHealth;
    private List<Vector3> path; // Yol noktalarý
    private int currentPathIndex = 0;
    private float lastAttackTime = 0f;

    // Hedefler
    private Vector3 moveTarget;
    private bool isMoving = false;
    private Soldier targetUnit;
    private Building targetBuilding;

    private void Start()
    {
        currentHealth = unitData.maxHealth;
    }

    // Her frame GameManager tarafýndan çaðrýlýr
    public void UpdateUnit()
    {
        // Hareket
        if (isMoving && path != null && path.Count > 0)
        {
            MoveAlongPath();
        }

        // Saldýrý kontrolü
        AttackTargetIfInRange();
    }

    // Hedefe hareket etme emri
    public void MoveTo(Vector3 targetPosition)
    {
        path = Pathfinder.Instance.FindPath(transform.position, targetPosition);

        if (path.Count > 0)
        {
            isMoving = true;
            currentPathIndex = 0;
            moveTarget = targetPosition;

            targetUnit = null;
            targetBuilding = null;
        }
    }

    // Yol boyunca hareket etme
    private void MoveAlongPath()
    {
        if (currentPathIndex >= path.Count)
        {
            isMoving = false;
            return;
        }

        // Bir sonraki noktayý al
        Vector3 targetPosition = path[currentPathIndex];
        Vector3 direction = targetPosition - transform.position;

        if (direction.magnitude > 0.1f)
        {
            // 2D için açýyý hesapla (sprite’ýn öne baktýðý yön genellikle yukarý)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // Hedefe doðru hareket et (x ve y eksenlerinde)
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            unitData.moveSpeed * Time.deltaTime
        );

        // Hedefe yeterince yaklaþtýysa, sonraki nokta
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentPathIndex++;
        }
    }

    // Saldýrý iþlemleri (deðiþiklik yapýlmadý)
    private void AttackTargetIfInRange()
    {
        if (Time.time < lastAttackTime + 1f / unitData.attackSpeed)
            return;

        if (targetUnit != null)
        {
            float distance = Vector3.Distance(transform.position, targetUnit.transform.position);
            if (distance <= unitData.attackRange)
            {
                targetUnit.TakeDamage(unitData.damage);
                lastAttackTime = Time.time;
                Debug.Log($"{unitData.unitName} attacked {targetUnit.GetUnitData().unitName} for {unitData.damage} damage");
            }
        }
        else if (targetBuilding != null)
        {
            float distance = Vector3.Distance(transform.position, targetBuilding.transform.position);
            if (distance <= unitData.attackRange + 1f)
            {
                targetBuilding.TakeDamage(unitData.damage);
                lastAttackTime = Time.time;
                Debug.Log($"{unitData.unitName} attacked {targetBuilding.GetBuildingData().buildingName} for {unitData.damage} damage");
            }
        }
    }

    public void AttackUnit(Soldier target)
    {
        targetUnit = target;
        targetBuilding = null;
        MoveTo(target.transform.position);
    }

    public void AttackBuilding(Building target)
    {
        targetBuilding = target;
        targetUnit = null;
        MoveTo(target.transform.position);
    }

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
        GameManager.Instance.RemoveUnit(this);
        Destroy(gameObject);
    }

    public UnitData GetUnitData()
    {
        return unitData;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return unitData.maxHealth;
    }

    public int GetDamage()
    {
        return unitData.damage;
    }
}
