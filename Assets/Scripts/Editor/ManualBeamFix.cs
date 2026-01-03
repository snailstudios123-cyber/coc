using UnityEngine;
using UnityEditor;

public class ManualBeamFix : EditorWindow
{
    [MenuItem("Tools/MANUAL Beam Fix - Select Sprites")]
    public static void ShowWindow()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/BeamEffect 1.prefab");
        
        if (prefab == null)
        {
            EditorUtility.DisplayDialog("Error", "BeamEffect 1 prefab not found!", "OK");
            return;
        }
        
        BeamVisualEffect beamEffect = prefab.GetComponent<BeamVisualEffect>();
        SpriteRenderer spriteRenderer = prefab.GetComponent<SpriteRenderer>();
        
        if (beamEffect == null || spriteRenderer == null)
        {
            EditorUtility.DisplayDialog("Error", "Components missing on prefab!", "OK");
            return;
        }
        
        SerializedObject beamSO = new SerializedObject(beamEffect);
        SerializedProperty framesProperty = beamSO.FindProperty("beamFrames");
        
        int validCount = 0;
        for (int i = 0; i < framesProperty.arraySize; i++)
        {
            if (framesProperty.GetArrayElementAtIndex(i).objectReferenceValue != null)
            {
                validCount++;
            }
        }
        
        if (validCount > 0)
        {
            Sprite firstValid = null;
            for (int i = 0; i < framesProperty.arraySize; i++)
            {
                Sprite spr = framesProperty.GetArrayElementAtIndex(i).objectReferenceValue as Sprite;
                if (spr != null)
                {
                    firstValid = spr;
                    break;
                }
            }
            
            if (firstValid != null)
            {
                Debug.Log($"Found {validCount} valid sprites. First sprite: {firstValid.name}");
                
                framesProperty.DeleteArrayElementAtIndex(0);
                
                beamSO.ApplyModifiedProperties();
                
                SerializedObject spriteSO = new SerializedObject(spriteRenderer);
                spriteSO.FindProperty("m_Sprite").objectReferenceValue = firstValid;
                spriteSO.ApplyModifiedProperties();
                
                EditorUtility.SetDirty(prefab);
                AssetDatabase.SaveAssets();
                
                EditorUtility.DisplayDialog("Fixed!", 
                    $"✓ Removed null sprite from array\n" +
                    $"✓ Assigned first sprite: {firstValid.name}\n" +
                    $"✓ Total valid frames: {validCount - 1}\n\n" +
                    $"Try firing the beam now!", "OK");
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Manual Setup Required", 
                "No valid sprites found!\n\n" +
                "MANUAL STEPS:\n\n" +
                "1. Select Laser_sheet.png in Project\n" +
                "2. In Inspector, change Sprite Mode to 'Multiple'\n" +
                "3. Click 'Sprite Editor'\n" +
                "4. Click 'Slice' > 'Grid By Cell Count'\n" +
                "5. Set Column=1, Row=15\n" +
                "6. Click 'Slice', then 'Apply'\n" +
                "7. Select BeamEffect 1 prefab\n" +
                "8. In Inspector, expand 'Beam Frames' array\n" +
                "9. Drag all 15 laser sprites into the array\n" +
                "10. Drag the first sprite to the Sprite Renderer's 'Sprite' field\n\n" +
                "Then run this tool again!", "OK");
        }
    }
}
