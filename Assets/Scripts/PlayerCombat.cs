using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public PlayerInputHandler input;
    private Animator animator;
    public bool isAttacking;
    public bool isSecondaryAttack;
    public bool isPlanted;

    public bool IsAttacking => isAttacking;
    public bool IsSecondaryAttack => isSecondaryAttack;
    public bool IsPlanted => isPlanted;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleAttack();
    }

    private void HandleAttack()
    {
        // prevent retriggering while mid-animation
        if (isAttacking) return;

        if (input.AttackPressed)
        {
            animator.SetTrigger("Attack");
            isAttacking = true;
            isPlanted = true;
        }
    }

    // Called from animation event near the end of Attack1
    public void ResetAttack()
    {
        isAttacking = false;
    }

    // Called from animation event near the end of Attack1
    public void SetAttackBTrue()
    {
        if (!IsAttacking)
            isSecondaryAttack = true;
            animator.SetBool("AttackB", isSecondaryAttack);
    }
    public void ResetAttackB()
    {
        isSecondaryAttack = false;
        animator.SetBool("AttackB", isSecondaryAttack);
    }

    // Called from animation event near the end of Attack1
    public void ResetPlanted()
    {
        isPlanted = false;
    }
}
