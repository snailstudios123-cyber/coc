using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowSpell : MonoBehaviour
{
    [Header("Snowfall Settings")]
    [SerializeField] private float damage = 5f;
    [SerializeField] private float freezeDuration = 3f;
    [SerializeField] private float snowfallRadius = 5f;
    [SerializeField] private float snowfallDuration = 5f;
    [SerializeField] private float spawnHeight = 8f;
    
    [Header("Snowflake Settings")]
    [SerializeField] private GameObject snowflakePrefab;
    [SerializeField] private int snowflakesPerSecond = 20;
    [SerializeField] private float snowflakeFallSpeed = 2f;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem snowParticles;
    [SerializeField] private bool createParticlesIfMissing = true;
    
    private Vector3 spawnCenter;
    private float timer = 0f;
    private float spawnTimer = 0f;
    private List<GameObject> activeSnowflakes = new List<GameObject>();
    private bool isActive = true;

    private void Start()
    {
        spawnCenter = transform.position;
        
        if (snowParticles == null && createParticlesIfMissing)
        {
            CreateSnowParticleSystem();
        }
        
        if (snowflakePrefab == null)
        {
            CreateDefaultSnowflake();
        }
        
        StartCoroutine(SnowfallCoroutine());
    }

    private void Update()
    {
        if (!isActive) return;
        
        timer += Time.deltaTime;
        spawnTimer += Time.deltaTime;
        
        float spawnInterval = 1f / snowflakesPerSecond;
        if (spawnTimer >= spawnInterval && timer < snowfallDuration)
        {
            SpawnSnowflake();
            spawnTimer = 0f;
        }
        
        if (timer >= snowfallDuration)
        {
            isActive = false;
            Destroy(gameObject, 3f);
        }
    }

    private void SpawnSnowflake()
    {
        float randomX = Random.Range(-snowfallRadius, snowfallRadius);
        Vector3 spawnPosition = new Vector3(
            spawnCenter.x + randomX, 
            spawnCenter.y + spawnHeight, 
            spawnCenter.z
        );
        
        GameObject snowflake = Instantiate(snowflakePrefab, spawnPosition, Quaternion.identity, transform);
        Snowflake snowflakeScript = snowflake.GetComponent<Snowflake>();
        
        if (snowflakeScript != null)
        {
            snowflakeScript.Initialize(snowflakeFallSpeed, damage, freezeDuration);
        }
        
        activeSnowflakes.Add(snowflake);
        Destroy(snowflake, 10f);
    }

    private IEnumerator SnowfallCoroutine()
    {
        yield return new WaitForSeconds(snowfallDuration);
        
        if (snowParticles != null)
        {
            var emission = snowParticles.emission;
            emission.enabled = false;
        }
    }

    private void CreateSnowParticleSystem()
    {
        GameObject particleObj = new GameObject("SnowParticles");
        particleObj.transform.SetParent(transform);
        particleObj.transform.localPosition = new Vector3(0, spawnHeight, 0);
        
        ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();
        
        var main = ps.main;
        main.startLifetime = 5f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 3f);
        main.maxParticles = 200;
        main.startColor = new Color(1f, 1f, 1f, 0.8f);
        main.gravityModifier = 0.2f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        
        var emission = ps.emission;
        emission.rateOverTime = snowflakesPerSecond * 2;
        
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(snowfallRadius * 2f, 0.1f, 1f);
        
        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);
        
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(
            new Keyframe(0, 1),
            new Keyframe(1, 0.5f)
        ));
        
        ParticleSystemRenderer renderer = particleObj.GetComponent<ParticleSystemRenderer>();
        renderer.sortingLayerName = "Default";
        renderer.sortingOrder = 10;
        
        snowParticles = ps;
    }

    private void CreateDefaultSnowflake()
    {
        snowflakePrefab = new GameObject("SnowflakeTemplate");
        
        SpriteRenderer sr = snowflakePrefab.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.9f, 0.95f, 1f, 0.9f);
        sr.sortingOrder = 9;
        
        CircleCollider2D collider = snowflakePrefab.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.15f;
        
        Rigidbody2D rb = snowflakePrefab.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.isKinematic = true;
        
        snowflakePrefab.AddComponent<Snowflake>();
        
        snowflakePrefab.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0.8f, 1f, 0.2f);
        Vector3 center = Application.isPlaying ? spawnCenter : transform.position;
        
        Gizmos.DrawWireCube(
            new Vector3(center.x, center.y + spawnHeight / 2f, center.z),
            new Vector3(snowfallRadius * 2f, spawnHeight, 1f)
        );
        
        Gizmos.color = new Color(0.3f, 0.6f, 1f, 0.5f);
        Gizmos.DrawLine(
            new Vector3(center.x - snowfallRadius, center.y + spawnHeight, center.z),
            new Vector3(center.x + snowfallRadius, center.y + spawnHeight, center.z)
        );
    }
}
