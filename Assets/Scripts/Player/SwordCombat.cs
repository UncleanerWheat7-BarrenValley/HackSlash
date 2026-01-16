using UnityEngine;

public class SwordCombat : MonoBehaviour
{
    private static readonly int StingerEnd = Animator.StringToHash("StingerEnd");
    public PlayerInputHandler input;
    private SwordCombat swordCombat;
    public Animator animator;
    public CharacterController characterController;
    public LocalForwardHelper forwardHelper;
    public bool isAttacking;
    public bool isSecondaryAttack;
    public bool isPlanted;
    public bool useStingerForce = false;
    
    public float stingerForce = 10;
    public LockOnSystem lockOnSystem;
    public bool IsAttacking => isAttacking;
    public bool IsSecondaryAttack => isSecondaryAttack;
    public WeaponHitBox weaponHitBox;
    
    int upperBodyLayer = 1;
    public int hightimeImpulseForce;
    
    public void HandleAttack()
    {
        if (useStingerForce)
        {
            HandleStinger();
            return;
        }

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
                    weaponHitBox.impulseForce = hightimeImpulseForce;
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
                lockOnSystem.SoftTarget();
                animator.SetTrigger("Attack");
                isAttacking = true;
                isPlanted = true;
            }
        }
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
    }

    void LaunchEnemy(Rigidbody enemyRigidBody)
    {
        enemyRigidBody.AddForce(Vector3.up * 100, ForceMode.Impulse);
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
