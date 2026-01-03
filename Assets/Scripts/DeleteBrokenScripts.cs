using UnityEngine;
using UnityEditor;
using System.IO;

public class DeleteBrokenScripts
{
    [MenuItem("Tools/Delete Broken Scripts")]
    public static void Delete()
    {
        string[] brokenFiles = new string[]
        {
            "Assets/Scripts/Editor/CompleteInventoryIconFix.cs",
            "Assets/Scripts/Editor/FixInventoryPanelLayers.cs",
            "Assets/Scripts/Editor/FixInventoryUILayers.cs"
        };
        
        foreach (string file in brokenFiles)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
                File.Delete(file + ".meta");
                Debug.Log("Deleted: " + file);
            }
        }
        
        AssetDatabase.Refresh();
        Debug.Log("Done! Broken scripts deleted.");
    }
}
