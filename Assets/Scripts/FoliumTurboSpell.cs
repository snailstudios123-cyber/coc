using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoliumTurboSpell : MonoBehaviour
{
    [Header("Spell Data")]
    [SerializeField] private SpellData spellData;
    
    [Header("Tornado Settings")]
    [SerializeField] private GameObject tornadoPrefab;
    [SerializeField] private float offensiveDuration = 3f;
    [SerializeField] private float pushForce = 15f;
    [SerializeField] private float seekSpeed = 8f;
    [SerializeField] private float swirlRadius = 1.5f;
    [SerializeField] private float swirlSpeed = 720f;
    
    [Header("Damage Settings")]
    [SerializeField] private float damagePerSecond = 5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float detectionRadius = 2f;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip tornadoSound;
    
    private Transform playerTransform;
    private bool isOffensiveActive = false;
    private bool isDefensiveActive = false;
    private GameObject defensiveTornado;
    private Coroutine offensiveCoroutine;
    private List<Transform> capturedEnemies = new List<Transform>();
    private bool pendingDefensiveCast = false;
    private bool pendingOffensiveCast = false;
    private Transform pendingOffensiveTarget;

    private void Start()
    {
        playerTransform = PlayerController.Instance.transform;
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    private void Update()
    {
        if (isDefensiveActive && defensiveTornado != null && playerTransform != null)
        {
            UpdateDefensiveTornado();
        }
    }

    public void StartDefensiveTornado()
    {
        if (spellData != null && !spellData.isEquipped)
        {
            return;
        }
        
        if (isDefensiveActive || isOffensiveActive) return;
        
        pendingDefensiveCast = true;
        
        Animator anim = PlayerController.Instance.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("CastInstant");
        }
    }

    public void StopDefensiveTornado()
    {
        if (!isDefensiveActive) return;
        
        isDefensiveActive = false;
        
        if (defensiveTornado != null)
        {
            Destroy(defensiveTornado);
            defensiveTornado = null;
        }
        
        if (audioSource != null)
        {
            audioSource.loop = false;
            audioSource.Stop();
        }
    }

    private void UpdateDefensiveTornado()
    {
        defensiveTornado.transform.position = playerTransform.position;
        
        PushEnemiesAway(defensiveTornado.transform.position);
        DamageNearbyEnemies(defensiveTornado.transform.position);
    }

    public void CastOffensiveTornado()
    {
        if (spellData != null && !spellData.isEquipped)
        {
            return;
        }
        
        if (isOffensiveActive || isDefensiveActive) return;
        
        Transform nearestEnemy = FindNearestEnemy();
        pendingOffensiveCast = true;
        pendingOffensiveTarget = nearestEnemy;
        
        Animator anim = PlayerController.Instance.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("CastInstant");
        }
    }

    public bool IsDefensiveActive()
    {
        return isDefensiveActive;
    }

    public bool IsAnySpellActive()
    {
        return isDefensiveActive || isOffensiveActive;
    }

    public void OnInstantSpellCast()
    {
        if (pendingDefensiveCast)
        {
            pendingDefensiveCast = false;
            ActuallyStartDefensiveTornado();
        }
        else if (pendingOffensiveCast)
        {
            pendingOffensiveCast = false;
            ActuallyStartOffensiveTornado(pendingOffensiveTarget);
            pendingOffensiveTarget = null;
        }
    }

    private void ActuallyStartDefensiveTornado()
    {
        isDefensiveActive = true;
        defensiveTornado = Instantiate(tornadoPrefab, playerTransform.position, Quaternion.identity);
        
        TornadoController tornadoController = defensiveTornado.GetComponent<TornadoController>();
        if (tornadoController != null)
        {
            tornadoController.SetRotationEnabled(false);
        }
        
        if (audioSource != null && tornadoSound != null)
        {
            audioSource.clip = tornadoSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void ActuallyStartOffensiveTornado(Transform targetEnemy)
    {
        if (offensiveCoroutine != null)
        {
            StopCoroutine(offensiveCoroutine);
        }
        
        offensiveCoroutine = StartCoroutine(OffensiveTornadoCoroutine(targetEnemy));
    }

    private Transform FindNearestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(playerTransform.position, 20f, enemyLayer);
        
        Transform nearest = null;
        float minDistance = float.MaxValue;
        
        foreach (Collider2D enemy in enemies)
        {
            float distance = Vector2.Distance(playerTransform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy.transform;
            }
        }
        
        return nearest;
    }

    private IEnumerator OffensiveTornadoCoroutine(Transform targetEnemy)
    {
        isOffensiveActive = true;
        capturedEnemies.Clear();
        
        GameObject tornado = Instantiate(tornadoPrefab, playerTransform.position, Quaternion.identity);
        
        TornadoController tornadoController = tornado.GetComponent<TornadoController>();
        if (tornadoController != null)
        {
            tornadoController.SetRotationEnabled(false);
        }
        
        if (audioSource != null && tornadoSound != null)
        {
            audioSource.PlayOneShot(tornadoSound);
        }
        
        float elapsedTime = 0f;
        Vector3 tornadoPosition = playerTransform.position;
        
        if (targetEnemy != null)
        {
            while (elapsedTime < offensiveDuration && tornado != null)
            {
                if (targetEnemy != null)
                {
                    Vector3 direction = (targetEnemy.position - tornado.transform.position).normalized;
                    tornadoPosition = Vector3.MoveTowards(tornado.transform.position, targetEnemy.position, seekSpeed * Time.deltaTime);
                    tornado.transform.position = tornadoPosition;
                    
                    float distanceToTarget = Vector2.Distance(tornado.transform.position, targetEnemy.position);
                    if (distanceToTarget < swirlRadius)
                    {
                        CaptureAndSwirlEnemies(tornado.transform.position);
                    }
                }
                
                DamageNearbyEnemies(tornado.transform.position);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            
            while (elapsedTime < offensiveDuration && tornado != null)
            {
                Vector3 direction = (mousePos - tornado.transform.position).normalized;
                tornadoPosition = Vector3.MoveTowards(tornado.transform.position, mousePos, seekSpeed * Time.deltaTime);
                tornado.transform.position = tornadoPosition;
                
                CaptureAndSwirlEnemies(tornado.transform.position);
                DamageNearbyEnemies(tornado.transform.position);
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        
        ReleaseAllCapturedEnemies();
        
        if (tornado != null)
        {
            Destroy(tornado);
        }
        
        isOffensiveActive = false;
        offensiveCoroutine = null;
    }

    private void PushEnemiesAway(Vector3 tornadoPosition)
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(tornadoPosition, detectionRadius * 1.5f, enemyLayer);
        
        foreach (Collider2D enemy in enemies)
        {
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 pushDirection = (enemy.transform.position - tornadoPosition).normalized;
                enemyRb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
            }
        }
    }

    private void CaptureAndSwirlEnemies(Vector3 tornadoPosition)
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(tornadoPosition, swirlRadius, enemyLayer);
        
        foreach (Collider2D enemy in enemies)
        {
            if (!capturedEnemies.Contains(enemy.transform))
            {
                capturedEnemies.Add(enemy.transform);
                
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    enemyRb.gravityScale = 0;
                }
            }
        }
        
        for (int i = capturedEnemies.Count - 1; i >= 0; i--)
        {
            if (capturedEnemies[i] == null)
            {
                capturedEnemies.RemoveAt(i);
                continue;
            }
            
            float angle = (Time.time * swirlSpeed + (i * 360f / Mathf.Max(1, capturedEnemies.Count))) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * swirlRadius;
            Vector3 targetPosition = tornadoPosition + offset;
            
            Rigidbody2D enemyRb = capturedEnemies[i].GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 direction = (targetPosition - capturedEnemies[i].position).normalized;
                enemyRb.velocity = direction * seekSpeed;
            }
        }
    }

    private void ReleaseAllCapturedEnemies()
    {
        foreach (Transform enemy in capturedEnemies)
        {
            if (enemy != null)
            {
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    enemyRb.gravityScale = 1;
                    enemyRb.velocity = Vector2.zero;
                }
            }
        }
        capturedEnemies.Clear();
    }

    private void DamageNearbyEnemies(Vector3 tornadoPosition)
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(tornadoPosition, detectionRadius, enemyLayer);
        
        foreach (Collider2D enemy in enemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.EnemyHit(damagePerSecond * Time.deltaTime, Vector2.zero, 0);
            }
        }
    }

    private void OnDestroy()
    {
        StopDefensiveTornado();
        ReleaseAllCapturedEnemies();
        
        if (offensiveCoroutine != null)
        {
            StopCoroutine(offensiveCoroutine);
        }
    }
}
