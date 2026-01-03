using UnityEngine;
using UnityEditor;
using System.IO;

public class BeamSpellSetupTool : EditorWindow
{
    private GameObject playerObject;
    private Texture2D laserSpriteSheet;
    private Sprite glowSprite;
    private int framesCount = 15;
    private int frameWidth = 300;
    private int frameHeight = 100;
    
    [MenuItem("Tools/Beam Spell Setup")]
    public static void ShowWindow()
    {
        BeamSpellSetupTool window = GetWindow<BeamSpellSetupTool>("Beam Spell Setup");
        window.minSize = new Vector2(400, 300);
        window.Show();
    }
    
    private void OnEnable()
    {
        string laserPath = "Assets/Animation/Spells Animation/Laser_sheet.png";
        laserSpriteSheet = AssetDatabase.LoadAssetAtPath<Texture2D>(laserPath);
        
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            playerObject = players[0];
        }
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Beam Spell Automatic Setup", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox("This tool will automatically set up the beam spell system for you.", MessageType.Info);
        EditorGUILayout.Space();
        
        playerObject = EditorGUILayout.ObjectField("Player GameObject", playerObject, typeof(GameObject), true) as GameObject;
        laserSpriteSheet = EditorGUILayout.ObjectField("Laser Sprite Sheet", laserSpriteSheet, typeof(Texture2D), false) as Texture2D;
        glowSprite = EditorGUILayout.ObjectField("Glow Sprite (Optional)", glowSprite, typeof(Sprite), false) as Sprite;
        
        EditorGUILayout.Space();
        GUILayout.Label("Sprite Sheet Settings", EditorStyles.boldLabel);
        framesCount = EditorGUILayout.IntField("Number of Frames", framesCount);
        frameWidth = EditorGUILayout.IntField("Frame Width", frameWidth);
        frameHeight = EditorGUILayout.IntField("Frame Height", frameHeight);
        
        EditorGUILayout.Space();
        
        GUI.enabled = playerObject != null && laserSpriteSheet != null;
        
        if (GUILayout.Button("Setup Beam Spell", GUILayout.Height(40)))
        {
            SetupBeamSpell();
        }
        
