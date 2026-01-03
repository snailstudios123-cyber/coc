using UnityEngine;

public class BeamVisualEffect : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Sprite[] beamFrames;
    [SerializeField] private float frameRate = 12f;
    
    [Header("Visual Settings")]
    [SerializeField] private float beamLength = 10f;
    [SerializeField] private Color beamColor = Color.cyan;
    [SerializeField] private float fadeOutDuration = 0.2f;
    
    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;
    private float frameTimer = 0f;
    private float lifetimeTimer = 0f;
    private bool isFadingOut = false;
    private float fadeTimer = 0f;
    private Color originalColor;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        if (beamFrames != null && beamFrames.Length > 0)
        {
            Sprite firstValidSprite = null;
            foreach (Sprite sprite in beamFrames)
            {
                if (sprite != null)
                {
                    firstValidSprite = sprite;
                    break;
                }
            }
            
            if (firstValidSprite != null)
            {
                spriteRenderer.sprite = firstValidSprite;
            }
            else
            {
                Debug.LogWarning("BeamVisualEffect: No valid sprites found in beamFrames array!");
            }
        }
        
        spriteRenderer.color = beamColor;
        originalColor = beamColor;
        
        Vector3 scale = transform.localScale;
        scale.x = beamLength;
        transform.localScale = scale;
    }
    
    private void Update()
    {
        if (beamFrames == null || beamFrames.Length == 0)
            return;
        
        frameTimer += Time.deltaTime;
        
        if (frameTimer >= 1f / frameRate)
        {
            frameTimer = 0f;
            currentFrame = (currentFrame + 1) % beamFrames.Length;
            
            if (beamFrames[currentFrame] != null)
            {
                spriteRenderer.sprite = beamFrames[currentFrame];
            }
        }
        
        if (isFadingOut)
        {
            fadeTimer += Time.deltaTime;
            float alpha = 1f - (fadeTimer / fadeOutDuration);
            Color newColor = originalColor;
            newColor.a = alpha;
            spriteRenderer.color = newColor;
            
            if (fadeTimer >= fadeOutDuration)
            {
                Destroy(gameObject);
            }
        }
    }
    
    public void SetBeamLength(float length)
    {
        beamLength = length;
        Vector3 scale = transform.localScale;
        scale.x = beamLength;
        transform.localScale = scale;
    }
    
    public void SetBeamColor(Color color)
    {
        beamColor = color;
        originalColor = color;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }
    
    public void StartFadeOut()
    {
        isFadingOut = true;
        fadeTimer = 0f;
    }
}
