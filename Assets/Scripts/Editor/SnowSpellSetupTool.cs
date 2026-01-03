using UnityEngine;
using UnityEditor;

public class SnowSpellSetupTool : EditorWindow
{
    [MenuItem("GameObject/Spells/Create Falling Snow Spell", false, 10)]
    public static void CreateFallingSnowSpell()
    {
        GameObject snowSpell = new GameObject("FallingSnowSpell");
        SnowSpell snowSpellScript = snowSpell.AddComponent<SnowSpell>();
        
        GameObject snowflake = CreateSnowflakePrefab();
        
        SerializedObject serializedSpell = new SerializedObject(snowSpellScript);
        serializedSpell.FindProperty("damage").floatValue = 5f;
        serializedSpell.FindProperty("freezeDuration").floatValue = 3f;
        serializedSpell.FindProperty("snowfallRadius").floatValue = 5f;
        serializedSpell.FindProperty("snowfallDuration").floatValue = 5f;
        serializedSpell.FindProperty("spawnHeight").floatValue = 8f;
        serializedSpell.FindProperty("snowflakesPerSecond").intValue = 20;
        serializedSpell.FindProperty("snowflakeFallSpeed").floatValue = 2f;
        serializedSpell.FindProperty("snowflakePrefab").objectReferenceValue = snowflake;
        serializedSpell.ApplyModifiedProperties();
        
        if (Selection.activeTransform != null)
        {
            snowSpell.transform.SetParent(Selection.activeTransform);
        }
        
        snowSpell.transform.localPosition = Vector3.zero;
        
        EditorUtility.SetDirty(snowSpell);
        Selection.activeGameObject = snowSpell;
        
        EditorUtility.DisplayDialog(
            "Falling Snow Spell Created!",
            "Snow spell has been created!\n\n" +
            "The spell will:\n" +
            "• Summon snow falling from the sky\n" +
            "• Freeze enemies on contact\n" +
            "• Turn enemies blue\n" +
            "• Last for 5 seconds\n\n" +
            "Add a snowflake sprite to the snowflake prefab for better visuals!\n\n" +
            "Or use 'Tools → Setup Complete Snow Spell' for full setup!",
            "OK"
        );
        
        Debug.Log("Falling Snow Spell created!");
    }

    [MenuItem("Tools/Setup Complete Snow Spell")]
    public static void SetupCompleteSnowSpell()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/Spells"))
        {
            if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
            {
                AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
            }
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Spells");
        }

        GameObject snowflake = CreateSnowflakePrefab();
        string snowflakePath = "Assets/Prefabs/Snowflake.prefab";
        PrefabUtility.SaveAsPrefabAsset(snowflake, snowflakePath);
        DestroyImmediate(snowflake);
        
        GameObject snowflakePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(snowflakePath);

        GameObject snowSpell = new GameObject("FallingSnowSpell");
        SnowSpell snowSpellScript = snowSpell.AddComponent<SnowSpell>();
        
        SerializedObject serializedSpell = new SerializedObject(snowSpellScript);
        serializedSpell.FindProperty("damage").floatValue = 8f;
        serializedSpell.FindProperty("freezeDuration").floatValue = 4f;
        serializedSpell.FindProperty("snowfallRadius").floatValue = 6f;
        serializedSpell.FindProperty("snowfallDuration").floatValue = 6f;
        serializedSpell.FindProperty("spawnHeight").floatValue = 10f;
        serializedSpell.FindProperty("snowflakesPerSecond").intValue = 25;
        serializedSpell.FindProperty("snowflakeFallSpeed").floatValue = 2.5f;
        serializedSpell.FindProperty("snowflakePrefab").objectReferenceValue = snowflakePrefab;
        serializedSpell.FindProperty("createParticlesIfMissing").boolValue = true;
        serializedSpell.ApplyModifiedProperties();
        
