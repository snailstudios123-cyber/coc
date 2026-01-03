using UnityEngine;

public class ClimbingDebugger : MonoBehaviour
{
    private PlayerController player;
    private PlayerStateList pState;
    private Animator anim;
    private Rigidbody2D rb;
    private float logTimer = 0f;
    private const float LOG_INTERVAL = 0.5f;

    void Start()
    {
        player = GetComponent<PlayerController>();
        pState = GetComponent<PlayerStateList>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        bool hasClimbingParam = false;
        bool hasJumpingParam = false;
        bool hasWalkingParam = false;
        
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name == "Climbing") hasClimbingParam = true;
            if (param.name == "Jumping") hasJumpingParam = true;
            if (param.name == "Walking") hasWalkingParam = true;
        }

        Debug.Log($"Animator Parameters - Climbing: {hasClimbingParam}, Jumping: {hasJumpingParam}, Walking: {hasWalkingParam}");
    }

    void Update()
    {
        logTimer += Time.deltaTime;
        if (logTimer >= LOG_INTERVAL)
        {
            logTimer = 0f;
            
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            float normalizedTime = stateInfo.normalizedTime;
            
            Debug.Log($"[CLIMBING DEBUG] Grounded: {player.Grounded()} | " +
                     $"Climbing: {pState.climbing} | " +
                     $"Jumping: {pState.jumping} | " +
                     $"Anim Climb: {anim.GetBool("Climbing")} | " +
                     $"Anim Jump: {anim.GetBool("Jumping")} | " +
                     $"State: {GetCurrentAnimatorState()} | " +
                     $"Normalized Time: {normalizedTime:F3} | " +
                     $"Anim Speed: {anim.speed} | " +
                     $"Velocity: {rb.velocity}");
        }
    }

    void OnGUI()
    {
        if (pState == null) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 14;
        style.normal.textColor = Color.white;
        style.normal.background = MakeTex(2, 2, new Color(0, 0, 0, 0.5f));

        int yPos = 10;
        int lineHeight = 22;

        GUI.Label(new Rect(10, yPos, 400, 20), $"Grounded: {player.Grounded()}", style);
        yPos += lineHeight;
        
        GUI.Label(new Rect(10, yPos, 400, 20), $"Climbing State: {pState.climbing}", style);
        yPos += lineHeight;
        
        GUI.Label(new Rect(10, yPos, 400, 20), $"Jumping State: {pState.jumping}", style);
        yPos += lineHeight;
        
        if (anim != null)
        {
            GUI.Label(new Rect(10, yPos, 400, 20), $"Anim Climbing: {anim.GetBool("Climbing")}", style);
            yPos += lineHeight;
            
            GUI.Label(new Rect(10, yPos, 400, 20), $"Anim Jumping: {anim.GetBool("Jumping")}", style);
            yPos += lineHeight;
            
            GUI.Label(new Rect(10, yPos, 400, 20), $"Anim Walking: {anim.GetBool("Walking")}", style);
            yPos += lineHeight;
            
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            GUI.Label(new Rect(10, yPos, 400, 20), $"Anim Time: {stateInfo.normalizedTime:F3}", style);
            yPos += lineHeight;
            
            GUI.Label(new Rect(10, yPos, 400, 20), $"Anim Speed: {anim.speed:F2}", style);
            yPos += lineHeight;
        }
        
        GUI.Label(new Rect(10, yPos, 400, 20), $"Velocity: {rb.velocity}", style);
        yPos += lineHeight;
        
        GUI.Label(new Rect(10, yPos, 400, 20), $"Current State: {GetCurrentAnimatorState()}", style);
    }

    private string GetCurrentAnimatorState()
    {
        if (anim == null) return "N/A";
        
        AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            return clipInfo[0].clip.name;
        }
        return "Unknown";
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
