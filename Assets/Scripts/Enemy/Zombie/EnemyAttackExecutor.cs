using UnityEngine;

public class EnemyAttackExecutor : MonoBehaviour
{
    private static readonly int AttackAnim = Animator.StringToHash("Attack");
    public bool IsAttacking { get; private set; }

    [SerializeField] Animator animator;
    [SerializeField] Collider hitBox;

    [SerializeField] float attackCooldown = 1.2f;
    private float lastAttackTime;
    private Rigidbody rb;
    
    private void Awake()
    {
        hitBox.enabled = false;
        rb = GetComponent<Rigidbody>();
    }

    public bool CanAttack()
    {
        return !IsAttacking && Time.time >= lastAttackTime + attackCooldown;
    }

    public void StartAttack()
    {
        if (!CanAttack()) return;
        EnableHitbox();
        
        rb.angularVelocity = Vector3.zero;
        rb.freezeRotation = true;

        IsAttacking = true;
        lastAttackTime = Time.time;

        animator.SetBool(AttackAnim, true);
    }

    public void EndAttack()
    {
        rb.freezeRotation = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ;
        IsAttacking = false;
        animator.SetBool(AttackAnim, false);
        DisableHitbox();
    }

    public void EnableHitbox()
    {
        hitBox.enabled = true;
    }

    public void DisableHitbox()
    {
        hitBox.enabled = false;
    }
}