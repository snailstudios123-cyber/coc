using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

public class FireSwordSpellSetupTool : EditorWindow
{
    [MenuItem("Tools/🔥 Setup Fire Sword Spell")]
    public static void SetupFireSwordSpell()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player == null)
        {
            EditorUtility.DisplayDialog(
                "Player Not Found",
                "Could not find a GameObject tagged 'Player' in the scene.\n\n" +
                "Please make sure your player is in the scene and tagged as 'Player'.",
                "OK"
            );
            return;
        }

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            EditorUtility.DisplayDialog(
                "PlayerController Missing",
                "The Player GameObject doesn't have a PlayerController component.\n\n" +
                "Please add the PlayerController script to your player.",
                "OK"
            );
            return;
        }

        FireSwordSpell fireSwordSpell = player.GetComponent<FireSwordSpell>();
        if (fireSwordSpell == null)
        {
            fireSwordSpell = player.AddComponent<FireSwordSpell>();
            Debug.Log("✓ Added FireSwordSpell component to Player");
        }

        Animator animator = player.GetComponent<Animator>();
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            EditorUtility.DisplayDialog(
                "Animator Missing",
                "The Player doesn't have an Animator or Animator Controller.\n\n" +
                "Please assign an Animator Controller to your player's Animator component.",
                "OK"
            );
            return;
        }

        AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
        if (controller == null)
        {
            EditorUtility.DisplayDialog(
                "Invalid Animator Controller",
                "Could not access the Animator Controller.\n\n" +
                "Make sure your player has a valid Animator Controller assigned.",
                "OK"
            );
            return;
        }

        AnimationClip chargeClip = CreateFireSwordChargeAnimation();
        AnimationClip strikeClip = CreateFireSwordStrikeAnimation();

        AddParametersToAnimator(controller);
        AddStatesToAnimator(controller, chargeClip, strikeClip);

        Transform attackPoint = FindChildTransform(player.transform, "attackPoint");
        if (attackPoint == null)
        {
            attackPoint = FindChildTransform(player.transform, "AttackPoint");
        }

        SerializedObject serializedFireSword = new SerializedObject(fireSwordSpell);
        serializedFireSword.FindProperty("manaPerSecond").floatValue = 0.15f;
        serializedFireSword.FindProperty("maxChargeTime").floatValue = 3f;
        serializedFireSword.FindProperty("strikeCount").intValue = 5;
        serializedFireSword.FindProperty("timeBetweenStrikes").floatValue = 0.1f;
        serializedFireSword.FindProperty("strikeRange").floatValue = 2f;
        serializedFireSword.FindProperty("damagePerStrike").floatValue = 15f;
        
        if (attackPoint != null)
        {
            serializedFireSword.FindProperty("attackPoint").objectReferenceValue = attackPoint;
        }
        
        serializedFireSword.FindProperty("attackArea").vector2Value = new Vector2(2f, 1.5f);
        
        int attackableLayer = LayerMask.NameToLayer("Attackable");
        if (attackableLayer != -1)
        {
            serializedFireSword.FindProperty("enemyLayer").intValue = 1 << attackableLayer;
        }
        
        serializedFireSword.ApplyModifiedProperties();

        SerializedObject serializedPlayer = new SerializedObject(playerController);
        serializedPlayer.FindProperty("fireSwordSpell").objectReferenceValue = fireSwordSpell;
        serializedPlayer.ApplyModifiedProperties();

        EditorUtility.SetDirty(player);
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog(
            "🔥⚔️ Fire Sword Spell Setup Complete! ⚔️🔥",
            "Successfully set up Fire Sword Spell!\n\n" +
            "✓ FireSwordSpell component added\n" +
            "✓ Animation clips created with Knight sprites:\n" +
            "  • FireSwordCharge.anim (Knight cast animation)\n" +
            "  • FireSwordRapidStrike.anim (Knight strike animation)\n" +
            "✓ Animator parameters added:\n" +
            "  • ChargingFireSword (Bool)\n" +
            "  • FireSwordStrike (Trigger)\n" +
            "✓ Animator states and transitions created\n" +
            "✓ Connected to PlayerController\n\n" +
            "HOW TO USE:\n" +
            "HOLD X to charge (plays cast animation, drains mana)\n" +
            "RELEASE X to unleash rapid strikes! (plays strike animation repeatedly)\n\n" +
            "READY TO GO!\n" +
            "The animations are already populated with Knight sprites.\n" +
            "You can replace them with your own fire sword sprites later!\n\n" +
            "STATS:\n" +
            "• 15 damage per strike\n" +
            "• Up to 5 strikes (75 total damage!)\n" +
            "• 0.1s between strikes\n" +
            "• 0.15 mana/sec while charging\n\n" +
            "Adjust settings in the FireSwordSpell component!",
            "Awesome!"
        );

        Selection.activeObject = chargeClip;
        
        Debug.Log("=== FIRE SWORD SPELL SETUP COMPLETE ===");
        Debug.Log("✓ FireSwordCharge animation: Assets/Animation/FireSwordCharge.anim");
        Debug.Log("✓ FireSwordRapidStrike animation: Assets/Animation/FireSwordRapidStrike.anim");
        Debug.Log("✓ Next step: Add your sprite frames to these animations!");
    }

    private static AnimationClip CreateFireSwordChargeAnimation()
    {
        string path = "Assets/Animation/FireSwordCharge.anim";
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        
        bool isNew = false;
        if (clip == null)
        {
            clip = new AnimationClip();
            clip.name = "FireSwordCharge";
            isNew = true;
        }

        Sprite[] chargeSprites = new Sprite[]
        {
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Animation/Art/Knight Files/Knight PNG/Knight_cast_01.png"),
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Animation/Art/Knight Files/Knight PNG/Knight_cast_02.png"),
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Animation/Art/Knight Files/Knight PNG/Knight_cast_03.png"),
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Animation/Art/Knight Files/Knight PNG/Knight_cast_04.png"),
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Animation/Art/Knight Files/Knight PNG/Knight_cast_05.png")
        };

        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";

        ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[chargeSprites.Length];
        float frameRate = 8f;
        
        for (int i = 0; i < chargeSprites.Length; i++)
        {
            spriteKeyFrames[i] = new ObjectReferenceKeyframe();
            spriteKeyFrames[i].time = i / frameRate;
            spriteKeyFrames[i].value = chargeSprites[i];
        }

        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);
        
        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        if (isNew)
        {
            AssetDatabase.CreateAsset(clip, path);
        }
        else
        {
            EditorUtility.SetDirty(clip);
        }
        
        AssetDatabase.SaveAssets();
        
        Debug.Log("✓ Created FireSwordCharge animation with Knight cast sprites at: " + path);
        return clip;
    }

    private static AnimationClip CreateFireSwordStrikeAnimation()
    {
        string path = "Assets/Animation/FireSwordRapidStrike.anim";
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        
        bool isNew = false;
        if (clip == null)
        {
            clip = new AnimationClip();
            clip.name = "FireSwordRapidStrike";
            isNew = true;
        }

        Sprite[] strikeSprites = new Sprite[]
        {
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Animation/Art/Knight Files/Knight PNG/Knight_strike_01.png"),
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Animation/Art/Knight Files/Knight PNG/Knight_strike_02.png"),
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Animation/Art/Knight Files/Knight PNG/Knight_strike_03.png")
        };

        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";

        ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[strikeSprites.Length];
        float frameRate = 20f;
        
        for (int i = 0; i < strikeSprites.Length; i++)
        {
            spriteKeyFrames[i] = new ObjectReferenceKeyframe();
            spriteKeyFrames[i].time = i / frameRate;
            spriteKeyFrames[i].value = strikeSprites[i];
        }

        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);
        
        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = false;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        if (isNew)
        {
            AssetDatabase.CreateAsset(clip, path);
        }
        else
        {
            EditorUtility.SetDirty(clip);
        }
        
        AssetDatabase.SaveAssets();
        
        Debug.Log("✓ Created FireSwordRapidStrike animation with Knight strike sprites at: " + path);
        return clip;
    }

    private static void AddParametersToAnimator(AnimatorController controller)
    {
        bool hasChargingParam = false;
        bool hasStrikeParam = false;

        foreach (AnimatorControllerParameter param in controller.parameters)
        {
            if (param.name == "ChargingFireSword")
                hasChargingParam = true;
            if (param.name == "FireSwordStrike")
                hasStrikeParam = true;
        }

        if (!hasChargingParam)
        {
            controller.AddParameter("ChargingFireSword", AnimatorControllerParameterType.Bool);
            Debug.Log("✓ Added 'ChargingFireSword' Bool parameter");
        }

        if (!hasStrikeParam)
        {
            controller.AddParameter("FireSwordStrike", AnimatorControllerParameterType.Trigger);
            Debug.Log("✓ Added 'FireSwordStrike' Trigger parameter");
        }
    }

    private static void AddStatesToAnimator(AnimatorController controller, AnimationClip chargeClip, AnimationClip strikeClip)
    {
        if (controller.layers.Length == 0)
        {
            Debug.LogWarning("No layers in animator controller");
            return;
        }

        AnimatorControllerLayer baseLayer = controller.layers[0];
        AnimatorStateMachine stateMachine = baseLayer.stateMachine;

        AnimatorState chargeState = FindOrCreateState(stateMachine, "Fire Sword Charge", chargeClip);
        AnimatorState strikeState = FindOrCreateState(stateMachine, "Fire Sword Strike", strikeClip);

        AnimatorState idleState = FindStateByName(stateMachine, "Idle");
        if (idleState == null)
        {
            idleState = FindStateByName(stateMachine, "Player_Idle");
        }
        if (idleState == null)
        {
            Debug.LogWarning("Could not find Idle state. You may need to manually create transitions back to idle.");
        }

        CreateTransitionFromAnyState(stateMachine, chargeState, "ChargingFireSword", true);
        CreateTransitionFromAnyState(stateMachine, strikeState, "FireSwordStrike");

        if (idleState != null)
        {
            CreateTransition(chargeState, idleState, "ChargingFireSword", false);
            CreateTransitionWithExitTime(strikeState, idleState);
        }

        EditorUtility.SetDirty(controller);
        Debug.Log("✓ Added animator states and transitions");
    }

    private static AnimatorState FindOrCreateState(AnimatorStateMachine stateMachine, string stateName, AnimationClip clip)
    {
        foreach (ChildAnimatorState childState in stateMachine.states)
        {
            if (childState.state.name == stateName)
            {
                Debug.Log("State '" + stateName + "' already exists");
                return childState.state;
            }
        }

        AnimatorState newState = stateMachine.AddState(stateName);
        newState.motion = clip;
        Debug.Log("✓ Created state: " + stateName);
        return newState;
    }

    private static AnimatorState FindStateByName(AnimatorStateMachine stateMachine, string stateName)
    {
        foreach (ChildAnimatorState childState in stateMachine.states)
        {
            if (childState.state.name == stateName)
            {
                return childState.state;
            }
        }
        return null;
    }

    private static void CreateTransitionFromAnyState(AnimatorStateMachine stateMachine, AnimatorState targetState, string paramName, bool paramValue)
    {
        foreach (AnimatorStateTransition transition in stateMachine.anyStateTransitions)
        {
            if (transition.destinationState == targetState)
            {
                Debug.Log("Transition from Any State to '" + targetState.name + "' already exists");
                return;
            }
        }

        AnimatorStateTransition newTransition = stateMachine.AddAnyStateTransition(targetState);
        newTransition.hasExitTime = false;
        newTransition.exitTime = 0f;
        newTransition.duration = 0f;
        newTransition.AddCondition(AnimatorConditionMode.If, 0, paramName);
        
        Debug.Log("✓ Created transition: Any State → " + targetState.name);
    }

    private static void CreateTransitionFromAnyState(AnimatorStateMachine stateMachine, AnimatorState targetState, string triggerName)
    {
        foreach (AnimatorStateTransition transition in stateMachine.anyStateTransitions)
        {
            if (transition.destinationState == targetState)
            {
                Debug.Log("Transition from Any State to '" + targetState.name + "' already exists");
                return;
            }
        }

        AnimatorStateTransition newTransition = stateMachine.AddAnyStateTransition(targetState);
        newTransition.hasExitTime = false;
        newTransition.exitTime = 0f;
        newTransition.duration = 0f;
        newTransition.AddCondition(AnimatorConditionMode.If, 0, triggerName);
        
        Debug.Log("✓ Created transition: Any State → " + targetState.name);
    }

    private static void CreateTransition(AnimatorState fromState, AnimatorState toState, string paramName, bool paramValue)
    {
        foreach (AnimatorStateTransition transition in fromState.transitions)
        {
            if (transition.destinationState == toState)
            {
                Debug.Log("Transition from '" + fromState.name + "' to '" + toState.name + "' already exists");
                return;
            }
        }

        AnimatorStateTransition newTransition = fromState.AddTransition(toState);
        newTransition.hasExitTime = false;
        newTransition.exitTime = 0f;
        newTransition.duration = 0.1f;
        
        AnimatorConditionMode mode = paramValue ? AnimatorConditionMode.IfNot : AnimatorConditionMode.If;
        newTransition.AddCondition(mode, 0, paramName);
        
        Debug.Log("✓ Created transition: " + fromState.name + " → " + toState.name);
    }

    private static void CreateTransitionWithExitTime(AnimatorState fromState, AnimatorState toState)
    {
        foreach (AnimatorStateTransition transition in fromState.transitions)
        {
            if (transition.destinationState == toState)
            {
                Debug.Log("Transition from '" + fromState.name + "' to '" + toState.name + "' already exists");
                return;
            }
        }

        AnimatorStateTransition newTransition = fromState.AddTransition(toState);
        newTransition.hasExitTime = true;
        newTransition.exitTime = 1.0f;
        newTransition.duration = 0.1f;
        
        Debug.Log("✓ Created transition: " + fromState.name + " → " + toState.name);
    }

    private static Transform FindChildTransform(Transform parent, string name)
    {
        if (parent.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
            return parent;

        foreach (Transform child in parent)
        {
            Transform result = FindChildTransform(child, name);
            if (result != null)
                return result;
        }

        return null;
    }
}
