using UnityEngine;

public class InventoryDebugger : MonoBehaviour
{
    [Header("Test Items")]
    [SerializeField] private ItemData testItem1;
    [SerializeField] private ItemData testItem2;
    [SerializeField] private ItemData testItem3;

    [Header("Settings")]
    [SerializeField] private bool addItemsOnStart = true;
    [SerializeField] private int quantityToAdd = 5;

    [Header("Debug Keys")]
    [SerializeField] private KeyCode addItem1Key = KeyCode.Alpha1;
    [SerializeField] private KeyCode addItem2Key = KeyCode.Alpha2;
    [SerializeField] private KeyCode addItem3Key = KeyCode.Alpha3;
    [SerializeField] private KeyCode clearInventoryKey = KeyCode.C;
    [SerializeField] private KeyCode printInventoryKey = KeyCode.P;

    private void Start()
    {
        if (addItemsOnStart)
        {
            AddTestItems();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(addItem1Key) && testItem1 != null)
        {
            InventoryManager.Instance?.AddItem(testItem1, quantityToAdd);
        }

        if (Input.GetKeyDown(addItem2Key) && testItem2 != null)
        {
            InventoryManager.Instance?.AddItem(testItem2, quantityToAdd);
        }

        if (Input.GetKeyDown(addItem3Key) && testItem3 != null)
        {
            InventoryManager.Instance?.AddItem(testItem3, quantityToAdd);
        }

        if (Input.GetKeyDown(clearInventoryKey))
        {
            ClearInventory();
        }

        if (Input.GetKeyDown(printInventoryKey))
        {
            PrintInventory();
        }
    }

    private void AddTestItems()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("[InventoryDebugger] InventoryManager.Instance is null!");
            return;
        }

        if (testItem1 != null)
        {
            InventoryManager.Instance.AddItem(testItem1, quantityToAdd);
        }

        if (testItem2 != null)
        {
            InventoryManager.Instance.AddItem(testItem2, quantityToAdd);
        }

        if (testItem3 != null)
        {
            InventoryManager.Instance.AddItem(testItem3, quantityToAdd);
        }

        Debug.Log("[InventoryDebugger] Test items added to inventory");
    }

    private void ClearInventory()
    {
        if (InventoryManager.Instance == null)
        {
            return;
        }

        var inventory = InventoryManager.Instance.GetInventory();
        var itemsToRemove = new System.Collections.Generic.List<ItemData>(inventory.Keys);

        foreach (var item in itemsToRemove)
        {
            int count = inventory[item];
            InventoryManager.Instance.RemoveItem(item, count);
        }

        Debug.Log("[InventoryDebugger] Inventory cleared");
    }

    private void PrintInventory()
    {
        if (InventoryManager.Instance == null)
        {
            return;
        }

        var inventory = InventoryManager.Instance.GetInventory();
        
        Debug.Log("=== INVENTORY CONTENTS ===");
        Debug.Log($"Total Items: {inventory.Count}");
        
        foreach (var entry in inventory)
        {
            Debug.Log($"- {entry.Key.itemName} x{entry.Value} ({entry.Key.itemType})");
        }
        
        Debug.Log("========================");
    }
}
