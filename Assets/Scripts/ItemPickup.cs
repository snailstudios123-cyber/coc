using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int quantity = 1;
    [SerializeField] private bool autoPickup = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!autoPickup)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            PickupItem();
        }
    }

    public void PickupItem()
    {
        if (InventoryManager.Instance != null && itemData != null)
        {
            bool success = InventoryManager.Instance.AddItem(itemData, quantity);
            
            if (success)
            {
                Destroy(gameObject);
            }
        }
    }
}
