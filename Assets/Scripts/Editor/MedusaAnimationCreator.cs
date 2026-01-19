using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MedusaAnimationCreator : EditorWindow
{
    [MenuItem("Tools/Create Medusa Animations")]
    public static void CreateAllAnimations()
    {
        CreateScreechAnimation();
        CreateHissAnimation();
        CreateTailWhipAnimation();
        CreateJumpAnimation();
        CreateSlamAnimation();
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("✅ All Medusa animations created successfully!");
        EditorUtility.DisplayDialog("Success", "All Medusa animation clips have been created!\n\nCheck: /Assets/Animation/Art/Enemy/", "OK");
    }
    
    private static void CreateScreechAnimation()
    {
        AnimationClip clip = new AnimationClip();
        clip.frameRate = 10;
        clip.wrapMode = WrapMode.Once;
        
        EditorCurveBinding spriteBinding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };
        
        List<ObjectReferenceKeyframe> keyframes = new List<ObjectReferenceKeyframe>();
        
        Sprite[] sprites = LoadSprites("Assets/Scenes/Medusa_TurnToStone_Sheet.png");
        if (sprites != null && sprites.Length >= 8)
        {
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.0f, value = sprites[5] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.1f, value = sprites[6] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.2f, value = sprites[7] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.3f, value = sprites[6] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.4f, value = sprites[5] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.5f, value = sprites[6] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.6f, value = sprites[7] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.7f, value = sprites[6] });
        }
        
        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyframes.ToArray());
        
        AssetDatabase.CreateAsset(clip, "Assets/Animation/Art/Enemy/MedusaScreech.anim");
        Debug.Log("Created MedusaScreech.anim");
    }
    
    private static void CreateHissAnimation()
    {
        AnimationClip clip = new AnimationClip();
        clip.frameRate = 10;
        clip.wrapMode = WrapMode.Once;
        
        EditorCurveBinding spriteBinding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };
        
        List<ObjectReferenceKeyframe> keyframes = new List<ObjectReferenceKeyframe>();
        
        Sprite[] sprites = LoadSprites("Assets/Scenes/Medusa_TurnToStone_Sheet.png");
        if (sprites != null && sprites.Length >= 4)
        {
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.0f, value = sprites[0] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.1f, value = sprites[1] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.2f, value = sprites[2] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.3f, value = sprites[3] });
        }
        
        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyframes.ToArray());
        
        AssetDatabase.CreateAsset(clip, "Assets/Animation/Art/Enemy/MedusaHiss.anim");
        Debug.Log("Created MedusaHiss.anim");
    }
    
    private static void CreateTailWhipAnimation()
    {
        AnimationClip clip = new AnimationClip();
        clip.frameRate = 12;
        clip.wrapMode = WrapMode.Once;
        
        EditorCurveBinding spriteBinding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };
        
        List<ObjectReferenceKeyframe> keyframes = new List<ObjectReferenceKeyframe>();
        
        Sprite[] sprites = LoadSprites("Assets/Scenes/Medusa_ChargeAttack_Sheet.png");
        if (sprites != null && sprites.Length >= 6)
        {
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.0f, value = sprites[0] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.1f, value = sprites[1] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.2f, value = sprites[2] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.3f, value = sprites[3] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.4f, value = sprites[4] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.5f, value = sprites[5] });
        }
        
        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyframes.ToArray());
        
        AssetDatabase.CreateAsset(clip, "Assets/Animation/Art/Enemy/MedusaTailWhip.anim");
        Debug.Log("Created MedusaTailWhip.anim");
    }
    
    private static void CreateJumpAnimation()
    {
        AnimationClip clip = new AnimationClip();
        clip.frameRate = 8;
        clip.wrapMode = WrapMode.Once;
        
        EditorCurveBinding spriteBinding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };
        
        List<ObjectReferenceKeyframe> keyframes = new List<ObjectReferenceKeyframe>();
        
        Sprite[] sprites = LoadSprites("Assets/Scenes/Medusa_TurnToStone_Sheet.png");
        if (sprites != null && sprites.Length >= 6)
        {
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.0f, value = sprites[0] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.15f, value = sprites[1] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.3f, value = sprites[2] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.6f, value = sprites[3] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.9f, value = sprites[4] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 1.05f, value = sprites[5] });
        }
        
        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyframes.ToArray());
        
        AssetDatabase.CreateAsset(clip, "Assets/Animation/Art/Enemy/MedusaJump.anim");
        Debug.Log("Created MedusaJump.anim");
    }
    
    private static void CreateSlamAnimation()
    {
        AnimationClip clip = new AnimationClip();
        clip.frameRate = 15;
        clip.wrapMode = WrapMode.Once;
        
        EditorCurveBinding spriteBinding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };
        
        List<ObjectReferenceKeyframe> keyframes = new List<ObjectReferenceKeyframe>();
        
        Sprite[] chargeSprites = LoadSprites("Assets/Scenes/Medusa_ChargeAttack_Sheet.png");
        if (chargeSprites != null && chargeSprites.Length >= 9)
        {
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.0f, value = chargeSprites[8] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.1f, value = chargeSprites[7] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.2f, value = chargeSprites[6] });
            keyframes.Add(new ObjectReferenceKeyframe { time = 0.3f, value = chargeSprites[5] });
        }
        
        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyframes.ToArray());
        
        AssetDatabase.CreateAsset(clip, "Assets/Animation/Art/Enemy/MedusaSlam.anim");
        Debug.Log("Created MedusaSlam.anim");
    }
    
    private static Sprite[] LoadSprites(string path)
    {
        Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);
        List<Sprite> sprites = new List<Sprite>();
        
        foreach (Object obj in objects)
        {
            if (obj is Sprite)
            {
                sprites.Add(obj as Sprite);
            }
        }
        
        sprites.Sort((a, b) => a.name.CompareTo(b.name));
        
        if (sprites.Count == 0)
        {
            Debug.LogError($"No sprites found in {path}! Make sure the texture is set to Sprite mode with Multiple slicing.");
            return null;
        }
        
        return sprites.ToArray();
    }
}
