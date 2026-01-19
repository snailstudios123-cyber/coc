using UnityEngine;
using UnityEditor;

public class PhysicsLayerDiagnostic : EditorWindow
{
    [MenuItem("Tools/Physics Layer Diagnostic")]
    public static void ShowDiagnostic()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int groundLayer = LayerMask.NameToLayer("Ground");
        
        bool canCollide = !Physics2D.GetIgnoreLayerCollision(playerLayer, groundLayer);
        
        string message = $"Player Layer Index: {playerLayer}\n" +
                        $"Ground Layer Index: {groundLayer}\n" +
                        $"Can Player and Ground collide? {canCollide}\n\n";
        
        if (!canCollide)
        {
            message += "PROBLEM FOUND: Player and Ground layers are set to IGNORE each other!\n\n";
            message += "Click 'Fix Now' to enable collision between Player and Ground layers.";
            
            if (EditorUtility.DisplayDialog("Physics Layer Collision Issue", message, "Fix Now", "Cancel"))
            {
                Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, false);
                Debug.Log("✓ Fixed: Player and Ground layers can now collide!");
                EditorUtility.DisplayDialog("Fixed!", "Player and Ground layers can now collide. Test in Play Mode.", "OK");
            }
        }
        else
        {
            message += "✓ Player and Ground layers are configured to collide correctly.";
            EditorUtility.DisplayDialog("Physics Layer Diagnostic", message, "OK");
        }
    }
}
