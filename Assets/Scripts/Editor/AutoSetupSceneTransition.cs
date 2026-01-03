using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoSetupSceneTransition
{
    static AutoSetupSceneTransition()
    {
        EditorApplication.delayCall += SetupTransitionOnce;
    }

    private static void SetupTransitionOnce()
    {
        if (!EditorPrefs.GetBool("SceneTransitionSetup_Done", false))
        {
            SceneTransitionSetupTool.SetupSceneTransition();
            BuildSettingsHelper.AddAllScenesToBuildSettings();
            EditorPrefs.SetBool("SceneTransitionSetup_Done", true);
        }
    }

    [MenuItem("Tools/Reset Scene Transition Setup Flag")]
    private static void ResetSetupFlag()
    {
        EditorPrefs.DeleteKey("SceneTransitionSetup_Done");
        Debug.Log("Scene Transition setup flag has been reset. The system will be set up again on next domain reload.");
    }
    
    [MenuItem("Tools/Complete Scene Transition Setup")]
    private static void CompleteSetup()
    {
        SceneTransitionSetupTool.SetupSceneTransition();
        BuildSettingsHelper.AddAllScenesToBuildSettings();
        Debug.Log("Scene Transition system setup complete with all scenes added to Build Settings!");
    }
}
