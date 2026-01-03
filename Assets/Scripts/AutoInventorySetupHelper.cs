using UnityEngine;
using UnityEditor;

public class AutoInventorySetupHelper : MonoBehaviour
{
    [MenuItem("Tools/Complete Inventory Setup Now")]
    public static void CompleteSetup()
    {
        Debug.Log("=== COMPLETING INVENTORY SETUP ===");
        
        GameObject inventoryManager = GameObject.Find("InventoryManager");
        GameObject inventoryPanel = GameObject.Find("InventoryPanel");
        GameObject pauseMenuManager = GameObject.Find("PauseMenuManager");
        
        if (inventoryManager == null || inventoryPanel == null || pauseMenuManager == null)
        {
            EditorUtility.DisplayDialog("Error", "Missing required objects. Please run 'Auto Setup Inventory System' first.", "OK");
            return;
        }
        
        InventoryManager invMgr = inventoryManager.GetComponent<InventoryManager>();
        InventoryUI invUI = inventoryPanel.GetComponent<InventoryUI>();
        PauseMenuManager pauseMgr = pauseMenuManager.GetComponent<PauseMenuManager>();
        
        ItemData healthPotion = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Items/HealthPotion.asset");
        ItemData manaPotion = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Items/ManaPotion.asset");
        ItemData ancientKey = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Items/AncientKey.asset");
        
        SerializedObject invMgrSO = new SerializedObject(invMgr);
        SerializedProperty allItemsProp = invMgrSO.FindProperty("allItems");
        allItemsProp.ClearArray();
        
        if (healthPotion != null)
        {
            allItemsProp.InsertArrayElementAtIndex(0);
            allItemsProp.GetArrayElementAtIndex(0).objectReferenceValue = healthPotion;
        }
        
        if (manaPotion != null)
        {
            allItemsProp.InsertArrayElementAtIndex(1);
            allItemsProp.GetArrayElementAtIndex(1).objectReferenceValue = manaPotion;
        }
        
        if (ancientKey != null)
        {
            allItemsProp.InsertArrayElementAtIndex(2);
            allItemsProp.GetArrayElementAtIndex(2).objectReferenceValue = ancientKey;
        }
        
        invMgrSO.ApplyModifiedProperties();
        Debug.Log("✓ Assigned 3 items to InventoryManager");
        
        SerializedObject pauseMgrSO = new SerializedObject(pauseMgr);
        pauseMgrSO.FindProperty("inventoryMenuPanel").objectReferenceValue = inventoryPanel;
        pauseMgrSO.ApplyModifiedProperties();
        Debug.Log("✓ Connected InventoryPanel to PauseMenuManager");
        
        GameObject debugger = GameObject.Find("InventoryDebugger");
        if (debugger == null)
        {
            debugger = new GameObject("InventoryDebugger");
            InventoryDebugger debugComponent = debugger.AddComponent<InventoryDebugger>();
            
            SerializedObject debugSO = new SerializedObject(debugComponent);
            debugSO.FindProperty("testItem1").objectReferenceValue = healthPotion;
            debugSO.FindProperty("testItem2").objectReferenceValue = manaPotion;
            debugSO.FindProperty("testItem3").objectReferenceValue = ancientKey;
            debugSO.FindProperty("addItemsOnStart").boolValue = true;
            debugSO.FindProperty("quantityToAdd").intValue = 5;
            debugSO.ApplyModifiedProperties();
            
            Debug.Log("✓ Created InventoryDebugger with test items");
        }
        
        EditorUtility.SetDirty(inventoryManager);
        EditorUtility.SetDirty(pauseMenuManager);
        if (debugger != null) EditorUtility.SetDirty(debugger);
        
        Debug.Log("=== SETUP COMPLETE! ===");
        EditorUtility.DisplayDialog("Success!", 
            "✅ Inventory System Fully Configured!\n\n" +
            "What's ready:\n" +
            "• InventoryManager has 3 items assigned\n" +
            "• PauseMenuManager connected to Inventory\n" +
            "• InventoryDebugger added to scene\n\n" +
            "Test it now:\n" +
            "1. Press Play\n" +
            "2. Press ESC → Click 'Inventory'\n" +
            "3. See your 3 items!\n" +
            "4. Press 1, 2, 3 to add more items", 
            "Let's Go!");
    }
}