        GUI.enabled = true;
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This will:\n• Slice the laser sprite sheet\n• Create beam effect prefab\n• Create charge effect prefab\n• Add BeamSpell component to player\n• Wire everything together", MessageType.None);
    }
    
    private void SetupBeamSpell()
    {
        if (playerObject == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a Player GameObject!", "OK");
            return;
        }
        
        if (laserSpriteSheet == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign the Laser Sprite Sheet!", "OK");
            return;
        }
        
        EditorUtility.DisplayProgressBar("Beam Spell Setup", "Setting up beam spell...", 0f);
        
        try
        {
            EditorUtility.DisplayProgressBar("Beam Spell Setup", "Slicing sprite sheet...", 0.2f);
            Sprite[] laserFrames = SliceSpriteSheet();
            
            EditorUtility.DisplayProgressBar("Beam Spell Setup", "Creating beam effect prefab...", 0.4f);
            GameObject beamPrefab = CreateBeamEffectPrefab(laserFrames);
            
            EditorUtility.DisplayProgressBar("Beam Spell Setup", "Creating charge effect prefab...", 0.6f);
            GameObject chargePrefab = CreateChargeEffectPrefab();
            
            EditorUtility.DisplayProgressBar("Beam Spell Setup", "Adding component to player...", 0.8f);
            SetupPlayerComponent(beamPrefab, chargePrefab);
            
            EditorUtility.DisplayProgressBar("Beam Spell Setup", "Finalizing...", 1f);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Success", "Beam Spell setup completed successfully!\n\nThe BeamSpell component has been added to your player.\n\nPress V to charge and release to fire!", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Error", "Setup failed: " + e.Message, "OK");
            Debug.LogError("Beam Spell Setup Error: " + e.Message);
        }
    }
    
    private Sprite[] SliceSpriteSheet()
    {
        string path = AssetDatabase.GetAssetPath(laserSpriteSheet);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.spritePixelsPerUnit = 100;
            
            SpriteMetaData[] spritesheet = new SpriteMetaData[framesCount];
            
            for (int i = 0; i < framesCount; i++)
            {
                SpriteMetaData smd = new SpriteMetaData();
                smd.name = "Laser_" + i;
                smd.rect = new Rect(0, laserSpriteSheet.height - (i + 1) * frameHeight, frameWidth, frameHeight);
                smd.alignment = (int)SpriteAlignment.Center;
                spritesheet[i] = smd;
            }
            
            importer.spritesheet = spritesheet;
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }
        
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);
        Sprite[] laserSprites = new Sprite[framesCount];
        int spriteIndex = 0;
        
        foreach (Object obj in sprites)
        {
            if (obj is Sprite && spriteIndex < framesCount)
            {
                laserSprites[spriteIndex] = obj as Sprite;
                spriteIndex++;
            }
        }
        
        return laserSprites;
    }
    
    private GameObject CreateBeamEffectPrefab(Sprite[] frames)
    {
        string prefabPath = "Assets/Prefabs/BeamEffect.prefab";
        
        EnsureDirectoryExists("Assets/Prefabs");
        
        GameObject beamObj = new GameObject("BeamEffect");
        
        SpriteRenderer sr = beamObj.AddComponent<SpriteRenderer>();
        sr.sprite = frames[0];
        sr.sortingLayerName = "Default";
        sr.sortingOrder = 10;
        
        BeamVisualEffect beamEffect = beamObj.AddComponent<BeamVisualEffect>();
        
        SerializedObject so = new SerializedObject(beamEffect);
        SerializedProperty framesProperty = so.FindProperty("beamFrames");
        framesProperty.arraySize = frames.Length;
        
        for (int i = 0; i < frames.Length; i++)
        {
            framesProperty.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
        }
        
        so.FindProperty("frameRate").floatValue = 12f;
        so.FindProperty("beamLength").floatValue = 10f;
        so.FindProperty("beamColor").colorValue = new Color(0f, 1f, 1f, 1f);
        so.FindProperty("fadeOutDuration").floatValue = 0.2f;
        
        so.ApplyModifiedProperties();
        
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existingPrefab != null)
        {
            AssetDatabase.DeleteAsset(prefabPath);
        }
        
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(beamObj, prefabPath);
        DestroyImmediate(beamObj);
        
        return prefab;
    }
    
    private GameObject CreateChargeEffectPrefab()
    {
        string prefabPath = "Assets/Prefabs/ChargeEffect.prefab";
        
        EnsureDirectoryExists("Assets/Prefabs");
        
        GameObject chargeObj = new GameObject("ChargeEffect");
        
        SpriteRenderer sr = chargeObj.AddComponent<SpriteRenderer>();
        
        if (glowSprite != null)
        {
            sr.sprite = glowSprite;
        }
        else
        {
            sr.sprite = CreateCircleSprite();
        }
        
        sr.sortingLayerName = "Default";
        sr.sortingOrder = 5;
        sr.color = new Color(0f, 1f, 1f, 0.8f);
        
        BeamChargeEffect chargeEffect = chargeObj.AddComponent<BeamChargeEffect>();
        
        SerializedObject so = new SerializedObject(chargeEffect);
        so.FindProperty("glowSprite").objectReferenceValue = sr;
        so.FindProperty("minScale").floatValue = 0.5f;
        so.FindProperty("maxScale").floatValue = 1.5f;
        so.FindProperty("pulseSpeed").floatValue = 3f;
        so.FindProperty("chargeColor").colorValue = new Color(0f, 1f, 1f, 0.8f);
        so.ApplyModifiedProperties();
        
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existingPrefab != null)
        {
            AssetDatabase.DeleteAsset(prefabPath);
        }
        
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(chargeObj, prefabPath);
        DestroyImmediate(chargeObj);
        
        return prefab;
    }
    
    private Sprite CreateCircleSprite()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = 1f - Mathf.Clamp01(distance / radius);
                alpha = Mathf.Pow(alpha, 2f);
                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        string texturePath = "Assets/Materials/BeamGlow.png";
        EnsureDirectoryExists("Assets/Materials");
        
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(texturePath, bytes);
        AssetDatabase.Refresh();
        
        TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Bilinear;
            importer.SaveAndReimport();
        }
        
        return AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);
    }
    
    private void SetupPlayerComponent(GameObject beamPrefab, GameObject chargePrefab)
    {
        BeamSpell beamSpell = playerObject.GetComponent<BeamSpell>();
        
        if (beamSpell == null)
        {
            beamSpell = playerObject.AddComponent<BeamSpell>();
        }
        
        Transform beamOrigin = playerObject.transform.Find("BeamOrigin");
        if (beamOrigin == null)
        {
            GameObject originObj = new GameObject("BeamOrigin");
            originObj.transform.SetParent(playerObject.transform);
            originObj.transform.localPosition = new Vector3(0.5f, 0f, 0f);
            beamOrigin = originObj.transform;
        }
        
        SerializedObject so = new SerializedObject(beamSpell);
        
        so.FindProperty("manaPerSecond").floatValue = 0.2f;
        so.FindProperty("maxChargeTime").floatValue = 2f;
        so.FindProperty("minimumHoldTime").floatValue = 0.2f;
        so.FindProperty("beamDuration").floatValue = 0.5f;
        so.FindProperty("baseDamagePerSecond").floatValue = 30f;
        so.FindProperty("maxDamageMultiplier").floatValue = 3f;
        
        so.FindProperty("beamRange").floatValue = 10f;
        so.FindProperty("beamWidth").floatValue = 0.5f;
        so.FindProperty("enemyLayer").intValue = LayerMask.GetMask("Attackable");
        
        so.FindProperty("chargeEffect").objectReferenceValue = chargePrefab;
        so.FindProperty("beamEffect").objectReferenceValue = beamPrefab;
        so.FindProperty("beamOrigin").objectReferenceValue = beamOrigin;
        
        so.ApplyModifiedProperties();
        
        EditorUtility.SetDirty(playerObject);
    }
    
    private void EnsureDirectoryExists(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parentFolder = Path.GetDirectoryName(path).Replace("\\", "/");
            string folderName = Path.GetFileName(path);
            
            if (!AssetDatabase.IsValidFolder(parentFolder))
            {
                EnsureDirectoryExists(parentFolder);
            }
            
            AssetDatabase.CreateFolder(parentFolder, folderName);
        }
    }
}
