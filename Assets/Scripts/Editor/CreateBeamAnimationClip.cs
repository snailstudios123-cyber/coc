using UnityEngine;
using UnityEditor;
using System.Linq;

public class CreateBeamAnimationClip : EditorWindow
{
    private Texture2D laserSpriteSheet;
    private float frameRate = 12f;
    
    [MenuItem("Tools/Create Beam Animation Clip")]
    public static void ShowWindow()
    {
        CreateBeamAnimationClip window = GetWindow<CreateBeamAnimationClip>("Create Beam Animation");
        window.minSize = new Vector2(400, 200);
        window.Show();
    }
    
    private void OnEnable()
    {
        laserSpriteSheet = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Animation/Spells Animation/Laser_sheet.png");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Create Beam Animation Clip", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox("This creates a Unity Animation Clip from your laser sprite sheet.", MessageType.Info);
        EditorGUILayout.Space();
        
        laserSpriteSheet = EditorGUILayout.ObjectField("Laser Sprite Sheet", laserSpriteSheet, typeof(Texture2D), false) as Texture2D;
        frameRate = EditorGUILayout.FloatField("Frame Rate", frameRate);
        
        EditorGUILayout.Space();
        
        GUI.enabled = laserSpriteSheet != null;
        
        if (GUILayout.Button("Create Animation Clip", GUILayout.Height(40)))
        {
            CreateAnimationClip();
        }
        
        GUI.enabled = true;
    }
    
    private void CreateAnimationClip()
    {
        if (laserSpriteSheet == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign the Laser Sprite Sheet!", "OK");
            return;
        }
        
        string path = AssetDatabase.GetAssetPath(laserSpriteSheet);
        
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
        Sprite[] sprites = assets.OfType<Sprite>().OrderBy(s => s.name).ToArray();
        
        if (sprites.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "No sprites found! Please slice the sprite sheet first:\n\n1. Select the sprite sheet\n2. Change Sprite Mode to Multiple\n3. Click Sprite Editor and slice it", "OK");
            return;
        }
        
        AnimationClip clip = new AnimationClip();
        clip.frameRate = frameRate;
        
        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";
        
        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[sprites.Length];
        
        for (int i = 0; i < sprites.Length; i++)
        {
            keyframes[i] = new ObjectReferenceKeyframe();
            keyframes[i].time = i / frameRate;
            keyframes[i].value = sprites[i];
        }
        
        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyframes);
        
        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        
        string clipPath = "Assets/Animation/Spells Animation/LaserBeam.anim";
        AssetDatabase.CreateAsset(clip, clipPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Animation created at:");
        EditorGUILayout.SelectableLabel(clipPath, GUILayout.Height(20));
        EditorGUILayout.EndHorizontal();
        
        EditorUtility.DisplayDialog("Success", $"Animation clip created!\n\nLocation: {clipPath}\nFrames: {sprites.Length}\nFrame Rate: {frameRate} FPS\n\nYou can now:\n1. Create a new Animator Controller\n2. Add this animation to it\n3. Assign the controller to your BeamEffect prefab", "OK");
        
        Selection.activeObject = clip;
        EditorGUIUtility.PingObject(clip);
    }
}
