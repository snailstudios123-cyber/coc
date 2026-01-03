using UnityEngine;
using UnityEditor;

public class FixSkeletonAttackAnimation : EditorWindow
{
    [MenuItem("Tools/Fix Skeleton Attack Animation")]
    public static void AddAttackEvent()
    {
        AnimationClip attack1 = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animation/Skeleton/Skeleton_Attack1.anim");
        AnimationClip attack2 = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animation/Skeleton/Skeleton_Attack2.anim");
        
        if (attack1 != null)
        {
            AddEventToAnimation(attack1, "Skeleton_Attack1");
        }
        else
        {
            Debug.LogError("Could not find Skeleton_Attack1.anim");
        }
        
        if (attack2 != null)
        {
            AddEventToAnimation(attack2, "Skeleton_Attack2");
        }
        else
        {
            Debug.LogWarning("Could not find Skeleton_Attack2.anim (optional)");
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("✅ Skeleton attack animations fixed! The skeleton will now damage the player.");
    }
    
    private static void AddEventToAnimation(AnimationClip clip, string animName)
    {
        AnimationEvent[] existingEvents = AnimationUtility.GetAnimationEvents(clip);
        
        bool hasHitEvent = false;
        foreach (AnimationEvent evt in existingEvents)
        {
            if (evt.functionName == "OnAttackAnimationHit")
            {
                hasHitEvent = true;
                Debug.Log($"Animation {animName} already has OnAttackAnimationHit event.");
                break;
            }
        }
        
        if (!hasHitEvent)
        {
            float hitTime = clip.length * 0.5f;
            
            AnimationEvent hitEvent = new AnimationEvent
            {
                time = hitTime,
                functionName = "OnAttackAnimationHit"
            };
            
            AnimationEvent[] newEvents = new AnimationEvent[existingEvents.Length + 1];
            for (int i = 0; i < existingEvents.Length; i++)
            {
                newEvents[i] = existingEvents[i];
            }
            newEvents[existingEvents.Length] = hitEvent;
            
            AnimationUtility.SetAnimationEvents(clip, newEvents);
            
            Debug.Log($"✅ Added OnAttackAnimationHit event to {animName} at time {hitTime:F2}s");
        }
    }
}
