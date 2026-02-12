using System;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    [SerializeField] int damageAmount = 10;
    [SerializeField] EnemyAttackExecutor attackExecutor;

    private void OnTriggerEnter(Collider other)
    {
        if(!attackExecutor.IsAttacking) return;
        
        print("Attacked for " + damageAmount);

        if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.Damage(damageAmount);
        }
    }
}
