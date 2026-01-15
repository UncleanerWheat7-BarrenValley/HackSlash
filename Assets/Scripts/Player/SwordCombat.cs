using UnityEngine;

public class SwordCombat : MonoBehaviour
{
    private static readonly int StingerEnd = Animator.StringToHash("StingerEnd");
    public PlayerInputHandler input;
    private SwordCombat swordCombat;
    public Animator animator;
    private CharacterController characterController;
    public LocalForwardHelper forwardHelper;
    public bool isAttacking;
    public bool isSecondaryAttack;
    public bool isPlanted;
    public bool useStingerForce = false;
    
    public float stingerForce = 10;
    public LockOnSystem lockOnSystem;
    public bool IsAttacking => isAttacking;
    public bool IsSecondaryAttack => isSecondaryAttack;
    
    int upperBodyLayer;
    
    public void HandleAttack()
    {
        // prevent retriggering while mid-animation
        if (isAttacking) return;

        if (input.AttackPressed)
        {
            if (lockOnSystem.LockMode)
            {
                float localZ = forwardHelper.GetLocalMoveZ();
                if (localZ > 0.5f)
                {
                    animator.SetLayerWeight(upperBodyLayer, 0);
                    animator.SetTrigger("Stinger");
                    isPlanted = true;
                }
                else if (localZ < -0.5f)
                {
                    animator.SetLayerWeight(upperBodyLayer, 0);
                    animator.SetTrigger("HighTime");
                    isPlanted = true;
                }
                else
                {
                    animator.SetLayerWeight(upperBodyLayer, 0);
                    animator.SetTrigger("Attack");
                    isAttacking = true;
                    isPlanted = true;
                }
            }
            else
            {
                animator.SetLayerWeight(upperBodyLayer, 0);
                CheckSoftTarget();
                animator.SetTrigger("Attack");
                isAttacking = true;
                isPlanted = true;
            }
        }
    }
    
    public void CheckSoftTarget()
    {
        if (lockOnSystem.LockMode) return;
        lockOnSystem.SoftTarget();
    }
    
    private void HandleStinger()
    {
        if (!useStingerForce) return;
        print("Stinging");
        characterController.Move((transform.forward * (stingerForce * Time.deltaTime)));
    }

    public void EndStringerForce()
    {
        print("Stinging End");
        animator.SetTrigger(StingerEnd);
    }

    private void HandleHighTime()
    {
        print("High Timing");
        RaycastHit objectHit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(transform.position + Vector3.up * 1, fwd * 5, Color.green);
        if (Physics.Raycast(transform.position + Vector3.up * 1, fwd, out objectHit, 50))
        {
            print(objectHit.transform.name);
        }
    }
    
    /////////////////////////////////////////////////////
    /// reset functions
/////////////////////////////////////////////////////
    /// // Called from animation event near the end of Attack1
    public void ResetAttack()
    {
        print("Reset");
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
        lockOnSystem.SoftLockMode = false;
        animator.SetLayerWeight(upperBodyLayer, 1);
    }

    public void SetStingerForceTrue()
    {
        useStingerForce = true;
    }

    public void ResetStingerForce()
    {
        isAttacking = false;
        isPlanted = false;
        useStingerForce = false;
    }
}
