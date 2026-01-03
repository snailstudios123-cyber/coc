using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddOutdoorsToRuinsTransition : EditorWindow
{
    [MenuItem("Tools/Story Progression/Add Outdoors → Ruins Transition")]
    public static void ShowWindow()
    {
        AddTransition();
    }

    private static void AddTransition()
    {
        string scenePath = "Assets/Scenes/Outdoors.unity";
        
        if (!System.IO.File.Exists(scenePath))
        {
            EditorUtility.DisplayDialog(
                "Scene Not Found",
                "Outdoors.unity scene not found!\n\nCreate it first or check the scene name.",
                "OK"
            );
            return;
        }

        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        GameObject existingTrigger = GameObject.Find("Trigger - To Outer Ruins");
        if (existingTrigger != null)
        {
            Debug.Log("[OutdoorsSetup] Transition trigger already exists");
            EditorUtility.DisplayDialog(
                "Already Exists",
                "Transition trigger 'Trigger - To Outer Ruins' already exists in the scene!",
                "OK"
            );
            return;
        }

        GameObject trigger = new GameObject("Trigger - To Outer Ruins");
        trigger.transform.position = new Vector3(15, 0, 0);
        trigger.tag = "Untagged";
        
        BoxCollider2D collider = trigger.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(2, 5);
        collider.isTrigger = true;
        
        SceneTransitionTrigger transitionTrigger = trigger.AddComponent<SceneTransitionTrigger>();
        SerializedObject so = new SerializedObject(transitionTrigger);
        so.FindProperty("targetSceneName").stringValue = "OuterRuins";
        so.FindProperty("spawnPosition").vector2Value = new Vector2(-8, 0);
        so.ApplyModifiedProperties();

        GameObject visualHelper = new GameObject("Visual Helper");
        visualHelper.transform.SetParent(trigger.transform);
        visualHelper.transform.localPosition = Vector3.zero;
        
        SpriteRenderer renderer = visualHelper.AddComponent<SpriteRenderer>();
        renderer.color = new Color(1f, 1f, 0f, 0.3f);
        renderer.sortingOrder = 100;

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Selection.activeGameObject = trigger;
        SceneView.lastActiveSceneView.FrameSelected();

        Debug.Log("[OutdoorsSetup] ✓ Added transition trigger to OuterRuins");
        
        EditorUtility.DisplayDialog(
            "Transition Added!",
            "Successfully added transition trigger to Outdoors scene!\n\n" +
            "Position: (15, 0, 0) - right side of scene\n" +
            "Target: OuterRuins scene\n" +
            "Spawn Position: (-8, 0) - left side of OuterRuins\n\n" +
            "The trigger is now selected in the hierarchy. Adjust position as needed!",
            "OK"
        );
    }
}
