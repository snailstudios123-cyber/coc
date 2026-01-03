using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

public class BringerOfDeathSetupTool : EditorWindow
{
    private GameObject coinPrefab;

    [MenuItem("Tools/Bringer of Death Setup")]
    public static void ShowWindow()
    {
        GetWindow<BringerOfDeathSetupTool>("Bringer of Death Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Bringer of Death Automated Setup", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("This tool will create:\n" +
            "✅ Animator Controllers (Bringer of Death & Death Spell)\n" +
            "✅ Enemy GameObject with all components\n" +
            "✅ Death Spell Prefab (Portal with hand attack)\n" +
            "✅ Attack Points for enemy and spell\n\n" +
            "Click the button below to create everything!\n" +
            "You'll add sprites and animations manually after.", MessageType.Info);

        EditorGUILayout.Space();

        coinPrefab = (GameObject)EditorGUILayout.ObjectField("Coin Prefab (Optional)", coinPrefab, typeof(GameObject), false);

        EditorGUILayout.Space();

        if (GUILayout.Button("🎮 CREATE COMPLETE SETUP", GUILayout.Height(50)))
        {
            CreateCompleteSetup();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        GUILayout.Label("Advanced Options:", EditorStyles.boldLabel);

        if (GUILayout.Button("Create Animator Controllers Only", GUILayout.Height(30)))
        {
            CreateAnimatorControllers();
        }

        if (GUILayout.Button("Create GameObjects Only", GUILayout.Height(30)))
        {
            CreateGameObjects();
        }
    }

    private void CreateCompleteSetup()
    {
        AnimatorController enemyController = CreateAnimatorControllers();
        CreateGameObjects();
        
        EditorUtility.DisplayDialog("✅ Setup Complete!", 
            "Bringer of Death setup is complete!\n\n" +
            "✅ Animator Controllers created\n" +
            "✅ Enemy GameObject created in scene\n" +
            "✅ Death Spell prefab created\n" +
            "✅ All components configured\n\n" +
            "Next steps:\n" +
            "1. Slice your sprite sheet\n" +
            "2. Create animation clips\n" +
            "3. Add animations to the Animator Controllers\n" +
            "4. Assign sprites to SpriteRenderer components", "OK");
    }

    private AnimatorController CreateAnimatorControllers()
    {
        string animationFolder = "Assets/Animation/Bringer of Death";
        
        if (!Directory.Exists(animationFolder))
        {
            Directory.CreateDirectory(animationFolder);
            AssetDatabase.Refresh();
        }

        AnimatorController enemyController = CreateBringerOfDeathController(animationFolder);
        AnimatorController spellController = CreateDeathSpellController(animationFolder);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ Animator Controllers created successfully!");
        EditorGUIUtility.PingObject(enemyController);

        return enemyController;
    }

    private AnimatorController CreateBringerOfDeathController(string folder)
    {
        string controllerPath = folder + "/BringerOfDeathController.controller";
        
        if (File.Exists(controllerPath))
        {
            if (!EditorUtility.DisplayDialog("Controller Exists", 
                "BringerOfDeathController already exists. Overwrite?", "Yes", "No"))
            {
                return AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            }
            AssetDatabase.DeleteAsset(controllerPath);
        }

        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        controller.AddParameter("isWalking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("attack", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("cast", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("hurt", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("die", AnimatorControllerParameterType.Trigger);

        AnimatorControllerLayer layer = controller.layers[0];
        AnimatorStateMachine stateMachine = layer.stateMachine;

        AnimatorState idleState = stateMachine.AddState("Idle", new Vector3(300, 0, 0));
        AnimatorState walkState = stateMachine.AddState("Walk", new Vector3(300, 80, 0));
        AnimatorState attackState = stateMachine.AddState("Attack", new Vector3(300, 160, 0));
        AnimatorState castState = stateMachine.AddState("Cast", new Vector3(300, 240, 0));
        AnimatorState hurtState = stateMachine.AddState("Hurt", new Vector3(300, 320, 0));
        AnimatorState deathState = stateMachine.AddState("Death", new Vector3(300, 400, 0));

        stateMachine.defaultState = idleState;

        AnimatorStateTransition idleToWalk = idleState.AddTransition(walkState);
        idleToWalk.AddCondition(AnimatorConditionMode.If, 0, "isWalking");
        idleToWalk.hasExitTime = false;
        idleToWalk.duration = 0.25f;

        AnimatorStateTransition walkToIdle = walkState.AddTransition(idleState);
        walkToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "isWalking");
        walkToIdle.hasExitTime = false;
        walkToIdle.duration = 0.25f;

        AnimatorStateTransition anyToAttack = stateMachine.AddAnyStateTransition(attackState);
        anyToAttack.AddCondition(AnimatorConditionMode.If, 0, "attack");
        anyToAttack.hasExitTime = false;
        anyToAttack.duration = 0.1f;

        AnimatorStateTransition anyToCast = stateMachine.AddAnyStateTransition(castState);
        anyToCast.AddCondition(AnimatorConditionMode.If, 0, "cast");
        anyToCast.hasExitTime = false;
        anyToCast.duration = 0.1f;

        AnimatorStateTransition anyToHurt = stateMachine.AddAnyStateTransition(hurtState);
        anyToHurt.AddCondition(AnimatorConditionMode.If, 0, "hurt");
        anyToHurt.hasExitTime = false;
        anyToHurt.duration = 0.1f;

        AnimatorStateTransition anyToDeath = stateMachine.AddAnyStateTransition(deathState);
        anyToDeath.AddCondition(AnimatorConditionMode.If, 0, "die");
        anyToDeath.hasExitTime = false;
        anyToDeath.duration = 0.1f;

        AnimatorStateTransition attackToIdle = attackState.AddTransition(idleState);
        attackToIdle.hasExitTime = true;
        attackToIdle.exitTime = 0.9f;
        attackToIdle.duration = 0.1f;

        AnimatorStateTransition castToIdle = castState.AddTransition(idleState);
        castToIdle.hasExitTime = true;
        castToIdle.exitTime = 0.9f;
        castToIdle.duration = 0.1f;

        AnimatorStateTransition hurtToIdle = hurtState.AddTransition(idleState);
        hurtToIdle.hasExitTime = true;
        hurtToIdle.exitTime = 0.9f;
        hurtToIdle.duration = 0.1f;

        EditorUtility.SetDirty(controller);
        Debug.Log("BringerOfDeathController created at: " + controllerPath);
        
        return controller;
    }

    private AnimatorController CreateDeathSpellController(string folder)
    {
        string controllerPath = folder + "/DeathSpellController.controller";
        
        if (File.Exists(controllerPath))
        {
            if (!EditorUtility.DisplayDialog("Controller Exists", 
                "DeathSpellController already exists. Overwrite?", "Yes", "No"))
            {
                return AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            }
            AssetDatabase.DeleteAsset(controllerPath);
        }

        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        controller.AddParameter("spell", AnimatorControllerParameterType.Trigger);

        AnimatorControllerLayer layer = controller.layers[0];
        AnimatorStateMachine stateMachine = layer.stateMachine;

        AnimatorState spellState = stateMachine.AddState("Spell", new Vector3(300, 0, 0));
        stateMachine.defaultState = spellState;

        EditorUtility.SetDirty(controller);
        Debug.Log("DeathSpellController created at: " + controllerPath);
        
        return controller;
    }

    private void CreateGameObjects()
    {
        string animationFolder = "Assets/Animation/Bringer of Death";
        string enemyControllerPath = animationFolder + "/BringerOfDeathController.controller";
        string spellControllerPath = animationFolder + "/DeathSpellController.controller";

        AnimatorController enemyController = AssetDatabase.LoadAssetAtPath<AnimatorController>(enemyControllerPath);
        AnimatorController spellController = AssetDatabase.LoadAssetAtPath<AnimatorController>(spellControllerPath);

        GameObject deathSpellPrefab = CreateDeathSpellPrefab(spellController);
        GameObject enemyObject = CreateBringerOfDeathGameObject(enemyController, deathSpellPrefab);

        Selection.activeGameObject = enemyObject;
        EditorGUIUtility.PingObject(enemyObject);

        Debug.Log("✅ GameObjects created successfully!");
    }

    private GameObject CreateBringerOfDeathGameObject(AnimatorController controller, GameObject spellPrefab)
    {
        GameObject enemy = new GameObject("Bringer of Death");
        enemy.tag = "Enemy";
        enemy.layer = LayerMask.NameToLayer("Attackable");

        SpriteRenderer sr = enemy.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Default";

        Animator anim = enemy.AddComponent<Animator>();
        if (controller != null)
        {
            anim.runtimeAnimatorController = controller;
        }
        anim.applyRootMotion = false;
        anim.updateMode = AnimatorUpdateMode.Normal;

        Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        BoxCollider2D col = enemy.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1f, 2f);
        col.offset = new Vector2(0f, 0f);

        BringerOfDeath script = enemy.AddComponent<BringerOfDeath>();
        
        SetPrivateField(script, "health", 20f);
        SetPrivateField(script, "maxHealth", 20f);
        SetPrivateField(script, "recoilLength", 0.1f);
        SetPrivateField(script, "recoilFactor", 2f);
        SetPrivateField(script, "speed", 2f);
        SetPrivateField(script, "damage", 1f);
        
        if (coinPrefab != null)
        {
            SetPrivateField(script, "coinPrefab", coinPrefab);
        }
        SetPrivateField(script, "minCoins", 1);
        SetPrivateField(script, "maxCoins", 3);
        SetPrivateField(script, "coinDropChance", 0.8f);
        
        SetPrivateField(script, "detectionRange", 10f);
        SetPrivateField(script, "losePlayerRange", 12f);
        SetPrivateField(script, "spellCastRange", 7f);
        SetPrivateField(script, "attackRange", 2f);
        SetPrivateField(script, "meleeAttackCooldown", 3f);
        SetPrivateField(script, "spellCastCooldown", 5f);
        SetPrivateField(script, "walkSpeed", 2f);
        SetPrivateField(script, "chaseSpeed", 3f);
        SetPrivateField(script, "attackArea", new Vector2(2f, 2f));
        SetPrivateField(script, "attackPointOffset", new Vector3(1f, 0.5f, 0f));
        
        if (spellPrefab != null)
        {
            SetPrivateField(script, "spellPrefab", spellPrefab);
        }
        SetPrivateField(script, "spellDamage", 2f);
        SetPrivateField(script, "spellSpawnHeight", 5f);

        GameObject attackPoint = new GameObject("AttackPoint");
        attackPoint.transform.SetParent(enemy.transform);
        attackPoint.transform.localPosition = new Vector3(1f, 0.5f, 0f);
        
        SetPrivateField(script, "attackPoint", attackPoint.transform);

        Debug.Log("✅ Bringer of Death GameObject created in scene!");
        
        return enemy;
    }

    private GameObject CreateDeathSpellPrefab(AnimatorController controller)
    {
        string prefabFolder = "Assets/Prefabs";
        
        if (!Directory.Exists(prefabFolder))
        {
            Directory.CreateDirectory(prefabFolder);
            AssetDatabase.Refresh();
        }

        string prefabPath = prefabFolder + "/DeathSpell.prefab";

        GameObject spell = new GameObject("DeathSpell");

        SpriteRenderer sr = spell.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Default";

        Animator anim = spell.AddComponent<Animator>();
        if (controller != null)
        {
            anim.runtimeAnimatorController = controller;
        }

        Rigidbody2D rb = spell.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        DeathSpell script = spell.AddComponent<DeathSpell>();
        SetPrivateField(script, "lifetime", 2f);
        SetPrivateField(script, "attackDelay", 0.5f);
        SetPrivateField(script, "attackArea", new Vector2(1.5f, 2f));
        SetPrivateField(script, "attackPointOffset", new Vector3(0f, -2f, 0f));

        GameObject attackPoint = new GameObject("SpellAttackPoint");
        attackPoint.transform.SetParent(spell.transform);
        attackPoint.transform.localPosition = new Vector3(0f, -2f, 0f);
        
        SetPrivateField(script, "attackPoint", attackPoint.transform);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(spell, prefabPath);
        
        DestroyImmediate(spell);

        Debug.Log("✅ Death Spell prefab created at: " + prefabPath);
        
        return prefab;
    }

    private void SetPrivateField(object obj, string fieldName, object value)
    {
        var type = obj.GetType();
        var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            field.SetValue(obj, value);
        }
        else
        {
            Debug.LogWarning($"Field '{fieldName}' not found on type '{type.Name}'");
        }
    }
}
