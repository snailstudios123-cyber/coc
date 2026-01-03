using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    [SerializeField] private string targetSceneName;
    [SerializeField] private Vector2 spawnPosition;
    [SerializeField] private bool useSpawnPosition = true;

    [Header("Visual Feedback")]
    [SerializeField] private bool showGizmo = true;
    [SerializeField] private Color gizmoColor = new Color(0f, 1f, 0f, 0.3f);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (SceneTransitionManager.Instance != null)
            {
                if (useSpawnPosition)
                {
                    SceneTransitionManager.Instance.TransitionToScene(targetSceneName, spawnPosition);
                }
                else
                {
                    SceneTransitionManager.Instance.TransitionToScene(targetSceneName);
                }
            }
            else
            {
                Debug.LogError("SceneTransitionManager instance not found! Make sure it exists in the scene.");
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmo) return;

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(boxCollider.offset, boxCollider.size);
        }
    }
}
