using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class CreateSimpleScene : EditorWindow
{
    [MenuItem("Tools/Create Simple Scene with Player")]
    public static void CreateScene()
    {
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/Electus.prefab");
        
        if (playerPrefab == null)
        {
            playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Electus.prefab");
        }
        
        if (playerPrefab != null)
        {
            GameObject player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
            player.transform.position = Vector3.zero;
            Debug.Log("✓ Created new scene with Electus prefab");
            EditorUtility.DisplayDialog("Success", "Created new untitled scene with Electus prefab!\n\nSave it with Ctrl+S or File → Save As", "OK");
        }
        else
        {
            Debug.LogError("Could not find Electus prefab at Assets/Scenes/Electus.prefab or Assets/Prefabs/Electus.prefab");
            EditorUtility.DisplayDialog("Error", "Could not find Electus prefab!\n\nLooked in:\n- Assets/Scenes/Electus.prefab\n- Assets/Prefabs/Electus.prefab", "OK");
        }
    }
}
