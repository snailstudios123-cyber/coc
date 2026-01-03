using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeEffect : MonoBehaviour
{
    [Header("Freeze Settings")]
    [SerializeField] private Color frozenColor = new Color(0.5f, 0.8f, 1f, 1f);
    [SerializeField] private bool showFreezeParticles = true;
    
    private MonoBehaviour[] allScripts;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D[] colliders;
    
    private Color originalColor;
    private float originalAnimatorSpeed;
    private bool originalKinematicState;
    private Vector2 originalVelocity;
    private int originalLayer;
    private List<bool> originalScriptStates = new List<bool>();
    private List<bool> originalColliderStates = new List<bool>();
    
    private bool isFrozen = false;
    private Coroutine freezeCoroutine;
    private GameObject freezeParticles;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colliders = GetComponentsInChildren<Collider2D>();
        allScripts = GetComponents<MonoBehaviour>();
        originalLayer = gameObject.layer;
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void ApplyFreeze(float duration)
    {
        if (freezeCoroutine != null)
        {
            StopCoroutine(freezeCoroutine);
        }
        
        freezeCoroutine = StartCoroutine(FreezeCoroutine(duration));
    }

    private IEnumerator FreezeCoroutine(float duration)
    {
        Freeze();
        
        yield return new WaitForSeconds(duration);
        
        Unfreeze();
    }

    private void Freeze()
    {
        if (isFrozen) return;
        
        isFrozen = true;

        if (rb != null)
        {
            originalVelocity = rb.velocity;
            originalKinematicState = rb.isKinematic;
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        if (animator != null)
        {
            originalAnimatorSpeed = animator.speed;
            animator.speed = 0f;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = frozenColor;
        }

        originalScriptStates.Clear();
        foreach (MonoBehaviour script in allScripts)
        {
            if (script != null && script != this)
            {
                originalScriptStates.Add(script.enabled);
                script.enabled = false;
            }
            else
            {
                originalScriptStates.Add(false);
            }
        }

        originalColliderStates.Clear();
        foreach (Collider2D col in colliders)
        {
            if (col != null)
            {
                originalColliderStates.Add(col.enabled);
                
                if (col.gameObject != gameObject)
                {
                    col.enabled = false;
                }
            }
            else
            {
                originalColliderStates.Add(false);
            }
        }

        if (showFreezeParticles)
        {
            CreateFreezeParticles();
        }
    }

    private void Unfreeze()
    {
        if (!isFrozen) return;
        
        isFrozen = false;

        if (rb != null)
        {
            rb.isKinematic = originalKinematicState;
            rb.velocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.speed = originalAnimatorSpeed;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        gameObject.layer = originalLayer;

        for (int i = 0; i < allScripts.Length && i < originalScriptStates.Count; i++)
        {
            if (allScripts[i] != null && allScripts[i] != this && originalScriptStates[i])
            {
                allScripts[i].enabled = true;
            }
        }
        originalScriptStates.Clear();

        for (int i = 0; i < colliders.Length && i < originalColliderStates.Count; i++)
        {
            if (colliders[i] != null)
            {
                colliders[i].enabled = originalColliderStates[i];
            }
        }
        originalColliderStates.Clear();

        if (freezeParticles != null)
        {
            Destroy(freezeParticles);
        }
    }

    private void CreateFreezeParticles()
    {
        GameObject particleObj = new GameObject("FreezeParticles");
        particleObj.transform.SetParent(transform);
        particleObj.transform.localPosition = Vector3.zero;
        
        ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();
        
        var main = ps.main;
        main.startLifetime = 0.5f;
        main.startSize = 0.2f;
        main.startSpeed = 1f;
        main.maxParticles = 50;
        main.loop = true;
        main.startColor = new Color(0.8f, 0.9f, 1f, 0.8f);
        
        var emission = ps.emission;
        emission.rateOverTime = 20f;
        
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.5f;
        
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(Color.white, 0.0f), 
                new GradientColorKey(Color.cyan, 1.0f) 
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1.0f, 0.0f), 
                new GradientAlphaKey(0.0f, 1.0f) 
            }
        );
        colorOverLifetime.color = gradient;
        
        ParticleSystemRenderer renderer = particleObj.GetComponent<ParticleSystemRenderer>();
        renderer.sortingLayerName = "Default";
        renderer.sortingOrder = 10;
        
        freezeParticles = particleObj;
    }

    public bool IsFrozen()
    {
        return isFrozen;
    }

    private void OnDestroy()
    {
        if (freezeCoroutine != null)
        {
            StopCoroutine(freezeCoroutine);
        }
        
        if (isFrozen)
        {
            Unfreeze();
        }
    }
}
