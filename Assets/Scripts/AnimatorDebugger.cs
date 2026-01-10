using UnityEngine;

public class AnimatorDebugger : MonoBehaviour
{
    private Animator anim;
    private bool wasDashing = false;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim != null)
        {
            Debug.Log("=== ANIMATOR DEBUGGER ACTIVE ===");
            Debug.Log($"Controller: {anim.runtimeAnimatorController.name}");
            Debug.Log($"Parameters:");
            foreach (var param in anim.parameters)
            {
                Debug.Log($"  - {param.name} ({param.type})");
            }
        }
    }
    
    void Update()
    {
        if (anim == null) return;
        
        PlayerStateList pState = GetComponent<PlayerStateList>();
        if (pState != null && pState.dashing && !wasDashing)
        {
            wasDashing = true;
            AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(0);
            Debug.Log($"[ANIMATOR] Dashing started! Current clip: {(clipInfo.Length > 0 ? clipInfo[0].clip.name : "NONE")}");
            
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            Debug.Log($"[ANIMATOR] Current state hash: {stateInfo.fullPathHash}");
            Debug.Log($"[ANIMATOR] State normalized time: {stateInfo.normalizedTime}");
        }
        else if (pState != null && !pState.dashing && wasDashing)
        {
            wasDashing = false;
            AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(0);
            Debug.Log($"[ANIMATOR] Dashing ended! Current clip: {(clipInfo.Length > 0 ? clipInfo[0].clip.name : "NONE")}");
        }
    }
}
