using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class AddBeamAnimationTool : EditorWindow
{
    private AnimatorController animatorController;
    private AnimationClip chargingAnimation;
    private AnimationClip fireAnimation;
    
    [MenuItem("Tools/Add Beam Animation to Animator")]
    public static void ShowWindow()
    {
        AddBeamAnimationTool window = GetWindow<AddBeamAnimationTool>("Add Beam Animation");
        window.minSize = new Vector2(400, 350);
        window.Show();
    }
    
    private void OnEnable()
    {
        animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Animation/Art/Electus.controller");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Add Beam Animation to Animator", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox("This tool will add ChargingBeam and FireBeam animations to your player animator.", MessageType.Info);
        EditorGUILayout.Space();
        
        animatorController = EditorGUILayout.ObjectField("Animator Controller", animatorController, typeof(AnimatorController), false) as AnimatorController;
        
        EditorGUILayout.Space();
        GUILayout.Label("Animation Clips (Optional)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Leave empty to create placeholder animations. You can replace them later with your own animations.", MessageType.None);
        
        chargingAnimation = EditorGUILayout.ObjectField("Charging Animation", chargingAnimation, typeof(AnimationClip), false) as AnimationClip;
        fireAnimation = EditorGUILayout.ObjectField("Fire Beam Animation", fireAnimation, typeof(AnimationClip), false) as AnimationClip;
        
        EditorGUILayout.Space();
        
        GUI.enabled = animatorController != null;
        
        if (GUILayout.Button("Add Beam Animations", GUILayout.Height(40)))
        {
            AddBeamAnimations();
        }
        
        GUI.enabled = true;
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This will add:\n• ChargingBeam (bool) parameter\n• FireBeam (trigger) parameter\n• Charging animation state\n• Fire beam animation state\n• Transitions between states", MessageType.None);
    }
    
    private void AddBeamAnimations()
    {
        if (animatorController == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign an Animator Controller!", "OK");
            return;
        }
        
        try
        {
            if (chargingAnimation == null)
            {
                chargingAnimation = CreatePlaceholderAnimation("BeamCharging", "Assets/Animation/BeamCharging.anim");
            }
            
            if (fireAnimation == null)
            {
                fireAnimation = CreatePlaceholderAnimation("BeamFire", "Assets/Animation/BeamFire.anim");
            }
            
            AddParametersToAnimator();
            
            AddAnimationStates();
            
            EditorUtility.SetDirty(animatorController);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("Success", "Beam animations added successfully!\n\nParameters added:\n• ChargingBeam (bool)\n• FireBeam (trigger)\n\nYou can now customize the animations in the Animator window.", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", "Failed to add beam animations: " + e.Message, "OK");
            Debug.LogError("Add Beam Animation Error: " + e.Message);
        }
    }
    
    private AnimationClip CreatePlaceholderAnimation(string name, string path)
    {
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        
        if (clip != null)
        {
            return clip;
        }
        
        clip = new AnimationClip();
        clip.name = name;
        clip.frameRate = 12;
        
        AnimationCurve curve = AnimationCurve.Constant(0, 0.5f, 0);
        clip.SetCurve("", typeof(Transform), "localPosition.x", curve);
        
        AssetDatabase.CreateAsset(clip, path);
        AssetDatabase.SaveAssets();
        
        return clip;
    }
    
    private void AddParametersToAnimator()
    {
        bool hasChargingBeam = false;
        bool hasFireBeam = false;
        
        foreach (AnimatorControllerParameter param in animatorController.parameters)
        {
            if (param.name == "ChargingBeam")
                hasChargingBeam = true;
            if (param.name == "FireBeam")
                hasFireBeam = true;
        }
        
        if (!hasChargingBeam)
        {
            animatorController.AddParameter("ChargingBeam", AnimatorControllerParameterType.Bool);
            Debug.Log("Added parameter: ChargingBeam (bool)");
        }
        
        if (!hasFireBeam)
        {
            animatorController.AddParameter("FireBeam", AnimatorControllerParameterType.Trigger);
            Debug.Log("Added parameter: FireBeam (trigger)");
        }
    }
    
    private void AddAnimationStates()
    {
        AnimatorControllerLayer baseLayer = animatorController.layers[0];
        AnimatorStateMachine stateMachine = baseLayer.stateMachine;
        
        AnimatorState chargingState = null;
        AnimatorState fireState = null;
        AnimatorState idleState = null;
        
        foreach (ChildAnimatorState childState in stateMachine.states)
        {
            if (childState.state.name == "BeamCharging")
            {
                chargingState = childState.state;
            }
            else if (childState.state.name == "BeamFire")
            {
                fireState = childState.state;
            }
            else if (childState.state.name.Contains("Idle") || childState.state.name.Contains("idle"))
            {
                idleState = childState.state;
            }
        }
        
        if (idleState == null && stateMachine.states.Length > 0)
        {
            idleState = stateMachine.states[0].state;
        }
        
        if (chargingState == null)
        {
            chargingState = stateMachine.AddState("BeamCharging");
            chargingState.motion = chargingAnimation;
            
            Vector3 position = new Vector3(300, 100, 0);
            if (stateMachine.states.Length > 0)
            {
                position = stateMachine.states[stateMachine.states.Length - 1].position;
                position.y += 80;
            }
            
            ChildAnimatorState[] states = stateMachine.states;
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].state == chargingState)
                {
                    states[i].position = position;
                    stateMachine.states = states;
                    break;
                }
            }
            
            Debug.Log("Added state: BeamCharging");
        }
        
        if (fireState == null)
        {
            fireState = stateMachine.AddState("BeamFire");
            fireState.motion = fireAnimation;
            
            Vector3 position = new Vector3(500, 100, 0);
            if (stateMachine.states.Length > 0)
            {
                position = stateMachine.states[stateMachine.states.Length - 1].position;
                position.y += 80;
            }
            
            ChildAnimatorState[] states = stateMachine.states;
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].state == fireState)
                {
                    states[i].position = position;
                    stateMachine.states = states;
                    break;
                }
            }
            
            Debug.Log("Added state: BeamFire");
        }
        
        if (idleState != null)
        {
            bool hasTransitionToCharging = false;
            foreach (AnimatorStateTransition transition in idleState.transitions)
            {
                if (transition.destinationState == chargingState)
                {
                    hasTransitionToCharging = true;
                    break;
                }
            }
            
            if (!hasTransitionToCharging)
            {
                AnimatorStateTransition toCharging = idleState.AddTransition(chargingState);
                toCharging.AddCondition(AnimatorConditionMode.If, 0, "ChargingBeam");
                toCharging.hasExitTime = false;
                toCharging.duration = 0.1f;
                Debug.Log("Added transition: Idle -> BeamCharging");
            }
        }
        
        bool hasTransitionChargingToIdle = false;
        foreach (AnimatorStateTransition transition in chargingState.transitions)
        {
            if (transition.destinationState == idleState)
            {
                hasTransitionChargingToIdle = true;
                break;
            }
        }
        
        if (!hasTransitionChargingToIdle && idleState != null)
        {
            AnimatorStateTransition toIdle = chargingState.AddTransition(idleState);
            toIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "ChargingBeam");
            toIdle.hasExitTime = false;
            toIdle.duration = 0.1f;
            Debug.Log("Added transition: BeamCharging -> Idle");
        }
        
        bool hasTransitionChargingToFire = false;
        foreach (AnimatorStateTransition transition in chargingState.transitions)
        {
            if (transition.destinationState == fireState)
            {
                hasTransitionChargingToFire = true;
                break;
            }
        }
        
        if (!hasTransitionChargingToFire)
        {
            AnimatorStateTransition toFire = chargingState.AddTransition(fireState);
            toFire.AddCondition(AnimatorConditionMode.If, 0, "FireBeam");
            toFire.hasExitTime = false;
            toFire.duration = 0.05f;
            Debug.Log("Added transition: BeamCharging -> BeamFire");
        }
        
        bool hasTransitionFireToIdle = false;
        foreach (AnimatorStateTransition transition in fireState.transitions)
        {
            if (transition.destinationState == idleState)
            {
                hasTransitionFireToIdle = true;
                break;
            }
        }
        
        if (!hasTransitionFireToIdle && idleState != null)
        {
            AnimatorStateTransition fireToIdle = fireState.AddTransition(idleState);
            fireToIdle.hasExitTime = true;
            fireToIdle.exitTime = 0.9f;
            fireToIdle.duration = 0.1f;
            Debug.Log("Added transition: BeamFire -> Idle");
        }
    }
}
