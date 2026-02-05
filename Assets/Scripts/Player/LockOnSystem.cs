using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class LockOnSystem : MonoBehaviour
{
    private static readonly int Aim = Animator.StringToHash("Aim");
    public PlayerInputHandler input;
    public Transform currentTarget;
    public Transform playerCamTarget;

    public LayerMask enemyMask;
    public float lockOnRange = 20;
    float maxAngle = 60f;

    public CinemachineCamera FreeLookCamera;
    CinemachineOrbitalFollow cinemachineOrbitalFollow;

    public bool LockMode = false;
    public bool SoftLockMode = false;

    public Animator animator;
    public AimIK aimIK;
    int upperBodyLayer = 1;

    private void Start()
    {
        cinemachineOrbitalFollow = FreeLookCamera.GetComponent<CinemachineOrbitalFollow>();
        aimIK = GetComponent<AimIK>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        if (input.LockOnHeld)
        {
            lockOnRange = 15;
            if (!LockMode)
            {
                animator.SetLayerWeight(upperBodyLayer, 1);
                LockMode = true;
                animator.SetBool(Aim, true);
                AcquireTarget();
            }
        }
        else
        {
            if (LockMode)
            {
                animator.SetLayerWeight(upperBodyLayer, 0);
                LockMode = false;
                animator.SetBool(Aim, false);
                ClearTarget();
            }
        }

        if (LockMode)
        {
            if (!TargetIsValid())
            {
                Transform oldTarget = currentTarget;
                currentTarget = null;
                AcquireTarget();
                if (currentTarget == null)
                {
                    FreeLookCamera.LookAt = transform;
                }
            }
            else
            {
                cinemachineOrbitalFollow.HorizontalAxis.Value =
                    (int)Mathf.Lerp(cinemachineOrbitalFollow.HorizontalAxis.Value, 24, 0.5f);
                cinemachineOrbitalFollow.VerticalAxis.Value =
                    (int)Mathf.Lerp(cinemachineOrbitalFollow.VerticalAxis.Value, 18, 0.5f);
            }
        }
    }

    void AcquireTarget()
    {
        var enemies = Physics.OverlapSphere(transform.position, lockOnRange, enemyMask);

        if (enemies.Length == 0)
            return;

        Transform best = null;
        float bestScore = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            Vector3 toEnemy = enemy.transform.position - Camera.main.transform.position;
            float angle = Vector3.Angle(Camera.main.transform.forward, toEnemy);

            if (angle > maxAngle)
                continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            float score = dist + angle * 0.5f;

            if (score < bestScore)
            {
                bestScore = score;
                best = enemy.transform;
            }
        }

        currentTarget = best;
        aimIK.target = best;

        if (currentTarget && LockMode)
        {
            FreeLookCamera.LookAt = currentTarget.transform.Find("TargetPos");
        }
    }

    bool TargetIsValid()
    {
        if (!currentTarget)
            return false;

        if (Vector3.Distance(transform.position, currentTarget.transform.position) > lockOnRange)
            return false;

        Vector3 viewPos = Camera.main.WorldToViewportPoint(currentTarget.position);

        if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
            return false;

        return true;
    }

    public void ClearTarget()
    {
        currentTarget = null;
        FreeLookCamera.LookAt = playerCamTarget;
    }

    public void SoftTarget()
    {
        ClearTarget();
        lockOnRange = 3;
        AcquireTarget();
        if (currentTarget == null)
        {
            SoftLockMode = false;
        }
        else
        {
            SoftLockMode = true;
        }
    }
}