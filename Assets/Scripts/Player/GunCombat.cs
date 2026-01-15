using UnityEngine;

public class GunCombat : MonoBehaviour
{
    private bool gunsActive = false;
    bool UseLeftGun = true;
    
    public Gun leftGun;
    public Gun rightGun;
    public PlayerInputHandler input;
    public LockOnSystem lockOnSystem;
    public Animator animator;

    public void HandleGunAttacks()
    {
        bool shouldGunsBeActive = lockOnSystem.LockMode || input.GunPressed ;

        if (shouldGunsBeActive != gunsActive)
        {
            gunsActive = shouldGunsBeActive;
            leftGun.gameObject.SetActive(gunsActive);
            rightGun.gameObject.SetActive(gunsActive);
        }

        if (input.GunPressed && gunsActive)
        {
            if (lockOnSystem.LockMode == false)
            {
                CheckSoftTarget();
            }

            Gun gunToFire = UseLeftGun ? leftGun : rightGun;

            if (UseLeftGun)
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
    public void CheckSoftTarget()
    {
        if (lockOnSystem.LockMode) return;
        lockOnSystem.SoftTarget();
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
