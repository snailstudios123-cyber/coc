using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class VineShieldSetup : EditorWindow
{
    [MenuItem("Tools/Setup Vine Shield")]
    public static void SetupVineShieldAutomatically()
    {
        CreateVineShieldPrefab();
        AssignToPlayer();
        EditorUtility.DisplayDialog("Success", "Vine Shield setup complete!\n\nPress T to activate the shield.", "OK");
    }

    private static void CreateVineShieldPrefab()
    {
        GameObject vineShield = new GameObject("VineShieldPrefab");
        
        SpriteRenderer sr = vineShield.AddComponent<SpriteRenderer>();
        Sprite vineSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Audio/Textures/Screenshot_2025-11-06_161336-removebg-preview.png");
        sr.sprite = vineSprite;
        sr.color = new Color(0.2f, 0.8f, 0.2f, 1f);
        sr.sortingOrder = 10;
        
        BoxCollider2D collider = vineShield.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(1f, 1f);
        
        VineShield vineShieldScript = vineShield.AddComponent<VineShield>();
        
        vineShield.transform.localScale = new Vector3(1.5f, 3f, 1f);
        
        string prefabFolder = "Assets/Prefabs";
        if (!AssetDatabase.IsValidFolder(prefabFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        string prefabPath = "Assets/Prefabs/VineShieldPrefab.prefab";
        PrefabUtility.SaveAsPrefabAsset(vineShield, prefabPath);
        
        DestroyImmediate(vineShield);
        
        Debug.Log("Vine Shield Prefab created at: " + prefabPath);
    }

    private static void AssignToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player == null)
        {
            Debug.LogError("Player GameObject not found! Make sure your player has the 'Player' tag.");
            return;
        }
        
        GrapplingHook grapplingHook = player.GetComponent<GrapplingHook>();
        
        if (grapplingHook == null)
        {
            Debug.LogError("GrapplingHook component not found on Player!");
            return;
        }
        
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/VineShieldPrefab.prefab");
        
        SerializedObject so = new SerializedObject(grapplingHook);
        so.FindProperty("vineShieldPrefab").objectReferenceValue = prefab;
        so.FindProperty("vineShieldDuration").floatValue = 1f;
        so.FindProperty("vineShieldDistance").floatValue = 1.5f;
        so.FindProperty("vineShieldScale").vector2Value = new Vector2(1.5f, 3f);
        so.ApplyModifiedProperties();
        
        EditorUtility.SetDirty(player);
        EditorSceneManager.MarkSceneDirty(player.scene);
        
        Debug.Log("Vine Shield assigned to Player: " + player.name);
    }
}
