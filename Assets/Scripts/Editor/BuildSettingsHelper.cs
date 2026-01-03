using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class BuildSettingsHelper : EditorWindow
{
    [MenuItem("Tools/Add All Scenes to Build Settings")]
    public static void AddAllScenesToBuildSettings()
    {
        string[] allSceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
        
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
        
        foreach (string guid in allSceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            
            if (!scenePath.EndsWith(".unity"))
                continue;
                
            EditorBuildSettingsScene buildScene = new EditorBuildSettingsScene(scenePath, true);
            scenes.Add(buildScene);
        }
        
        EditorBuildSettings.scenes = scenes.ToArray();
        
        Debug.Log($"Added {scenes.Count} scenes to Build Settings:");
        foreach (var scene in scenes)
        {
            Debug.Log($"  - {scene.path}");
        }
        
        EditorUtility.DisplayDialog(
            "Build Settings Updated",
            $"Successfully added {scenes.Count} scenes to Build Settings!\n\nCheck the Console for the full list.",
            "OK"
        );
    }

    [MenuItem("Tools/Show Scenes in Build Settings")]
    public static void ShowScenesInBuildSettings()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        
        if (scenes.Length == 0)
        {
            Debug.Log("No scenes in Build Settings!");
            EditorUtility.DisplayDialog(
                "Build Settings",
                "No scenes are currently in Build Settings.\n\nUse 'Tools -> Add All Scenes to Build Settings' to add them.",
                "OK"
            );
            return;
        }
        
        Debug.Log($"Scenes in Build Settings ({scenes.Length} total):");
        for (int i = 0; i < scenes.Length; i++)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenes[i].path);
            Debug.Log($"  [{i}] {sceneName} - {(scenes[i].enabled ? "Enabled" : "Disabled")}");
        }
        
        string message = $"Found {scenes.Length} scenes in Build Settings.\n\nCheck the Console for details.";
        EditorUtility.DisplayDialog("Build Settings", message, "OK");
    }
}
