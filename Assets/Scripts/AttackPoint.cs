using UnityEngine;

public class EnemyAttackWarning : MonoBehaviour
{
    private Enemy ownerEnemy;
    private float attackWarningTime;
    private bool isInAttackWarning = false;
    
    private void Start()
    {
        ownerEnemy = GetComponentInParent<Enemy>();
    }
    
    public void StartAttackWarning(float warningDuration)
    {
        isInAttackWarning = true;
        attackWarningTime = Time.time + warningDuration;
    }
    
    public void EndAttackWarning()
    {
        isInAttackWarning = false;
    }
    
    public bool IsInParryWindow()
    {
        if (!isInAttackWarning) return false;
        
        if (ParrySystem.Instance == null) return false;
        
        float timeUntilAttack = attackWarningTime - Time.time;
        return timeUntilAttack <= ParrySystem.Instance.GetParryWindow() && timeUntilAttack > 0;
    }
    
    public Enemy GetOwnerEnemy()
    {
        return ownerEnemy;
    }
}
