using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class MedusaBossSetupTool : EditorWindow
{
    private const string ANIMATION_PATH = "Assets/Animation/Art/Enemy/";
    private const string CONTROLLER_NAME = "MedusaBossController";
    
    private GameObject medusaPrefab;
    private AnimationClip gazeAnimationClip;
    private AnimationClip chargeAnimationClip;
    private float gazeEventTime = 0.6f;
    private float chargeEventTime = 0.5f;
    
    [MenuItem("Tools/Medusa Boss/Setup Animations")]
    public static void ShowWindow()
    {
        GetWindow<MedusaBossSetupTool>("Medusa Boss Setup");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Medusa Boss Animation Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        medusaPrefab = (GameObject)EditorGUILayout.ObjectField("Medusa Prefab", medusaPrefab, typeof(GameObject), false);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Create All Animations & Controller", GUILayout.Height(40)))
        {
            CreateAnimationSetup();
        }
        
        GUILayout.Space(20);
        
        EditorGUILayout.HelpBox(
            "This tool will:\n" +
            "• Create 4 animation clips (Idle, Moving, GazeAttack, ChargeAttack)\n" +
            "• Add Animation Event to GazeAttack at 0.6s (triggers petrification)\n" +
            "• Add Animation Event to ChargeAttack at 0.5s (triggers forward lunge)\n" +
            "• Create an Animator Controller\n" +
            "• Set up all states, parameters, and transitions\n" +
            "• Assign the controller to the prefab (if selected)",
            MessageType.Info);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Create Animations Only"))
        {
            CreateAnimationClips();
        }
        
        if (GUILayout.Button("Create Controller Only"))
        {
            CreateAnimatorController();
        }
        
        GUILayout.Space(20);
        GUILayout.Label("Add Animation Events (Manual)", EditorStyles.boldLabel);
        
        gazeEventTime = EditorGUILayout.Slider("Gaze Event Time (s)", gazeEventTime, 0f, 3f);
        gazeAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(
            "Gaze Animation", 
            gazeAnimationClip, 
            typeof(AnimationClip), 
            false);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Add Gaze Event"))
        {
            if (gazeAnimationClip != null)
            {
                AddGazeAnimationEvent(gazeAnimationClip);
                EditorUtility.SetDirty(gazeAnimationClip);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Success", $"Gaze event added at {gazeEventTime} seconds!", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please assign a Gaze Animation clip first!", "OK");
            }
        }
        
        if (GUILayout.Button("Load MedusaGazeAttack"))
        {
            gazeAnimationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(ANIMATION_PATH + "MedusaGazeAttack.anim");
        }
        
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        chargeEventTime = EditorGUILayout.Slider("Charge Event Time (s)", chargeEventTime, 0f, 3f);
        chargeAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(
            "Charge Animation", 
            chargeAnimationClip, 
            typeof(AnimationClip), 
            false);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Add Charge Event"))
        {
            if (chargeAnimationClip != null)
            {
                AddChargeAnimationEvent(chargeAnimationClip);
                EditorUtility.SetDirty(chargeAnimationClip);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Success", $"Charge event added at {chargeEventTime} seconds!", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please assign a Charge Animation clip first!", "OK");
            }
        }
        
        if (GUILayout.Button("Load MedusaChargeAttack"))
        {
            chargeAnimationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(ANIMATION_PATH + "MedusaChargeAttack.anim");
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.HelpBox(
            "The animation event calls 'OnGazeAnimationKey()' which checks if the player is in front and petrifies them. " +
            "Adjust the event time to match when the gaze effect happens in your animation!",
            MessageType.Info);
    }
    
    private void CreateAnimationSetup()
    {
        CreateAnimationClips();
        AnimatorController controller = CreateAnimatorController();
        
        if (medusaPrefab != null && controller != null)
        {
            AssignControllerToPrefab(controller);
        }
        
        EditorUtility.DisplayDialog("Success", "Medusa Boss animation setup completed!", "OK");
    }
    
    private void CreateAnimationClips()
    {
        if (!AssetDatabase.IsValidFolder(ANIMATION_PATH.TrimEnd('/')))
        {
            Debug.LogError($"Animation folder not found: {ANIMATION_PATH}");
            return;
        }
        
        CreateAnimationClip("MedusaIdle", 60, WrapMode.Loop);
        CreateAnimationClip("MedusaMoving", 60, WrapMode.Loop);
        CreateAnimationClip("MedusaGazeAttack", 60, WrapMode.Once);
        CreateAnimationClip("MedusaChargeAttack", 60, WrapMode.Once);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("Animation clips created successfully!");
    }
    
    private void CreateAnimationClip(string clipName, float frameRate, WrapMode wrapMode)
    {
        string path = ANIMATION_PATH + clipName + ".anim";
        
        if (AssetDatabase.LoadAssetAtPath<AnimationClip>(path) != null)
        {
            Debug.Log($"Animation clip already exists: {clipName}");
            return;
        }
        
        AnimationClip clip = new AnimationClip();
        clip.frameRate = frameRate;
        clip.wrapMode = wrapMode;
        
        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = (wrapMode == WrapMode.Loop);
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        
        if (clipName == "MedusaGazeAttack")
        {
            AddGazeAnimationEvent(clip);
        }
        
        if (clipName == "MedusaChargeAttack")
        {
            AddChargeAnimationEvent(clip);
        }
        
        AssetDatabase.CreateAsset(clip, path);
        Debug.Log($"Created animation clip: {clipName}");
    }
    
    private void AddGazeAnimationEvent(AnimationClip clip)
    {
        AnimationEvent gazeEvent = new AnimationEvent();
        gazeEvent.time = gazeEventTime;
        gazeEvent.functionName = "OnGazeAnimationKey";
        gazeEvent.messageOptions = SendMessageOptions.DontRequireReceiver;
        
        AnimationUtility.SetAnimationEvents(clip, new AnimationEvent[] { gazeEvent });
        
        Debug.Log($"Added gaze animation event at {gazeEventTime} seconds");
    }
    
    private void AddChargeAnimationEvent(AnimationClip clip)
    {
        AnimationEvent chargeEvent = new AnimationEvent();
        chargeEvent.time = chargeEventTime;
        chargeEvent.functionName = "OnChargeAnimationKey";
        chargeEvent.messageOptions = SendMessageOptions.DontRequireReceiver;
        
        AnimationUtility.SetAnimationEvents(clip, new AnimationEvent[] { chargeEvent });
        
        Debug.Log($"Added charge animation event at {chargeEventTime} seconds");
    }
    
    private AnimatorController CreateAnimatorController()
    {
        string controllerPath = ANIMATION_PATH + CONTROLLER_NAME + ".controller";
        
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        
        if (controller != null)
        {
            bool overwrite = EditorUtility.DisplayDialog(
                "Controller Exists", 
                "Animator Controller already exists. Overwrite?", 
                "Yes", 
                "No");
            
            if (!overwrite)
            {
                Debug.Log("Using existing controller.");
                return controller;
            }
            
            AssetDatabase.DeleteAsset(controllerPath);
        }
        
        controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        
        AnimationClip idleClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(ANIMATION_PATH + "MedusaIdle.anim");
        AnimationClip movingClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(ANIMATION_PATH + "MedusaMoving.anim");
        AnimationClip gazeClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(ANIMATION_PATH + "MedusaGazeAttack.anim");
        AnimationClip chargeClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(ANIMATION_PATH + "MedusaChargeAttack.anim");
        
        if (idleClip == null || movingClip == null || gazeClip == null || chargeClip == null)
        {
            Debug.LogError("One or more animation clips not found. Create animations first!");
            return null;
        }
        
        AnimatorControllerLayer layer = controller.layers[0];
        AnimatorStateMachine stateMachine = layer.stateMachine;
        
        controller.AddParameter("Moving", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Idle", AnimatorControllerParameterType.Bool);
        controller.AddParameter("GazeAttack", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("ChargeAttack", AnimatorControllerParameterType.Trigger);
        
        AnimatorState idleState = stateMachine.AddState("Idle", new Vector3(300, 0, 0));
        idleState.motion = idleClip;
        stateMachine.defaultState = idleState;
        
        AnimatorState movingState = stateMachine.AddState("Moving", new Vector3(300, 100, 0));
        movingState.motion = movingClip;
        
        AnimatorState gazeState = stateMachine.AddState("GazeAttack", new Vector3(550, 50, 0));
        gazeState.motion = gazeClip;
        
        AnimatorState chargeState = stateMachine.AddState("ChargeAttack", new Vector3(800, 50, 0));
        chargeState.motion = chargeClip;
        
        AnimatorStateTransition idleToMoving = idleState.AddTransition(movingState);
        idleToMoving.hasExitTime = false;
        idleToMoving.exitTime = 0;
        idleToMoving.duration = 0.1f;
        idleToMoving.AddCondition(AnimatorConditionMode.If, 0, "Moving");
        
        AnimatorStateTransition movingToIdle = movingState.AddTransition(idleState);
        movingToIdle.hasExitTime = false;
        movingToIdle.exitTime = 0;
        movingToIdle.duration = 0.1f;
        movingToIdle.AddCondition(AnimatorConditionMode.If, 0, "Idle");
        movingToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "Moving");
        
        AnimatorStateTransition anyToGaze = stateMachine.AddAnyStateTransition(gazeState);
        anyToGaze.hasExitTime = false;
        anyToGaze.duration = 0.1f;
        anyToGaze.AddCondition(AnimatorConditionMode.If, 0, "GazeAttack");
        
        AnimatorStateTransition gazeToCharge = gazeState.AddTransition(chargeState);
        gazeToCharge.hasExitTime = true;
        gazeToCharge.exitTime = 1.0f;
        gazeToCharge.duration = 0.1f;
        
        AnimatorStateTransition chargeToIdle = chargeState.AddTransition(idleState);
        chargeToIdle.hasExitTime = true;
        chargeToIdle.exitTime = 1.0f;
        chargeToIdle.duration = 0.1f;
        
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"Animator Controller created: {CONTROLLER_NAME}");
        
        return controller;
    }
    
    private void AssignControllerToPrefab(AnimatorController controller)
    {
        if (medusaPrefab == null)
        {
            Debug.LogWarning("No prefab selected. Skipping controller assignment.");
            return;
        }
        
        string prefabPath = AssetDatabase.GetAssetPath(medusaPrefab);
        
        if (string.IsNullOrEmpty(prefabPath))
        {
            Debug.LogError("Selected object is not a prefab asset!");
            return;
        }
        
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
        
        Animator animator = prefabRoot.GetComponent<Animator>();
        
        if (animator == null)
        {
            Debug.LogError("Prefab does not have an Animator component!");
            PrefabUtility.UnloadPrefabContents(prefabRoot);
            return;
        }
        
        animator.runtimeAnimatorController = controller;
        
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
        
        Debug.Log($"Animator Controller assigned to {medusaPrefab.name}");
    }
}
