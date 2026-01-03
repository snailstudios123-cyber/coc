using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

public class SkeletonSetup : EditorWindow
{
    [MenuItem("Tools/🦴 Create Skeleton Enemy")]
    public static void CreateSkeletonEnemy()
    {
        if (!EditorUtility.DisplayDialog(
            "Create Skeleton Enemy",
            "This will create:\n\n" +
            "✓ Skeleton animations (Idle, Walk, Attack1, Attack2, Hurt, Die)\n" +
            "✓ Animator Controller\n" +
            "✓ Skeleton Prefab with combat setup\n\n" +
            "Continue?",
            "Yes, Create!",
            "Cancel"))
        {
            return;
        }

        try
        {
            SliceSprites();
            CreateAnimations();
            CreateAnimatorController();
            CreatePrefab();
            
            EditorUtility.DisplayDialog(
                "✅ Success!",
                "Skeleton enemy created!\n\n" +
                "✓ Animations created in /Assets/Animation/Skeleton\n" +
                "✓ Prefab created: /Assets/Prefabs/Skeleton.prefab\n\n" +
                "Drag the Skeleton prefab into your scene to test!",
                "Awesome!"
            );
            
            Debug.Log("✅ Skeleton enemy created successfully!");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", "Failed to create skeleton: " + e.Message, "OK");
            Debug.LogError("Skeleton creation error: " + e);
        }
    }

