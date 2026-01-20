using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CreatePetrifyAnimation : MonoBehaviour
{
    [MenuItem("Tools/Create Player Petrify Animation")]
    static void CreateAnimation()
    {
        AnimationClip clip = new AnimationClip();
        clip.frameRate = 12f;
        clip.wrapMode = WrapMode.Once;
        
        string spriteSheetPath = "Assets/Scenes/Medusa_TurnToStone_Sheet.png";
        
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheetPath);
        List<Sprite> spriteFrames = new List<Sprite>();
        
        foreach (Object obj in sprites)
        {
            if (obj is Sprite)
            {
                spriteFrames.Add(obj as Sprite);
            }
        }
        
        if (spriteFrames.Count == 0)
        {
            Debug.LogError("No sprites found in " + spriteSheetPath);
            return;
        }
        
        spriteFrames.Sort((a, b) => a.name.CompareTo(b.name));
        
        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";
        
        ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[spriteFrames.Count];
        for (int i = 0; i < spriteFrames.Count; i++)
        {
            spriteKeyFrames[i] = new ObjectReferenceKeyframe();
            spriteKeyFrames[i].time = i / clip.frameRate;
            spriteKeyFrames[i].value = spriteFrames[i];
        }
        
        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);
        
        Keyframe[] colorFrames = new Keyframe[spriteFrames.Count];
        for (int i = 0; i < spriteFrames.Count; i++)
        {
            float grayValue = 1f - (i / (float)(spriteFrames.Count - 1)) * 0.5f;
            colorFrames[i] = new Keyframe(i / clip.frameRate, grayValue);
        }
        
        AnimationCurve colorCurve = new AnimationCurve(colorFrames);
        
        clip.SetCurve("", typeof(SpriteRenderer), "m_Color.r", colorCurve);
        clip.SetCurve("", typeof(SpriteRenderer), "m_Color.g", colorCurve);
        clip.SetCurve("", typeof(SpriteRenderer), "m_Color.b", colorCurve);
        
        string savePath = "Assets/Animation/Art/Player/Player_TurnToStone.anim";
        AssetDatabase.CreateAsset(clip, savePath);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"Created petrification animation at {savePath} with {spriteFrames.Count} frames");
    }
}
