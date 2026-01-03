using UnityEngine;

public class SkeletonAnimatorDebug : MonoBehaviour
{
    private Animator anim;
    private float logTimer = 0f;
    private float logInterval = 2f;

    void Start()
    {
        anim = GetComponent<Animator>();
        
        if (anim == null)
        {
            Debug.LogError("SkeletonAnimatorDebug: No Animator found!");
            enabled = false;
            return;
        }
        
        if (anim.runtimeAnimatorController == null)
        {
            Debug.LogError("SkeletonAnimatorDebug: No controller assigned!");
            enabled = false;
            return;
        }
        
        Debug.Log("SkeletonAnimatorDebug: Started monitoring Skeleton animator");
    }

    void Update()
    {
        if (anim == null) return;

        logTimer += Time.deltaTime;
        
        if (logTimer >= logInterval)
        {
            logTimer = 0f;
            
            AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(0);
            string currentClip = clipInfo.Length > 0 ? clipInfo[0].clip.name : "NONE";
            
            bool isWalking = anim.GetBool("isWalking");
            
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            string spriteName = sr != null && sr.sprite != null ? sr.sprite.name : "NULL SPRITE";
            
            Debug.Log($"[ANIMATOR] Clip: {currentClip} | isWalking: {isWalking} | Sprite: {spriteName}");
        }
    }
}