    private static void SliceSprites()
    {
        string[] spriteFiles = new string[]
        {
            "Assets/Art/Enemy/Skeleton_01_White_Idle.png",
            "Assets/Art/Enemy/Skeleton_01_White_Walk.png",
            "Assets/Art/Enemy/Skeleton_01_White_Attack1.png",
            "Assets/Art/Enemy/Skeleton_01_White_Attack2.png",
            "Assets/Art/Enemy/Skeleton_01_White_Hurt.png",
            "Assets/Art/Enemy/Skeleton_01_White_Die.png"
        };

        foreach (string path in spriteFiles)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.spritePixelsPerUnit = 100;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;

                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                int width = texture.width;
                int height = texture.height;
                
                int frameWidth = 64;
                int frameHeight = 64;
                int framesX = width / frameWidth;
                
                SpriteMetaData[] spritesheet = new SpriteMetaData[framesX];
                
                for (int i = 0; i < framesX; i++)
                {
                    SpriteMetaData smd = new SpriteMetaData();
                    smd.pivot = new Vector2(0.5f, 0f);
                    smd.alignment = 9;
                    smd.name = Path.GetFileNameWithoutExtension(path) + "_" + i;
                    smd.rect = new Rect(i * frameWidth, 0, frameWidth, frameHeight);
                    spritesheet[i] = smd;
                }
                
                importer.spritesheet = spritesheet;
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }
        }
        
        AssetDatabase.Refresh();
        Debug.Log("✓ Sprites sliced (64x64 per frame)");
    }

    private static void CreateAnimations()
    {
        string animFolder = "Assets/Animation/Skeleton";
        if (!AssetDatabase.IsValidFolder(animFolder))
        {
            AssetDatabase.CreateFolder("Assets/Animation", "Skeleton");
        }

        CreateAnimation("Idle", "Assets/Art/Enemy/Skeleton_01_White_Idle.png", animFolder, 6);
        CreateAnimation("Walk", "Assets/Art/Enemy/Skeleton_01_White_Walk.png", animFolder, 12);
        CreateAnimation("Attack1", "Assets/Art/Enemy/Skeleton_01_White_Attack1.png", animFolder, 8);
        CreateAnimation("Attack2", "Assets/Art/Enemy/Skeleton_01_White_Attack2.png", animFolder, 8);
        CreateAnimation("Hurt", "Assets/Art/Enemy/Skeleton_01_White_Hurt.png", animFolder, 4);
        CreateAnimation("Die", "Assets/Art/Enemy/Skeleton_01_White_Die.png", animFolder, 15);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✓ Animations created");
    }

    private static void CreateAnimation(string name, string spritePath, string outputFolder, int frameRate)
    {
        AnimationClip clip = new AnimationClip();
        clip.frameRate = frameRate;
        
        if (name == "Die")
        {
            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = false;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
        }

        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(spritePath);
        
        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";

        ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[sprites.Length - 1];
        int frameIndex = 0;

        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] is Sprite)
            {
                spriteKeyFrames[frameIndex] = new ObjectReferenceKeyframe();
                spriteKeyFrames[frameIndex].time = frameIndex / (float)frameRate;
                spriteKeyFrames[frameIndex].value = sprites[i];
                frameIndex++;
            }
        }

        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);
        
        AssetDatabase.CreateAsset(clip, $"{outputFolder}/Skeleton_{name}.anim");
    }

    private static void CreateAnimatorController()
    {
        string controllerPath = "Assets/Animation/Skeleton/SkeletonController.controller";
        
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        
        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;

        AnimationClip idleClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animation/Skeleton/Skeleton_Idle.anim");
        AnimationClip walkClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animation/Skeleton/Skeleton_Walk.anim");
        AnimationClip attack1Clip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animation/Skeleton/Skeleton_Attack1.anim");
        AnimationClip attack2Clip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animation/Skeleton/Skeleton_Attack2.anim");
        AnimationClip hurtClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animation/Skeleton/Skeleton_Hurt.anim");
        AnimationClip dieClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animation/Skeleton/Skeleton_Die.anim");

        AnimatorState idleState = rootStateMachine.AddState("Idle");
        idleState.motion = idleClip;
        
        AnimatorState walkState = rootStateMachine.AddState("Walk");
        walkState.motion = walkClip;
        
        AnimatorState attack1State = rootStateMachine.AddState("Attack1");
        attack1State.motion = attack1Clip;
        
        AnimatorState attack2State = rootStateMachine.AddState("Attack2");
        attack2State.motion = attack2Clip;
        
        AnimatorState hurtState = rootStateMachine.AddState("Hurt");
        hurtState.motion = hurtClip;
        
        AnimatorState dieState = rootStateMachine.AddState("Die");
        dieState.motion = dieClip;

        rootStateMachine.defaultState = idleState;

        controller.AddParameter("isWalking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("attack1", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("attack2", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("hurt", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("die", AnimatorControllerParameterType.Trigger);

        AnimatorStateTransition idleToWalk = idleState.AddTransition(walkState);
        idleToWalk.AddCondition(AnimatorConditionMode.If, 0, "isWalking");
        idleToWalk.hasExitTime = false;
        idleToWalk.duration = 0.1f;

        AnimatorStateTransition walkToIdle = walkState.AddTransition(idleState);
        walkToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "isWalking");
        walkToIdle.hasExitTime = false;
        walkToIdle.duration = 0.1f;

        AnimatorStateTransition idleToAttack1 = idleState.AddTransition(attack1State);
        idleToAttack1.AddCondition(AnimatorConditionMode.If, 0, "attack1");
        idleToAttack1.hasExitTime = false;
        idleToAttack1.duration = 0.1f;

        AnimatorStateTransition idleToAttack2 = idleState.AddTransition(attack2State);
        idleToAttack2.AddCondition(AnimatorConditionMode.If, 0, "attack2");
        idleToAttack2.hasExitTime = false;
        idleToAttack2.duration = 0.1f;

        AnimatorStateTransition attack1ToIdle = attack1State.AddTransition(idleState);
        attack1ToIdle.hasExitTime = true;
        attack1ToIdle.exitTime = 0.9f;
        attack1ToIdle.duration = 0.1f;

        AnimatorStateTransition attack2ToIdle = attack2State.AddTransition(idleState);
        attack2ToIdle.hasExitTime = true;
        attack2ToIdle.exitTime = 0.9f;
        attack2ToIdle.duration = 0.1f;

        AnimatorStateTransition anyToHurt = rootStateMachine.AddAnyStateTransition(hurtState);
        anyToHurt.AddCondition(AnimatorConditionMode.If, 0, "hurt");
        anyToHurt.hasExitTime = false;
        anyToHurt.duration = 0.1f;

        AnimatorStateTransition hurtToIdle = hurtState.AddTransition(idleState);
        hurtToIdle.hasExitTime = true;
        hurtToIdle.exitTime = 0.9f;
        hurtToIdle.duration = 0.1f;

        AnimatorStateTransition anyToDie = rootStateMachine.AddAnyStateTransition(dieState);
        anyToDie.AddCondition(AnimatorConditionMode.If, 0, "die");
        anyToDie.hasExitTime = false;
        anyToDie.duration = 0.1f;

        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        Debug.Log("✓ Animator controller created");
    }

    private static void CreatePrefab()
    {
        GameObject skeleton = new GameObject("Skeleton");
        
        skeleton.tag = "Enemy";
        skeleton.layer = LayerMask.NameToLayer("Attackable");
        
        SpriteRenderer sr = skeleton.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Default";
        sr.sortingOrder = 0;
        
        Object[] allSprites = AssetDatabase.LoadAllAssetsAtPath("Assets/Art/Enemy/Skeleton_01_White_Idle.png");
        foreach (Object obj in allSprites)
        {
            if (obj is Sprite)
            {
                sr.sprite = obj as Sprite;
                break;
            }
        }

        Animator animator = skeleton.AddComponent<Animator>();
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Animation/Skeleton/SkeletonController.controller");
        animator.runtimeAnimatorController = controller;

        Rigidbody2D rb = skeleton.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        CapsuleCollider2D collider = skeleton.AddComponent<CapsuleCollider2D>();
        collider.size = new Vector2(0.5f, 0.9f);
        collider.offset = new Vector2(0, 0.45f);

        GameObject attackPoint = new GameObject("AttackPoint");
        attackPoint.transform.SetParent(skeleton.transform);
        attackPoint.transform.localPosition = new Vector3(0.7f, 0.5f, 0);

        Skeleton skeletonScript = skeleton.AddComponent<Skeleton>();
        
        SerializedObject so = new SerializedObject(skeletonScript);
        so.FindProperty("health").floatValue = 50f;
        so.FindProperty("recoilLength").floatValue = 0.2f;
        so.FindProperty("recoilFactor").floatValue = 2f;
        so.FindProperty("speed").floatValue = 3f;
        so.FindProperty("damage").floatValue = 1f;
        so.FindProperty("attackPoint").objectReferenceValue = attackPoint.transform;
        so.ApplyModifiedProperties();

        string prefabPath = "Assets/Prefabs/Skeleton.prefab";
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        PrefabUtility.SaveAsPrefabAsset(skeleton, prefabPath);
        
        DestroyImmediate(skeleton);
        
        Debug.Log("✓ Skeleton prefab created at: " + prefabPath);
    }
}
