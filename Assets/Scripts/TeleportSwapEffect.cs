using UnityEngine;

public class TeleportSwapEffect : MonoBehaviour
{
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float scaleSpeed = 3f;
    [SerializeField] private float maxScale = 2f;
    
    private SpriteRenderer spriteRenderer;
    private float timer;
    private Vector3 initialScale;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            initialScale = transform.localScale;
        }
    }
    
    private void Update()
    {
        timer += Time.deltaTime;
        
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(1f, 0f, timer / lifetime);
            spriteRenderer.color = color;
            
            float scale = Mathf.Lerp(1f, maxScale, timer / lifetime);
            transform.localScale = initialScale * scale;
        }
        
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
