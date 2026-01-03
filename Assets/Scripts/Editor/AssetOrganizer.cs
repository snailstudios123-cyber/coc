using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class AssetOrganizer : EditorWindow
{
    private static List<string> moveLog = new List<string>();
    
    [MenuItem("Tools/Organize All Assets")]
    public static void OrganizeAssets()
    {
        bool confirmed = EditorUtility.DisplayDialog(
            "Organize Assets",
            "This will organize all assets according to project structure:\n\n" +
            "• Scripts → /Assets/Scripts\n" +
            "• Prefabs → /Assets/Prefabs\n" +
            "• Images → /Assets/Art/Textures\n" +
            "• Audio → /Assets/Audio\n" +
            "• Materials → /Assets/Materials\n" +
            "• Documentation → /Assets/Documentation\n\n" +
            "Do you want to continue?",
            "Yes, Organize",
            "Cancel"
        );

        if (!confirmed) return;

        moveLog.Clear();
        
        EnsureDirectoriesExist();
        
        OrganizeRootAssets();
        OrganizeScriptsFolder();
        OrganizeScenesFolder();
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        string logMessage = $"Asset Organization Complete!\n\nMoved {moveLog.Count} files:\n\n" + 
                          string.Join("\n", moveLog.Take(20));
        
        if (moveLog.Count > 20)
        {
            logMessage += $"\n... and {moveLog.Count - 20} more files.";
        }
        
        Debug.Log("=== ASSET ORGANIZATION LOG ===");
        foreach (string log in moveLog)
        {
            Debug.Log(log);
        }
        
        EditorUtility.DisplayDialog("Organization Complete", 
            $"Successfully organized {moveLog.Count} assets!\n\nCheck the Console for details.", 
            "OK");
    }

    private static void EnsureDirectoriesExist()
    {
        string[] directories = new string[]
        {
            "Assets/Scripts",
            "Assets/Materials",
            "Assets/Prefabs",
            "Assets/Models",
            "Assets/Scenes",
            "Assets/Audio",
            "Assets/Art/Textures",
            "Assets/Documentation"
        };

        foreach (string dir in directories)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                Debug.Log($"Created directory: {dir}");
            }
        }
    }

    private static void OrganizeRootAssets()
    {
        string[] allAssets = Directory.GetFiles("Assets", "*.*", SearchOption.TopDirectoryOnly);

        foreach (string assetPath in allAssets)
        {
            if (assetPath.EndsWith(".meta")) continue;
            
            string fileName = Path.GetFileName(assetPath);
            string extension = Path.GetExtension(assetPath).ToLower();
            
            string targetFolder = GetTargetFolder(fileName, extension);
            
            if (!string.IsNullOrEmpty(targetFolder))
            {
                MoveAsset(assetPath, targetFolder, fileName);
            }
        }
    }

    private static void OrganizeScriptsFolder()
    {
        string scriptsPath = "Assets/Scripts";
        if (!Directory.Exists(scriptsPath)) return;

        string[] allFiles = Directory.GetFiles(scriptsPath, "*.*", SearchOption.TopDirectoryOnly);

        foreach (string filePath in allFiles)
        {
            if (filePath.EndsWith(".meta")) continue;
            
            string fileName = Path.GetFileName(filePath);
            string extension = Path.GetExtension(filePath).ToLower();
            
            if (extension == ".prefab")
            {
                MoveAsset(filePath, "Assets/Prefabs", fileName);
            }
            else if (extension == ".anim" || extension == ".controller")
            {
                MoveAsset(filePath, "Assets/Animation", fileName);
            }
            else if (extension == ".wav" || extension == ".mp3" || extension == ".ogg")
            {
                MoveAsset(filePath, "Assets/Audio", fileName);
            }
            else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
            {
                MoveAsset(filePath, "Assets/Art/Textures", fileName);
            }
            else if (extension == ".txt")
            {
                MoveAsset(filePath, "Assets/Documentation", fileName);
            }
        }
    }

    private static void OrganizeScenesFolder()
    {
        string scenesPath = "Assets/Scenes";
        if (!Directory.Exists(scenesPath)) return;

        string[] allFiles = Directory.GetFiles(scenesPath, "*.*", SearchOption.TopDirectoryOnly);

        foreach (string filePath in allFiles)
        {
            if (filePath.EndsWith(".meta")) continue;
            if (filePath.EndsWith(".unity")) continue;
            
            string fileName = Path.GetFileName(filePath);
            string extension = Path.GetExtension(filePath).ToLower();
            
            if (extension == ".prefab")
            {
                MoveAsset(filePath, "Assets/Prefabs", fileName);
            }
            else if (extension == ".anim" || extension == ".controller")
            {
                MoveAsset(filePath, "Assets/Animation", fileName);
            }
            else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
            {
                MoveAsset(filePath, "Assets/Art/Textures", fileName);
            }
            else if (extension == ".asset")
            {
                MoveAsset(filePath, "Assets/ScriptableObjects", fileName);
            }
        }
    }

    private static string GetTargetFolder(string fileName, string extension)
    {
        if (extension == ".cs")
        {
            return "Assets/Scripts";
        }
        else if (extension == ".prefab")
        {
            return "Assets/Prefabs";
        }
        else if (extension == ".mat")
        {
            return "Assets/Materials";
        }
        else if (extension == ".physicsmaterial2d" || extension == ".physicmaterial")
        {
            return "Assets/Materials";
        }
        else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".psd")
        {
            return "Assets/Art/Textures";
        }
        else if (extension == ".wav" || extension == ".mp3" || extension == ".ogg" || extension == ".aiff")
        {
            return "Assets/Audio";
        }
        else if (extension == ".txt")
        {
            return "Assets/Documentation";
        }
        
        return null;
    }

    private static void MoveAsset(string fromPath, string toFolder, string fileName)
    {
        string normalizedFromPath = fromPath.Replace("\\", "/");
        string normalizedToFolder = toFolder.Replace("\\", "/");
        
        if (!Directory.Exists(normalizedToFolder))
        {
            Directory.CreateDirectory(normalizedToFolder);
        }

        string toPath = Path.Combine(normalizedToFolder, fileName).Replace("\\", "/");
        
        if (normalizedFromPath == toPath)
        {
            return;
        }

        if (File.Exists(toPath))
        {
            string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            string ext = Path.GetExtension(fileName);
            int counter = 1;
            
            while (File.Exists(toPath))
            {
                string newFileName = $"{nameWithoutExt}_{counter}{ext}";
                toPath = Path.Combine(normalizedToFolder, newFileName).Replace("\\", "/");
                counter++;
            }
        }

        string error = AssetDatabase.MoveAsset(normalizedFromPath, toPath);
        
        if (string.IsNullOrEmpty(error))
        {
            moveLog.Add($"✓ Moved: {fileName} → {normalizedToFolder}");
        }
        else
        {
            Debug.LogError($"Failed to move {normalizedFromPath} to {toPath}: {error}");
        }
    }

    [MenuItem("Tools/Show Asset Organization Preview")]
    public static void ShowOrganizationPreview()
    {
        List<string> preview = new List<string>();
        Dictionary<string, int> categoryCounts = new Dictionary<string, int>();
        
        string[] rootAssets = Directory.GetFiles("Assets", "*.*", SearchOption.TopDirectoryOnly);
        
        foreach (string assetPath in rootAssets)
        {
            if (assetPath.EndsWith(".meta")) continue;
            
            string fileName = Path.GetFileName(assetPath);
            string extension = Path.GetExtension(assetPath).ToLower();
            string targetFolder = GetTargetFolder(fileName, extension);
            
            if (!string.IsNullOrEmpty(targetFolder))
            {
                preview.Add($"{fileName} → {targetFolder}");
                
                if (!categoryCounts.ContainsKey(targetFolder))
                {
                    categoryCounts[targetFolder] = 0;
                }
                categoryCounts[targetFolder]++;
            }
        }
        
        if (preview.Count == 0)
        {
            EditorUtility.DisplayDialog("Preview", "Root Assets folder is already organized! ✓", "OK");
            return;
        }
        
        Debug.Log("=== ORGANIZATION PREVIEW ===");
        Debug.Log($"Found {preview.Count} assets in root folder to organize:\n");
        
        Debug.Log("By Category:");
        foreach (var kvp in categoryCounts.OrderByDescending(x => x.Value))
        {
            Debug.Log($"  {kvp.Key}: {kvp.Value} files");
        }
        
        Debug.Log("\nDetailed List:");
        foreach (string item in preview)
        {
            Debug.Log($"  {item}");
        }
        
        string message = $"Found {preview.Count} assets in root folder:\n\n";
        foreach (var kvp in categoryCounts.OrderByDescending(x => x.Value))
        {
            string folderName = kvp.Key.Replace("Assets/", "");
            message += $"• {kvp.Value} → {folderName}\n";
        }
        message += "\nCheck Console for full details.";
        
        EditorUtility.DisplayDialog("Organization Preview", message, "OK");
    }
}
