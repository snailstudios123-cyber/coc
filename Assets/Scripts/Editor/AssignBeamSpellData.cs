using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class AssignBeamSpellData : EditorWindow
{
    [MenuItem("Tools/Assign Beam Spell Data")]
    public static void AssignSpellData()
    {
        SpellData beamSpellData = AssetDatabase.LoadAssetAtPath<SpellData>("Assets/ScriptableObjects/Spells/New Spell.asset");
        
        if (beamSpellData == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not find 'New Spell.asset' at Assets/ScriptableObjects/Spells/New Spell.asset", "OK");
            return;
        }

        string[] scenePaths = { "Assets/Scenes/BedroomScene.unity", "Assets/Scenes/DownstairsScene.unity" };
        int assignedCount = 0;

        foreach (string scenePath in scenePaths)
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            
            GameObject[] rootObjects = scene.GetRootGameObjects();
            foreach (GameObject root in rootObjects)
            {
                BeamSpell[] beamSpells = root.GetComponentsInChildren<BeamSpell>(true);
                
                foreach (BeamSpell beamSpell in beamSpells)
                {
                    SerializedObject so = new SerializedObject(beamSpell);
                    SerializedProperty spellDataProp = so.FindProperty("spellData");
                    
                    if (spellDataProp != null)
                    {
                        spellDataProp.objectReferenceValue = beamSpellData;
                        so.ApplyModifiedProperties();
                        assignedCount++;
                        Debug.Log($"Assigned BeamSpell data to {beamSpell.gameObject.name} in {scene.name}");
                    }
                }
            }
            
            EditorSceneManager.SaveScene(scene);
        }

        EditorUtility.DisplayDialog("Success", $"Assigned BeamSpell data to {assignedCount} BeamSpell component(s)!\n\nThe spell is currently NOT equipped. To equip it, either:\n1. Open 'New Spell.asset' and check 'Is Equipped'\n2. Or add it through SpellManager", "OK");
    }
}
