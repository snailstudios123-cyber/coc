using UnityEngine;
using UnityEditor;
using System.Linq;

public class QuickFixBeamSpell : EditorWindow
{
    [MenuItem("Tools/Quick Fix Beam Spell (ONE CLICK)")]
    public static void QuickFix()
    {
        if (!EditorUtility.DisplayDialog("Quick Fix Beam Spell", 
            "This will:\n\n" +
            "1. Slice your Laser_sheet.png into frames\n" +
            "2. Assign sprites to the BeamEffect prefab\n" +
            "3. Make the beam visible\n\n" +
            "Continue?", "Yes!", "Cancel"))
        {
            return;
        }
        
        EditorUtility.DisplayProgressBar("Quick Fix", "Slicing laser sprite...", 0.3f);
        
        try
        {
            Debug.Log("Starting Quick Fix Beam Spell...");
            
            Sprite[] laserSprites = SliceLaserSprite();
            
            Debug.Log($"Slicing result: {(laserSprites != null ? laserSprites.Length.ToString() : "NULL")} sprites");
            
            if (laserSprites == null || laserSprites.Length == 0)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Error", "Failed to slice the laser sprite sheet!\n\nMake sure Laser_sheet.png exists at:\nAssets/Animation/Spells Animation/Laser_sheet.png", "OK");
                return;
            }
            
            EditorUtility.DisplayProgressBar("Quick Fix", "Updating BeamEffect prefab...", 0.6f);
            
            FixBeamEffectPrefab(laserSprites);
            
            EditorUtility.DisplayProgressBar("Quick Fix", "Finalizing...", 0.9f);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            EditorUtility.ClearProgressBar();
            
            Debug.Log($"Quick Fix Complete! {laserSprites.Length} sprites assigned.");
            
            EditorUtility.DisplayDialog("Success!", 
                $"Beam Spell Fixed!\n\n" +
                $"✓ Laser sprite sliced into {laserSprites.Length} frames\n" +
                $"✓ BeamEffect prefab updated\n" +
                $"✓ Beam is now visible\n\n" +
                $"Press V in Play mode to charge and fire!", "Awesome!");
        }
        catch (System.Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Error", "Quick fix failed:\n\n" + e.Message, "OK");
            Debug.LogError("Quick Fix Error: " + e);
            Debug.LogError(e.StackTrace);
        }
    }
    
    private static Sprite[] SliceLaserSprite()
    {
        string path = "Assets/Animation/Spells Animation/Laser_sheet.png";
        
        Debug.Log($"Loading texture from: {path}");
        
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        
        if (texture == null)
        {
            Debug.LogError("Laser_sheet.png not found at: " + path);
            return null;
        }
        
        Debug.Log($"Texture loaded: {texture.width}x{texture.height}");
        
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        
        if (importer == null)
        {
            Debug.LogError("Could not get TextureImporter for: " + path);
            return null;
        }
        
        Debug.Log("Setting texture import settings...");
        
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.spritePixelsPerUnit = 100;
        
        int frameCount = 15;
        int frameWidth = 300;
        int frameHeight = 100;
        
        Debug.Log($"Creating {frameCount} sprite slices...");
        
        SpriteMetaData[] spritesheet = new SpriteMetaData[frameCount];
        
        for (int i = 0; i < frameCount; i++)
        {
            SpriteMetaData smd = new SpriteMetaData();
            smd.name = "Laser_" + i;
            smd.rect = new Rect(0, texture.height - (i + 1) * frameHeight, frameWidth, frameHeight);
            smd.alignment = (int)SpriteAlignment.Center;
            spritesheet[i] = smd;
            Debug.Log($"  Sprite {i}: {smd.name} at {smd.rect}");
        }
        
        importer.spritesheet = spritesheet;
        
        Debug.Log("Saving and reimporting texture...");
        
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
        
        AssetDatabase.Refresh();
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        
        Debug.Log("Loading sliced sprites...");
        
        Object[] allSprites = AssetDatabase.LoadAllAssetsAtPath(path);
        Sprite[] sprites = allSprites.OfType<Sprite>().OrderBy(s => s.name).ToArray();
        
        Debug.Log($"Loaded {sprites.Length} sprites from sheet");
        
        foreach (Sprite s in sprites)
        {
            Debug.Log($"  - {s.name}");
        }
        
        return sprites;
    }
    
    private static void FixBeamEffectPrefab(Sprite[] sprites)
    {
        string prefabPath = "Assets/Prefabs/BeamEffect 1.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            Debug.LogError("BeamEffect 1 prefab not found at: " + prefabPath);
            return;
        }
        
        BeamVisualEffect beamEffect = prefab.GetComponent<BeamVisualEffect>();
        SpriteRenderer spriteRenderer = prefab.GetComponent<SpriteRenderer>();
        
        if (beamEffect == null)
        {
            Debug.LogError("BeamVisualEffect component not found on prefab");
            return;
        }
        
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on prefab");
            return;
        }
        
        SerializedObject beamSO = new SerializedObject(beamEffect);
        SerializedProperty framesProperty = beamSO.FindProperty("beamFrames");
        
        framesProperty.ClearArray();
        framesProperty.arraySize = sprites.Length;
        
        for (int i = 0; i < sprites.Length; i++)
        {
            framesProperty.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
        }
        
        beamSO.FindProperty("frameRate").floatValue = 12f;
        beamSO.FindProperty("beamLength").floatValue = 10f;
        beamSO.FindProperty("beamColor").colorValue = new Color(0f, 1f, 1f, 1f);
        beamSO.FindProperty("fadeOutDuration").floatValue = 0.2f;
        
        beamSO.ApplyModifiedProperties();
        
        SerializedObject spriteSO = new SerializedObject(spriteRenderer);
        spriteSO.FindProperty("m_Sprite").objectReferenceValue = sprites[0];
        spriteSO.FindProperty("m_Color").colorValue = new Color(0f, 1f, 1f, 1f);
        spriteSO.FindProperty("m_SortingOrder").intValue = 10;
        spriteSO.ApplyModifiedProperties();
        
        EditorUtility.SetDirty(prefab);
        
        Debug.Log("BeamEffect prefab updated successfully with " + sprites.Length + " frames");
    }
}
