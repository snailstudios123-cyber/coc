using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private List<ItemData> allItems = new List<ItemData>();
    [SerializeField] private int maxInventorySlots = 30;

    private Dictionary<ItemData, int> inventory = new Dictionary<ItemData, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public List<ItemData> GetAllItems()
    {
        return allItems;
    }

    public Dictionary<ItemData, int> GetInventory()
    {
        return inventory;
    }

    public bool HasItem(ItemData item)
    {
        return inventory.ContainsKey(item) && inventory[item] > 0;
    }

    public int GetItemCount(ItemData item)
    {
        if (inventory.ContainsKey(item))
        {
            return inventory[item];
        }
        return 0;
    }

    public bool AddItem(ItemData item, int quantity = 1)
    {
        if (item == null || quantity <= 0)
        {
            return false;
        }

        if (inventory.ContainsKey(item))
        {
            int currentCount = inventory[item];
            int newCount = currentCount + quantity;

            if (newCount > item.maxStackSize)
            {
                inventory[item] = item.maxStackSize;
                return false;
            }

            inventory[item] = newCount;
        }
        else
        {
            if (inventory.Count >= maxInventorySlots)
            {
                Debug.Log("Inventory is full!");
                return false;
            }

            inventory[item] = Mathf.Min(quantity, item.maxStackSize);
        }

        Debug.Log($"Added {quantity} x {item.itemName} to inventory");
        return true;
    }

    public bool RemoveItem(ItemData item, int quantity = 1)
    {
        if (item == null || quantity <= 0)
        {
            return false;
        }

        if (!inventory.ContainsKey(item))
        {
            return false;
        }

        int currentCount = inventory[item];
        if (currentCount < quantity)
        {
            return false;
        }

        inventory[item] = currentCount - quantity;

        if (inventory[item] <= 0)
        {
            inventory.Remove(item);
        }

        Debug.Log($"Removed {quantity} x {item.itemName} from inventory");
        return true;
    }

    public bool UseItem(ItemData item)
    {
        if (item == null || !item.isConsumable)
        {
            return false;
        }

        if (!HasItem(item))
        {
            return false;
        }

        Debug.Log($"Used item: {item.itemName}");
        RemoveItem(item, 1);
        return true;
    }

    public int GetInventoryCount()
    {
        return inventory.Count;
    }

    public bool IsFull()
    {
        return inventory.Count >= maxInventorySlots;
    }
}
