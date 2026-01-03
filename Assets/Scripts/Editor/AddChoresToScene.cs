using UnityEngine;
using UnityEditor;

public class AddChoresToScene : EditorWindow
{
    [MenuItem("Tools/Add Chores to Current Scene")]
    public static void AddChores()
    {
        SpellData telekinesis = AssetDatabase.LoadAssetAtPath<SpellData>("Assets/ScriptableObjects/Spells/Telekinesis.asset");
        SpellData beamSpell = AssetDatabase.LoadAssetAtPath<SpellData>("Assets/ScriptableObjects/Spells/BeamSpell.asset");
        SpellData loraVitis = AssetDatabase.LoadAssetAtPath<SpellData>("Assets/ScriptableObjects/Spells/Lora Vitis.asset");

        if (telekinesis == null || beamSpell == null || loraVitis == null)
        {
            EditorUtility.DisplayDialog("Error", 
                "Could not find spell assets!\n\nMake sure these exist:\n" +
                "- Assets/ScriptableObjects/Spells/Telekinesis.asset\n" +
                "- Assets/ScriptableObjects/Spells/BeamSpell.asset\n" +
                "- Assets/ScriptableObjects/Spells/Lora Vitis.asset", 
                "OK");
            return;
        }

        GameObject choresParent = new GameObject("--- CHORES ---");
        Undo.RegisterCreatedObjectUndo(choresParent, "Add Chores");

        CreateChore("Chore_Telekinesis", new Vector3(-8, 0, 0), telekinesis, "Wash the Dishes", choresParent.transform);
        CreateChore("Chore_BeamSpell", new Vector3(0, 0, 0), beamSpell, "Water the Plants", choresParent.transform);
        CreateChore("Chore_LoraVitis", new Vector3(8, 0, 0), loraVitis, "Clean the Yard", choresParent.transform);

        Selection.activeGameObject = choresParent;

        EditorUtility.DisplayDialog("Success!", 
            "Added 3 chores to the scene:\n\n" +
            "• Chore_Telekinesis (-8, 0) → Telekinesis\n" +
            "• Chore_BeamSpell (0, 0) → Beam Spell\n" +
            "• Chore_LoraVitis (8, 0) → Lora Vitis\n\n" +
            "Next:\n" +
            "1. Position them where you want\n" +
            "2. Add sprites to make them look nice\n" +
            "3. Test by pressing E near them!", 
            "OK");
    }

    private static void CreateChore(string name, Vector3 position, SpellData reward, string choreName, Transform parent)
    {
        GameObject chore = new GameObject(name);
        chore.transform.SetParent(parent);
        chore.transform.position = position;
        Undo.RegisterCreatedObjectUndo(chore, "Create Chore");

        SpriteRenderer sr = chore.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        sr.color = new Color(1f, 0.9f, 0.2f);
        sr.sortingOrder = 1;

        BoxCollider2D collider = chore.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(1.5f, 1.5f);

        ChoreInteractable choreScript = chore.AddComponent<ChoreInteractable>();
        
        SerializedObject so = new SerializedObject(choreScript);
        so.FindProperty("choreName").stringValue = choreName;
        so.FindProperty("spellReward").objectReferenceValue = reward;
        so.FindProperty("interactionRange").floatValue = 3f;
        so.FindProperty("spriteRenderer").objectReferenceValue = sr;
        so.ApplyModifiedProperties();

        Debug.Log($"✓ Created {name} with spell reward: {reward.spellName}");
    }
}
