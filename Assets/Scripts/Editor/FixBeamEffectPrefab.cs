using UnityEngine;
using UnityEditor;
using System.Linq;

public class FixBeamEffectPrefab : EditorWindow
{
    [MenuItem("Tools/Fix Beam Effect Prefab")]
    public static void ShowWindow()
    {
        if (EditorUtility.DisplayDialog("Fix Beam Effect", 
            "This will fix the BeamEffect prefab by:\n\n" +
            "1. Removing null sprites from the frames array\n" +
            "2. Assigning the first valid sprite to the SpriteRenderer\n" +
            "3. Making the beam visible\n\n" +
            "Continue?", "Yes", "Cancel"))
        {
            FixPrefab();
        }
    }
    
    private static void FixPrefab()
    {
        string prefabPath = "Assets/Prefabs/BeamEffect 1.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not find BeamEffect 1 prefab at:\n" + prefabPath, "OK");
            return;
        }
        
        BeamVisualEffect beamEffect = prefab.GetComponent<BeamVisualEffect>();
        SpriteRenderer spriteRenderer = prefab.GetComponent<SpriteRenderer>();
        
        if (beamEffect == null || spriteRenderer == null)
        {
            EditorUtility.DisplayDialog("Error", "BeamEffect prefab is missing required components!", "OK");
            return;
        }
        
        SerializedObject so = new SerializedObject(beamEffect);
        SerializedProperty framesProperty = so.FindProperty("beamFrames");
        
        Sprite[] validSprites = new Sprite[framesProperty.arraySize];
        int validCount = 0;
        
        for (int i = 0; i < framesProperty.arraySize; i++)
        {
            Sprite sprite = framesProperty.GetArrayElementAtIndex(i).objectReferenceValue as Sprite;
            if (sprite != null)
            {
                validSprites[validCount] = sprite;
                validCount++;
            }
        }
        
        if (validCount == 0)
        {
            EditorUtility.DisplayDialog("Error", "No valid sprites found in the beam frames array!\n\nPlease run the Beam Spell Setup tool first.", "OK");
            return;
        }
        
        framesProperty.ClearArray();
        framesProperty.arraySize = validCount;
        
        for (int i = 0; i < validCount; i++)
        {
            framesProperty.GetArrayElementAtIndex(i).objectReferenceValue = validSprites[i];
        }
        
        so.ApplyModifiedProperties();
        
        SerializedObject spriteRendererSO = new SerializedObject(spriteRenderer);
        spriteRendererSO.FindProperty("m_Sprite").objectReferenceValue = validSprites[0];
        spriteRendererSO.FindProperty("m_Color").colorValue = new Color(0, 1, 1, 1);
        spriteRendererSO.ApplyModifiedProperties();
        
        EditorUtility.SetDirty(prefab);
        AssetDatabase.SaveAssets();
        
        EditorUtility.DisplayDialog("Success!", 
            $"Beam Effect Fixed!\n\n" +
            $"• Removed null sprites\n" +
            $"• {validCount} valid frames assigned\n" +
            $"• First sprite assigned to SpriteRenderer\n" +
            $"• Color set to cyan\n\n" +
            $"The beam should now be visible when fired!", "OK");
        
        Selection.activeObject = prefab;
        EditorGUIUtility.PingObject(prefab);
    }
}
