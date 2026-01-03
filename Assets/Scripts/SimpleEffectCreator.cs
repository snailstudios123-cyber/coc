using UnityEngine;

public class SimpleEffectCreator : MonoBehaviour
{
    public static GameObject CreateSimpleHitEffect(Vector3 position, Color color, float size = 0.5f, float lifetime = 0.5f)
    {
        GameObject effectObj = new GameObject("HitEffect");
        effectObj.transform.position = position;

        ParticleSystem ps = effectObj.AddComponent<ParticleSystem>();
        
        var main = ps.main;
        main.startColor = color;
        main.startSize = size;
        main.startLifetime = lifetime;
        main.startSpeed = 5f;
        main.maxParticles = 20;
        main.duration = lifetime;
        
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 15) });
        
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;
        
        var renderer = effectObj.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.sortingOrder = 100;
        
        GameObject.Destroy(effectObj, lifetime + 0.5f);
        
        return effectObj;
    }

    public static GameObject CreateImpactRing(Vector3 position, Color color, float maxSize = 2f, float lifetime = 0.3f)
    {
        GameObject ringObj = new GameObject("ImpactRing");
        ringObj.transform.position = position;
        
        LineRenderer line = ringObj.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.loop = true;
        line.widthMultiplier = 0.1f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = color;
        line.endColor = new Color(color.r, color.g, color.b, 0f);
        line.sortingOrder = 99;
        
        int segments = 32;
        line.positionCount = segments;
        
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            float x = Mathf.Cos(angle) * 0.1f;
            float y = Mathf.Sin(angle) * 0.1f;
            line.SetPosition(i, new Vector3(x, y, 0f));
        }
        
        SimpleRingAnimator animator = ringObj.AddComponent<SimpleRingAnimator>();
        animator.targetSize = maxSize;
        animator.lifetime = lifetime;
        
        return ringObj;
    }

    public static GameObject CreateBloodSpatter(Vector3 position, Vector2 direction, int dropletCount = 5)
    {
        GameObject spatterParent = new GameObject("BloodSpatter");
        spatterParent.transform.position = position;
        
        for (int i = 0; i < dropletCount; i++)
        {
            GameObject droplet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            droplet.transform.parent = spatterParent.transform;
            droplet.transform.position = position;
            droplet.transform.localScale = Vector3.one * Random.Range(0.05f, 0.15f);
            
            SpriteRenderer sr = droplet.AddComponent<SpriteRenderer>();
            sr.color = new Color(0.6f, 0f, 0f);
            sr.sortingOrder = 98;
            
            GameObject.Destroy(droplet.GetComponent<MeshRenderer>());
            GameObject.Destroy(droplet.GetComponent<Collider>());
            
            Rigidbody2D rb = droplet.AddComponent<Rigidbody2D>();
            rb.gravityScale = 2f;
            
            Vector2 randomDir = direction + Random.insideUnitCircle * 0.5f;
            rb.AddForce(randomDir * Random.Range(2f, 5f), ForceMode2D.Impulse);
        }
        
        GameObject.Destroy(spatterParent, 3f);
        
        return spatterParent;
    }

    public static GameObject CreateSparks(Vector3 position, Vector2 direction, Color sparkColor, int sparkCount = 8)
    {
        GameObject sparkParent = new GameObject("Sparks");
        sparkParent.transform.position = position;
        
        for (int i = 0; i < sparkCount; i++)
        {
            GameObject spark = new GameObject("Spark");
            spark.transform.parent = sparkParent.transform;
            spark.transform.position = position;
            
            LineRenderer line = spark.AddComponent<LineRenderer>();
            line.positionCount = 2;
            line.startWidth = 0.05f;
            line.endWidth = 0.01f;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = sparkColor;
            line.endColor = new Color(sparkColor.r, sparkColor.g, sparkColor.b, 0f);
            line.sortingOrder = 100;
            
            Vector2 sparkDir = Quaternion.Euler(0, 0, Random.Range(-45f, 45f)) * direction;
            
            line.SetPosition(0, Vector3.zero);
            line.SetPosition(1, sparkDir * Random.Range(0.2f, 0.5f));
            
            Rigidbody2D rb = spark.AddComponent<Rigidbody2D>();
            rb.gravityScale = 1f;
            rb.AddForce(sparkDir * Random.Range(3f, 6f), ForceMode2D.Impulse);
        }
        
        GameObject.Destroy(sparkParent, 1f);
        
        return sparkParent;
    }

    public static GameObject CreateFlashEffect(Vector3 position, Color flashColor, float size = 1f, float duration = 0.1f)
    {
        GameObject flashObj = new GameObject("FlashEffect");
        flashObj.transform.position = position;
        
        SpriteRenderer sr = flashObj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite(64);
        sr.color = flashColor;
        sr.sortingOrder = 101;
        
        flashObj.transform.localScale = Vector3.one * size;
        
        SimpleFadeOut fade = flashObj.AddComponent<SimpleFadeOut>();
        fade.duration = duration;
        
        return flashObj;
    }

    private static Sprite CreateCircleSprite(int resolution)
    {
        int diameter = resolution;
        Texture2D texture = new Texture2D(diameter, diameter);
        Color[] pixels = new Color[diameter * diameter];
        
        Vector2 center = new Vector2(diameter / 2f, diameter / 2f);
        float radius = diameter / 2f;
        
        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);
                
                if (distance <= radius)
                {
                    float alpha = 1f - (distance / radius);
                    pixels[y * diameter + x] = new Color(1f, 1f, 1f, alpha);
                }
                else
                {
                    pixels[y * diameter + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, diameter, diameter), Vector2.one * 0.5f);
    }
}

public class SimpleRingAnimator : MonoBehaviour
{
    public float targetSize = 2f;
    public float lifetime = 0.3f;
    
    private float elapsed = 0f;
    private LineRenderer lineRenderer;
    private int segments;
    
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        segments = lineRenderer.positionCount;
    }
    
    private void Update()
    {
        elapsed += Time.deltaTime;
        float t = elapsed / lifetime;
        
        float currentSize = Mathf.Lerp(0.1f, targetSize, t);
        
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            float x = Mathf.Cos(angle) * currentSize;
            float y = Mathf.Sin(angle) * currentSize;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
        
        Color startColor = lineRenderer.startColor;
        Color endColor = lineRenderer.endColor;
        lineRenderer.startColor = new Color(startColor.r, startColor.g, startColor.b, 1f - t);
        lineRenderer.endColor = new Color(endColor.r, endColor.g, endColor.b, 0f);
        
        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}

public class SimpleFadeOut : MonoBehaviour
{
    public float duration = 0.1f;
    private float elapsed = 0f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
    
    private void Update()
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;
        
        if (spriteRenderer != null)
        {
            Color newColor = originalColor;
            newColor.a = 1f - t;
            spriteRenderer.color = newColor;
        }
        
        if (elapsed >= duration)
        {
            Destroy(gameObject);
        }
    }
}
