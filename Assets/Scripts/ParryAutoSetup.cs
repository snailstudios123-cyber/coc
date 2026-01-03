using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ParryAutoSetup : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Setup Parry System")]
    public static void SetupParrySystem()
    {
        GameObject parrySystemObj = GameObject.Find("ParrySystem");
        
        if (parrySystemObj == null)
        {
            parrySystemObj = new GameObject("ParrySystem");
            Debug.Log("Created ParrySystem GameObject");
        }
        
        ParrySystem parrySystem = parrySystemObj.GetComponent<ParrySystem>();
        if (parrySystem == null)
        {
            parrySystem = parrySystemObj.AddComponent<ParrySystem>();
            Debug.Log("Added ParrySystem component");
        }
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int setupCount = 0;
        
        foreach (GameObject enemy in enemies)
        {
            Skeleton skeleton = enemy.GetComponent<Skeleton>();
            if (skeleton != null)
            {
                SerializedObject so = new SerializedObject(skeleton);
                SerializedProperty attackPoint = so.FindProperty("attackPoint");
                
                if (attackPoint.objectReferenceValue != null)
                {
                    Transform attackTransform = attackPoint.objectReferenceValue as Transform;
                    
                    if (attackTransform != null)
                    {
                        EnemyAttackWarning warning = attackTransform.GetComponent<EnemyAttackWarning>();
                        if (warning == null)
                        {
                            attackTransform.gameObject.AddComponent<EnemyAttackWarning>();
                            setupCount++;
                        }
                    }
                }
                
                so.ApplyModifiedProperties();
            }
        }
        
        Debug.Log($"=== PARRY SYSTEM SETUP COMPLETE ===");
        Debug.Log($"Setup {setupCount} enemy attack points with parry detection.");
        Debug.Log($"HOW TO PARRY: Attack just before an enemy's attack lands (within 0.2s window)");
        Debug.Log($"REWARDS: Brief invulnerability + slow-motion + bonus damage + enemy stun!");
        
        Selection.activeGameObject = parrySystemObj;
    }
#endif
}
