using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSetupValidator : MonoBehaviour
{
    [ContextMenu("Validate Scene Setup")]
    public void ValidateSetup()
    {
        Debug.Log("=== SCENE SETUP VALIDATION ===");
        
        ValidateCamera();
        ValidateBuildSettings();
        ValidateSceneTransition();
        
        Debug.Log("=== VALIDATION COMPLETE ===");
    }

    private void ValidateCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("❌ No Main Camera found!");
            return;
        }

        Debug.Log($"Camera Clear Flags: {cam.clearFlags}");
        Debug.Log($"Camera Position: {cam.transform.position}");
        Debug.Log($"Camera Orthographic: {cam.orthographic}");

        if (cam.clearFlags == CameraClearFlags.Skybox)
        {
            Debug.LogWarning("⚠️ Camera Clear Flags is set to Skybox. Change to 'Solid Color' for 2D games.");
        }
        else
        {
            Debug.Log("✓ Camera Clear Flags is correct");
        }

        if (Mathf.Approximately(cam.transform.position.z, 0f))
        {
            Debug.LogWarning("⚠️ Camera Z position is 0. Set to -10 for 2D games.");
        }
        else
        {
            Debug.Log("✓ Camera Z position is correct");
        }
    }

    private void ValidateBuildSettings()
    {
        Debug.Log($"\nScenes in Build Settings: {SceneManager.sceneCountInBuildSettings}");
        
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            Debug.Log($"  [{i}] {sceneName}");
        }

        string[] requiredScenes = { "BedroomScene", "DownstairsScene" };
        foreach (string scene in requiredScenes)
        {
            if (SceneExistsInBuildSettings(scene))
            {
                Debug.Log($"✓ {scene} is in Build Settings");
            }
            else
            {
                Debug.LogError($"❌ {scene} is NOT in Build Settings! Add via File → Build Settings");
            }
        }
    }

    private void ValidateSceneTransition()
    {
        SceneTransitionManager manager = FindObjectOfType<SceneTransitionManager>();
        if (manager == null)
        {
            Debug.LogWarning("⚠️ No SceneTransitionManager found in scene");
        }
        else
        {
            Debug.Log("✓ SceneTransitionManager found");
        }
    }

    private bool SceneExistsInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneNameFromPath == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}
