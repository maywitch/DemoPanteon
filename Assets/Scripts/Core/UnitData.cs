using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "RTS/Unit Data")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public string description;
    public Sprite unitSprite;
    public int maxHealth = 50;
    public float moveSpeed = 3f;
    public int damage = 10;
    public float attackRange = 1f;
    public float attackSpeed = 1f;
}