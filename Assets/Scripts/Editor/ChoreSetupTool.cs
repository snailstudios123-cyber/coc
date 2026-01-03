using UnityEngine;
using UnityEditor;

public class ChoreSetupTool : EditorWindow
{
    [MenuItem("Tools/Setup Chores")]
    public static void SetupChores()
    {
        SpellData telekinesis = AssetDatabase.LoadAssetAtPath<SpellData>("Assets/ScriptableObjects/Spells/Telekinesis.asset");
        SpellData beamSpell = AssetDatabase.LoadAssetAtPath<SpellData>("Assets/ScriptableObjects/Spells/BeamSpell.asset");
        SpellData loraVitis = AssetDatabase.LoadAssetAtPath<SpellData>("Assets/ScriptableObjects/Spells/Lora Vitis.asset");

        if (telekinesis == null || beamSpell == null || loraVitis == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not find spell assets. Make sure these exist:\n- Telekinesis.asset\n- BeamSpell.asset\n- Lora Vitis.asset", "OK");
            return;
        }

        CreateChoreObject("Chore_Telekinesis", new Vector3(-5, 0, 0), telekinesis, "Wash the Dishes");
        CreateChoreObject("Chore_BeamSpell", new Vector3(0, 0, 0), beamSpell, "Water the Plants");
        CreateChoreObject("Chore_LoraVitis", new Vector3(5, 0, 0), loraVitis, "Clean the Yard");

        EditorUtility.DisplayDialog("Success", "Created 3 chore objects!\n\n- Chore_Telekinesis (left)\n- Chore_BeamSpell (center)\n- Chore_LoraVitis (right)\n\nPosition them outside and assign sprites in the Inspector.", "OK");
    }

    private static void CreateChoreObject(string name, Vector3 position, SpellData reward, string choreName)
    {
        GameObject chore = new GameObject(name);
        chore.transform.position = position;

        SpriteRenderer sr = chore.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.8f, 0.8f, 0.2f);
        sr.sortingOrder = 1;

        BoxCollider2D collider = chore.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(1f, 1f);

        ChoreInteractable choreScript = chore.AddComponent<ChoreInteractable>();
        SerializedObject so = new SerializedObject(choreScript);
        so.FindProperty("choreName").stringValue = choreName;
        so.FindProperty("spellReward").objectReferenceValue = reward;
        so.FindProperty("interactionRange").floatValue = 2f;
        
        GameObject prompt = CreateInteractionPrompt(chore.transform);
        so.FindProperty("interactionPrompt").objectReferenceValue = prompt;
        so.FindProperty("spriteRenderer").objectReferenceValue = sr;
        
        so.ApplyModifiedProperties();

        Debug.Log($"Created {name} at {position} with reward: {reward.spellName}");
    }

    private static GameObject CreateInteractionPrompt(Transform parent)
    {
        GameObject prompt = new GameObject("InteractionPrompt");
        prompt.transform.SetParent(parent);
        prompt.transform.localPosition = new Vector3(0, 1.5f, 0);

        SpriteRenderer sr = prompt.AddComponent<SpriteRenderer>();
        sr.color = Color.white;
        sr.sortingOrder = 10;

        prompt.SetActive(false);
        return prompt;
    }
}