        string prefabPath = "Assets/Prefabs/FallingSnowSpell.prefab";
        PrefabUtility.SaveAsPrefabAsset(snowSpell, prefabPath);
        DestroyImmediate(snowSpell);
        
        SpellData spellData = ScriptableObject.CreateInstance<SpellData>();
        spellData.spellName = "Blizzard";
        spellData.description = "Summons falling snow from the sky that freezes all enemies it touches, turning them blue and stopping them in their tracks.";
        spellData.manaCost = 25f;
        spellData.isEquipped = false;
        
        string spellDataPath = "Assets/ScriptableObjects/Spells/Blizzard.asset";
        AssetDatabase.CreateAsset(spellData, spellDataPath);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        GameObject snowSpellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                SerializedObject serializedPlayer = new SerializedObject(playerController);
                serializedPlayer.FindProperty("snowSpellPrefab").objectReferenceValue = snowSpellPrefab;
                serializedPlayer.FindProperty("snowSpellManaCost").floatValue = 0.5f;
                serializedPlayer.FindProperty("timeBetweenSnowCast").floatValue = 2f;
                serializedPlayer.ApplyModifiedProperties();
                
                EditorUtility.SetDirty(player);
                
                EditorUtility.DisplayDialog(
                    "Snow Spell Setup Complete!",
                    "Successfully created and connected to Player:\n\n" +
                    "✓ Snowflake Prefab: " + snowflakePath + "\n" +
                    "✓ Falling Snow Spell: " + prefabPath + "\n" +
                    "✓ Spell Data: " + spellDataPath + "\n" +
                    "✓ Connected to Player: " + player.name + "\n\n" +
                    "Press Q to cast the snow spell!\n\n" +
                    "The spell:\n" +
                    "• Summons snow falling from the sky\n" +
                    "• Freezes enemies and turns them blue\n" +
                    "• Costs 0.5 mana\n" +
                    "• 2 second cooldown\n\n" +
                    "You can add a snowflake sprite to the Snowflake prefab for better visuals!",
                    "OK"
                );
                
                Debug.Log("=== SNOW SPELL FULLY INTEGRATED ===");
                Debug.Log("Player: " + player.name);
                Debug.Log("Cast Key: Q");
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "Snow Spell Created!",
                    "Snow spell created but Player doesn't have PlayerController!\n\n" +
                    "✓ Snowflake Prefab: " + snowflakePath + "\n" +
                    "✓ Falling Snow Spell: " + prefabPath + "\n" +
                    "✓ Spell Data: " + spellDataPath + "\n\n" +
                    "Manually assign the FallingSnowSpell prefab to your player's Snow Spell field.",
                    "OK"
                );
            }
        }
        else
        {
            EditorUtility.DisplayDialog(
                "Snow Spell Created!",
                "Snow spell created but no Player found in scene!\n\n" +
                "✓ Snowflake Prefab: " + snowflakePath + "\n" +
                "✓ Falling Snow Spell: " + prefabPath + "\n" +
                "✓ Spell Data: " + spellDataPath + "\n\n" +
                "Add a Player with tag 'Player' and assign the FallingSnowSpell prefab to the Snow Spell field in PlayerController.",
                "OK"
            );
        }
        
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        Debug.Log("Snowflake Prefab: " + snowflakePath);
        Debug.Log("Snow Spell Prefab: " + prefabPath);
        Debug.Log("Spell Data: " + spellDataPath);
    }

    private static GameObject CreateSnowflakePrefab()
    {
        GameObject snowflake = new GameObject("Snowflake");
        
        SpriteRenderer sr = snowflake.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.9f, 0.95f, 1f, 0.9f);
        sr.sortingOrder = 9;
        
        CircleCollider2D collider = snowflake.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.15f;
        
        Rigidbody2D rb = snowflake.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.isKinematic = true;
        
        Snowflake snowflakeScript = snowflake.AddComponent<Snowflake>();
        
        return snowflake;
    }
}
