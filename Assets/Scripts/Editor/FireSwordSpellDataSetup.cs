using UnityEngine;
using UnityEditor;

public class FireSwordSpellDataSetup : EditorWindow
{
    [MenuItem("Tools/Fire Sword/Create Spell Data")]
    public static void CreateFireSwordSpellData()
    {
        string assetPath = "Assets/ScriptableObjects/Spells/FireSword.asset";
        
        SpellData existingAsset = AssetDatabase.LoadAssetAtPath<SpellData>(assetPath);
        if (existingAsset != null)
        {
            Debug.Log("Fire Sword spell data already exists!");
            
            Selection.activeObject = existingAsset;
            EditorGUIUtility.PingObject(existingAsset);
            return;
        }
        
        SpellData spellData = ScriptableObject.CreateInstance<SpellData>();
        spellData.spellName = "Fire Sword";
        spellData.description = "Hold C to charge mana into your blade. Release to unleash a devastating rapid-strike fire sword attack that hits multiple times!";
        spellData.manaCost = 0.15f;
        spellData.isEquipped = false;
        
        Sprite icon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Blink/Art/Icons/Classes/Elementalist/Pyromancer/Pyromancer7.png");
        if (icon != null)
        {
            spellData.icon = icon;
        }
        else
        {
            Debug.LogWarning("Could not find icon for Fire Sword spell. Please assign manually.");
        }
        
        AssetDatabase.CreateAsset(spellData, assetPath);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"✅ Fire Sword spell data created at: {assetPath}");
        
        AddSpellToManager(spellData);
        
        Selection.activeObject = spellData;
        EditorGUIUtility.PingObject(spellData);
    }
    
    private static void AddSpellToManager(SpellData spellData)
    {
        SpellManager manager = GameObject.FindObjectOfType<SpellManager>();
        
        if (manager == null)
        {
            Debug.LogWarning("⚠️ SpellManager not found in scene. Please add the spell to SpellManager manually.");
            return;
        }
        
        SerializedObject so = new SerializedObject(manager);
        SerializedProperty allSpellsProp = so.FindProperty("allSpells");
        
        if (allSpellsProp == null)
        {
            Debug.LogError("❌ Could not find 'allSpells' property in SpellManager!");
            return;
        }
        
        for (int i = 0; i < allSpellsProp.arraySize; i++)
        {
            SerializedProperty element = allSpellsProp.GetArrayElementAtIndex(i);
            if (element.objectReferenceValue == spellData)
            {
                Debug.Log("✅ Fire Sword spell already in SpellManager!");
                return;
            }
        }
        
        allSpellsProp.arraySize++;
        allSpellsProp.GetArrayElementAtIndex(allSpellsProp.arraySize - 1).objectReferenceValue = spellData;
        
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(manager);
        
        Debug.Log("✅ Fire Sword spell added to SpellManager!");
    }
}
