using UnityEngine;

public class LedgeClimbDebugger : MonoBehaviour
{
    private PlayerController pc;
    private PlayerStateList pState;
    
    [SerializeField] private LayerMask climbableLayer;
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private Transform ledgeCheckPoint;
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private float ledgeCheckDistance = 0.5f;
    
    private float logTimer = 0f;
    private float logInterval = 0.5f;

    void Start()
    {
        pc = GetComponent<PlayerController>();
        pState = GetComponent<PlayerStateList>();
    }

    void Update()
    {
        if (pState.climbing)
        {
            logTimer += Time.deltaTime;
            if (logTimer >= logInterval)
            {
                logTimer = 0f;
                CheckLedgeConditions();
            }
        }
    }

    void CheckLedgeConditions()
    {
        if (wallCheckPoint == null || ledgeCheckPoint == null)
        {
            Debug.LogWarning("Wall or Ledge check point is missing!");
            return;
        }

        Vector2 direction = pState.lookingRight ? Vector2.right : Vector2.left;
        
        RaycastHit2D wallHit = Physics2D.Raycast(
            wallCheckPoint.position, 
            direction, 
            wallCheckDistance, 
            climbableLayer
        );
        
        RaycastHit2D ledgeHit = Physics2D.Raycast(
            ledgeCheckPoint.position, 
            direction, 
            ledgeCheckDistance, 
            climbableLayer
        );
        
        Debug.Log($"[LEDGE DEBUG] Climbing: {pState.climbing}");
        Debug.Log($"  Wall Check: {(wallHit.collider != null ? "HIT" : "MISS")} at {wallCheckPoint.position}");
        Debug.Log($"  Ledge Check: {(ledgeHit.collider != null ? "HIT" : "MISS")} at {ledgeCheckPoint.position}");
        Debug.Log($"  Looking Right: {pState.lookingRight}");
        Debug.Log($"  Can Ledge Climb: {wallHit.collider == null && ledgeHit.collider != null}");
    }

    void OnDrawGizmos()
    {
        if (pState == null) return;
        
        if (wallCheckPoint != null)
        {
            Gizmos.color = Color.cyan;
            Vector2 direction = pState.lookingRight ? Vector2.right : Vector2.left;
            Gizmos.DrawRay(wallCheckPoint.position, direction * wallCheckDistance);
            Gizmos.DrawWireSphere(wallCheckPoint.position, 0.1f);
        }
        
        if (ledgeCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Vector2 direction = pState.lookingRight ? Vector2.right : Vector2.left;
            Gizmos.DrawRay(ledgeCheckPoint.position, direction * ledgeCheckDistance);
            Gizmos.DrawWireSphere(ledgeCheckPoint.position, 0.1f);
        }
    }
}
