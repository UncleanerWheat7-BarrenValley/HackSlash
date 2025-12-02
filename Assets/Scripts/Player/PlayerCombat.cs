using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public PlayerInputHandler input;
    private Animator animator;
    public bool isAttacking;
    public bool isSecondaryAttack;
    public bool isPlanted;
    LockOnSystem lockOnSystem;

    public Gun leftGun;
    public Gun rightGun;    
    bool UseLeftGun = true;

    public bool IsAttacking => isAttacking;
    public bool IsSecondaryAttack => isSecondaryAttack;
    public bool IsPlanted => isPlanted;

    int upperBodyLayer;

    

    private void Awake()
    {
        animator = GetComponent<Animator>();
        lockOnSystem = GetComponent<LockOnSystem>();
        upperBodyLayer = animator.GetLayerIndex("TopLayer");
    }

    private void Update()
    {
        HandleAttack();

        if (isAttacking) return;

        HandleGunAttack();

    }
    private void HandleAttack()
    {
        // prevent retriggering while mid-animation
        if (isAttacking) return;

        if (input.AttackPressed)
        {
            animator.SetLayerWeight(upperBodyLayer, 0);
            CheckSoftTarget();
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
        lockOnSystem.SoftLockMode = false;
        animator.SetLayerWeight(upperBodyLayer, 1);
    }

    public void CheckSoftTarget()
    {
        if (lockOnSystem.LockMode) return;
        lockOnSystem.SoftTarget();
    }


    private void HandleGunAttack()
    {
        if (input.GunPressed)
        {
            if (lockOnSystem.LockMode == false)
            {
                CheckSoftTarget();
            }

            Gun gunToFire = UseLeftGun ? leftGun : rightGun;

            if(UseLeftGun)
            {
                animator.SetTrigger("FireLeft");
            }
            else
                animator.SetTrigger("FireRight");
                
            UseLeftGun = !UseLeftGun;

            



            Vector3 shootDir = GetAimDirection();
            gunToFire.Fire(shootDir);
        }
    }

    private Vector3 GetAimDirection()
    {
        if (lockOnSystem.LockMode && lockOnSystem.currentTarget != null)
        {
            var target = lockOnSystem.currentTarget.Find("TargetPos");
            Vector3 dir = (target.position - lockOnSystem.playerCamTarget.position).normalized;
            return dir;
        }

        return Camera.main.transform.forward;
    }
}
