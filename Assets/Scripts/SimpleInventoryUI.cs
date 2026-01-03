using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleInventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private Button closeButton;
    
    private List<GameObject> slots = new List<GameObject>();
    
    void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseInventory);
        }
        
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }
    
    public void OpenInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
            RefreshInventory();
        }
    }
    
    public void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }
    
    void RefreshInventory()
    {
        foreach (GameObject slot in slots)
        {
            if (slot != null)
            {
                Destroy(slot);
            }
        }
        slots.Clear();
        
        if (InventoryManager.Instance == null)
        {
            return;
        }
        
        Dictionary<ItemData, int> inventory = InventoryManager.Instance.GetInventory();
        
        foreach (KeyValuePair<ItemData, int> entry in inventory)
        {
            ItemData data = entry.Key;
            int quantity = entry.Value;
            
            if (data == null)
            {
                continue;
            }
            
            GameObject slotObj = Instantiate(itemSlotPrefab, gridContainer);
            SimpleItemSlotUI slotUI = slotObj.GetComponent<SimpleItemSlotUI>();
            
            if (slotUI != null)
            {
                slotUI.SetItem(data, quantity);
            }
            
            slots.Add(slotObj);
        }
        
        Debug.Log($"Refreshed inventory with {slots.Count} items");
    }
}
