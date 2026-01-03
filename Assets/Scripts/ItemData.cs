using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    
    [TextArea(3, 6)]
    public string description;
    
    public Sprite icon;
    
    public ItemType itemType;
    
    public int maxStackSize = 1;
    
    public bool isConsumable = false;
    
    public bool isKeyItem = false;
}

public enum ItemType
{
    Consumable,
    KeyItem,
    Equipment,
    Material,
    Quest
}
