using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class FoliumTurboSpellSetup : EditorWindow
{
    private Sprite tornadoSprite;
    private RuntimeAnimatorController tornadoAnimator;
    
    [MenuItem("Tools/Setup Folium Turbo Spell")]
    public static void ShowWindow()
    {
        GetWindow<FoliumTurboSpellSetup>("Folium Turbo Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Folium Turbo Spell Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox("This tool will automatically set up the Folium Turbo spell system.", MessageType.Info);
        GUILayout.Space(10);
        
        tornadoSprite = (Sprite)EditorGUILayout.ObjectField("Tornado Sprite", tornadoSprite, typeof(Sprite), false);
        tornadoAnimator = (RuntimeAnimatorController)EditorGUILayout.ObjectField("Tornado Animator (Optional)", tornadoAnimator, typeof(RuntimeAnimatorController), false);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Setup Everything", GUILayout.Height(40)))
        {
            SetupFoliumTurboSpell();
        }
        
        GUILayout.Space(10);
        EditorGUILayout.HelpBox("What this tool does:\n" +
            "1. Creates tornado prefab in /Assets/Prefabs/\n" +
            "2. Creates SpellData asset in /Assets/ScriptableObjects/Spells/\n" +
            "3. Adds spell to SpellManager\n" +
            "4. Configures Player with FoliumTurboSpell component\n" +
            "5. Sets up all references and settings\n\n" +
            "Controls:\n" +
            "• Tap F = Offensive tornado (seeks nearest enemy, swirls them around)\n" +
            "• Hold F = Defensive tornado (stays on player, pushes enemies away)", MessageType.None);
    }

    private void SetupFoliumTurboSpell()
    {
        try
        {
            Debug.Log("=== Starting Folium Turbo Spell Setup ===");
            
            GameObject tornadoPrefab = CreateTornadoPrefab();
            SpellData spellData = CreateSpellDataAsset();
            AddSpellToManager(spellData);
            ConfigurePlayer(tornadoPrefab);
            
            Debug.Log("=== Folium Turbo Spell Setup Complete! ===");
            EditorUtility.DisplayDialog("Success", 
                "Folium Turbo spell has been successfully set up!\n\n" +
                "Controls:\n" +
                "• Tap F = Offensive tornado (seeks nearest enemy, swirls them around)\n" +
                "• Hold F = Defensive tornado (stays on player, pushes enemies away)", 
                "OK");
            
            Close();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Setup failed: {e.Message}");
            EditorUtility.DisplayDialog("Error", $"Setup failed: {e.Message}", "OK");
        }
    }

    private GameObject CreateTornadoPrefab()
    {
        Debug.Log("Creating tornado prefab...");
        
        string prefabPath = "Assets/Prefabs/FoliumTurboTornado.prefab";
        
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            Debug.Log("Tornado prefab already exists, using existing one.");
            return AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }
        
        if (!Directory.Exists("Assets/Prefabs"))
        {
            Directory.CreateDirectory("Assets/Prefabs");
        }
        
        GameObject tornado = new GameObject("FoliumTurboTornado");
        
        SpriteRenderer sr = tornado.AddComponent<SpriteRenderer>();
        sr.sprite = tornadoSprite;
        sr.sortingOrder = 5;
        
        Animator animator = tornado.AddComponent<Animator>();
        if (tornadoAnimator != null)
        {
            animator.runtimeAnimatorController = tornadoAnimator;
        }
        
        TornadoController controller = tornado.AddComponent<TornadoController>();
        
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(tornado, prefabPath);
        DestroyImmediate(tornado);
        
        Debug.Log($"Tornado prefab created at: {prefabPath}");
        return prefab;
    }

    private SpellData CreateSpellDataAsset()
    {
        Debug.Log("Creating SpellData asset...");
        
        string assetPath = "Assets/ScriptableObjects/Spells/FoliumTurbo.asset";
        
        SpellData existingAsset = AssetDatabase.LoadAssetAtPath<SpellData>(assetPath);
        if (existingAsset != null)
        {
            Debug.Log("SpellData already exists, using existing one.");
            return existingAsset;
        }
        
        if (!Directory.Exists("Assets/ScriptableObjects"))
        {
            Directory.CreateDirectory("Assets/ScriptableObjects");
        }
        if (!Directory.Exists("Assets/ScriptableObjects/Spells"))
        {
            Directory.CreateDirectory("Assets/ScriptableObjects/Spells");
        }
        
        SpellData spellData = ScriptableObject.CreateInstance<SpellData>();
        spellData.spellName = "Folium Turbo";
        spellData.description = "Summon a leaf tornado around you to blow enemies away from you. Or summon a tornado towards enemies to get them caught in them.";
        spellData.manaCost = 0.3f;
        spellData.isEquipped = false;
        
        AssetDatabase.CreateAsset(spellData, assetPath);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"SpellData created at: {assetPath}");
        return spellData;
    }

    private void AddSpellToManager(SpellData spellData)
    {
        Debug.Log("Adding spell to SpellManager...");
        
        SpellManager spellManager = FindObjectOfType<SpellManager>();
        
        if (spellManager == null)
        {
            Debug.LogWarning("SpellManager not found in scene. You'll need to add the spell manually.");
            return;
        }
        
        SerializedObject so = new SerializedObject(spellManager);
        SerializedProperty allSpellsProp = so.FindProperty("allSpells");
        
        bool alreadyAdded = false;
        for (int i = 0; i < allSpellsProp.arraySize; i++)
        {
            SerializedProperty element = allSpellsProp.GetArrayElementAtIndex(i);
            if (element.objectReferenceValue == spellData)
            {
                alreadyAdded = true;
                break;
            }
        }
        
        if (!alreadyAdded)
        {
            allSpellsProp.InsertArrayElementAtIndex(allSpellsProp.arraySize);
            allSpellsProp.GetArrayElementAtIndex(allSpellsProp.arraySize - 1).objectReferenceValue = spellData;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(spellManager);
            Debug.Log("Spell added to SpellManager.");
        }
        else
        {
            Debug.Log("Spell already in SpellManager.");
        }
    }

    private void ConfigurePlayer(GameObject tornadoPrefab)
    {
        Debug.Log("Configuring Player...");
        
        PlayerController player = FindObjectOfType<PlayerController>();
        
        if (player == null)
        {
            Debug.LogWarning("PlayerController not found in scene. You'll need to configure manually.");
            return;
        }
        
        FoliumTurboSpell foliumSpell = player.GetComponent<FoliumTurboSpell>();
        if (foliumSpell == null)
        {
            foliumSpell = player.gameObject.AddComponent<FoliumTurboSpell>();
            Debug.Log("Added FoliumTurboSpell component to Player.");
        }
        
        SerializedObject playerSO = new SerializedObject(player);
        SerializedProperty foliumTurboProp = playerSO.FindProperty("foliumTurbo");
        SerializedProperty foliumManaCostProp = playerSO.FindProperty("foliumTurboManaCost");
        SerializedProperty foliumManaPerSecondProp = playerSO.FindProperty("foliumTurboManaPerSecond");
        SerializedProperty holdTimeThresholdProp = playerSO.FindProperty("holdTimeThreshold");
        
        if (foliumTurboProp != null)
        {
            foliumTurboProp.objectReferenceValue = foliumSpell;
        }
        
        if (foliumManaCostProp != null)
        {
            foliumManaCostProp.floatValue = 0.3f;
        }
        
        if (foliumManaPerSecondProp != null)
        {
            foliumManaPerSecondProp.floatValue = 0.2f;
        }
        
        if (holdTimeThresholdProp != null)
        {
            holdTimeThresholdProp.floatValue = 0.3f;
        }
        
        playerSO.ApplyModifiedProperties();
        
        SerializedObject spellSO = new SerializedObject(foliumSpell);
        
        SerializedProperty tornadoPrefabProp = spellSO.FindProperty("tornadoPrefab");
        SerializedProperty offensiveDurationProp = spellSO.FindProperty("offensiveDuration");
        SerializedProperty pushForceProp = spellSO.FindProperty("pushForce");
        SerializedProperty seekSpeedProp = spellSO.FindProperty("seekSpeed");
        SerializedProperty swirlRadiusProp = spellSO.FindProperty("swirlRadius");
        SerializedProperty swirlSpeedProp = spellSO.FindProperty("swirlSpeed");
        SerializedProperty damagePerSecondProp = spellSO.FindProperty("damagePerSecond");
        SerializedProperty detectionRadiusProp = spellSO.FindProperty("detectionRadius");
        SerializedProperty enemyLayerProp = spellSO.FindProperty("enemyLayer");
        
        if (tornadoPrefabProp != null) tornadoPrefabProp.objectReferenceValue = tornadoPrefab;
        if (offensiveDurationProp != null) offensiveDurationProp.floatValue = 3f;
        if (pushForceProp != null) pushForceProp.floatValue = 15f;
        if (seekSpeedProp != null) seekSpeedProp.floatValue = 8f;
        if (swirlRadiusProp != null) swirlRadiusProp.floatValue = 1.5f;
        if (swirlSpeedProp != null) swirlSpeedProp.floatValue = 720f;
        if (damagePerSecondProp != null) damagePerSecondProp.floatValue = 5f;
        if (detectionRadiusProp != null) detectionRadiusProp.floatValue = 2f;
        
        if (enemyLayerProp != null)
        {
            int enemyLayerIndex = LayerMask.NameToLayer("Attackable");
            if (enemyLayerIndex >= 0)
            {
                enemyLayerProp.intValue = 1 << enemyLayerIndex;
                Debug.Log($"Set enemy layer to: Attackable (layer {enemyLayerIndex})");
            }
            else
            {
                Debug.LogWarning("'Attackable' layer not found. Using default layer mask.");
                enemyLayerProp.intValue = LayerMask.GetMask("Default");
            }
        }
        
        spellSO.ApplyModifiedProperties();
        
        EditorUtility.SetDirty(player);
        EditorUtility.SetDirty(foliumSpell);
        
        Debug.Log("Player configured successfully.");
    }
}
