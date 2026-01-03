using UnityEngine;
using UnityEditor;

public class FixPlayerEnemyCollision : EditorWindow
{
    [MenuItem("Tools/Fix Player Can't Stand on Enemies")]
    public static void FixEnemyCollision()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player == null)
        {
            Debug.LogError("❌ Player not found! Make sure a GameObject is tagged as 'Player'.");
            return;
        }
        
        PlayerController playerController = player.GetComponent<PlayerController>();
        
        if (playerController == null)
        {
            Debug.LogError("❌ PlayerController component not found on player!");
            return;
        }
        
        SerializedObject so = new SerializedObject(playerController);
        SerializedProperty groundLayerProp = so.FindProperty("whatIsGround");
        
        if (groundLayerProp == null)
        {
            Debug.LogError("❌ Could not find 'whatIsGround' property!");
            return;
        }
        
        int currentMask = groundLayerProp.intValue;
        int enemyLayer = LayerMask.NameToLayer("Attackable");
        
        if (enemyLayer == -1)
        {
            Debug.LogWarning("⚠️ 'Attackable' layer not found. Checking for 'Enemy' layer...");
            enemyLayer = LayerMask.NameToLayer("Enemy");
        }
        
        if (enemyLayer == -1)
        {
            Debug.LogError("❌ Neither 'Attackable' nor 'Enemy' layer found! Please create one in Project Settings → Tags and Layers.");
            return;
        }
        
        int enemyLayerMask = 1 << enemyLayer;
        
        if ((currentMask & enemyLayerMask) != 0)
        {
            int newMask = currentMask & ~enemyLayerMask;
            groundLayerProp.intValue = newMask;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(playerController);
            
            string layerName = LayerMask.LayerToName(enemyLayer);
            Debug.Log($"✅ Fixed! Removed '{layerName}' layer from ground detection. Player can no longer stand on enemies!");
            
            Debug.Log("💡 Also make sure in Edit → Project Settings → Physics 2D:");
            Debug.Log("   - Player layer and Attackable/Enemy layer collision is UNCHECKED");
            Debug.Log("   - This prevents physics-based collisions while still allowing damage detection");
        }
        else
        {
            string layerName = LayerMask.LayerToName(enemyLayer);
            Debug.Log($"✅ Already fixed! '{layerName}' layer is not in ground detection.");
            Debug.Log($"Current ground layers: {GetLayerNames(currentMask)}");
        }
        
        Debug.Log("\n📋 Additional Steps:");
        Debug.Log("1. Go to Edit → Project Settings → Physics 2D");
        Debug.Log("2. In the Layer Collision Matrix, UNCHECK the box where 'Player' and 'Attackable' (or 'Enemy') intersect");
        Debug.Log("3. This prevents the player from physically colliding with enemies");
    }
    
    private static string GetLayerNames(int layerMask)
    {
        string result = "";
        for (int i = 0; i < 32; i++)
        {
            if ((layerMask & (1 << i)) != 0)
            {
                string layerName = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(layerName))
                {
                    result += layerName + ", ";
                }
            }
        }
        return result.TrimEnd(',', ' ');
    }
}
