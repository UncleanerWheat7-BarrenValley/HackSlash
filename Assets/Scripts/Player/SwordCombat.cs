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
    public float hightimeImpulseForce;

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
                    StartAttack("Stinger", 0);
                }
                else if (localZ < -0.5f)
                {
                    StartAttack("HighTime", hightimeImpulseForce);
                }
                else
                {
                    StartAttack("Attack", 0);
                }
            }
            else
            {
                StartAttack("Attack", 0);
            }
        }
    }

    void StartAttack(string animationName, float impulseForce)
    {
        print("Anim weight 1 " + animator.GetLayerWeight(upperBodyLayer));
        animator.SetLayerWeight(upperBodyLayer, 0);
        print("Anim weight 2 " +animator.GetLayerWeight(upperBodyLayer));
        lockOnSystem.SoftTarget();
        animator.SetTrigger(animationName);
        isAttacking = true;
        isPlanted = true;
        weaponHitBox.impulseForce = impulseForce;
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