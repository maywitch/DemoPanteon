using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Building", menuName = "RTS/Building Data")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public string description;
    public Sprite buildingSprite;
    public int maxHealth = 100;
    public Vector2 buildingSize = new Vector2(2, 2);
    public int constructionTime = 5;
    public List<string> produceableUnitTypes = new List<string>();
}