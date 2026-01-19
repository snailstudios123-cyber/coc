using UnityEngine;

public class PhysicsDebugger : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D col;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        
        Debug.Log($"=== PHYSICS DEBUGGER FOR {gameObject.name} ===");
        Debug.Log($"Layer: {LayerMask.LayerToName(gameObject.layer)} (Index: {gameObject.layer})");
        Debug.Log($"Time.timeScale: {Time.timeScale}, Time.fixedDeltaTime: {Time.fixedDeltaTime}");
        
        if (rb != null)
        {
            Debug.Log($"Rigidbody2D: {rb.bodyType}, Mass: {rb.mass}, Gravity: {rb.gravityScale}");
            Debug.Log($"Collision Detection: {rb.collisionDetectionMode}, Simulated: {rb.simulated}");
            Debug.Log($"Sleeping: {rb.IsSleeping()}, Constraints: {rb.constraints}");
        }
        
        if (col != null)
        {
            Debug.Log($"BoxCollider2D: Enabled={col.enabled}, IsTrigger={col.isTrigger}, Size={col.size}");
        }
        
        int playerLayer = LayerMask.NameToLayer("Player");
        int groundLayer = LayerMask.NameToLayer("Ground");
        bool canCollide = !Physics2D.GetIgnoreLayerCollision(playerLayer, groundLayer);
        Debug.Log($"Can Player-Ground collide? {canCollide}");
        Debug.Log("===========================================");
    }
    
    void FixedUpdate()
    {
        if (rb != null && gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log($"Player Velocity: {rb.velocity}, Position: {transform.position}, Simulated: {rb.simulated}, BodyType: {rb.bodyType}, GravityScale: {rb.gravityScale}");
            
            if (rb.velocity.magnitude < 0.1f && transform.position.y > 20f)
            {
                Debug.LogWarning($"Player is frozen in midair! Position: {transform.position}");
            }
        }
    }
}
