using UnityEngine;

public class TornadoController : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float scaleMultiplier = 1f;
    [SerializeField] private bool enableRotation = true;
    
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 5;
        }
    }

    private void Update()
    {
        if (enableRotation)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }

    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale * scaleMultiplier;
    }

    public void SetRotationEnabled(bool enabled)
    {
        enableRotation = enabled;
    }
}
