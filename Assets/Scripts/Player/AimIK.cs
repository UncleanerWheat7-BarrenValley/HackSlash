using UnityEngine;

public class AimIK : MonoBehaviour
{
    public LockOnSystem lockOnSystem;
    public Transform target;
    public Animator animator;
    [Range(0, 1)] public float headWeight = 1;
    [Range(0, 1)] public float bodyWeight = 0.5f;
    [Range(0, 1)] public float handWeight = 1;
    public Transform rightHandTransform;
    public Transform leftHandTransform;
    public float offsetNumberThing = 0;

    private void OnAnimatorIK(int layerIndex)
    {
        if (target == null || animator == null || lockOnSystem.currentTarget == null || animator.GetLayerWeight(1) == 0)
            return;

        target = lockOnSystem.currentTarget;
        Vector3 targetPosition = target.position + Vector3.up * 1.5f; // Adjust height offset

        // Look at target with head and body
        animator.SetLookAtPosition(targetPosition);
        animator.SetLookAtWeight(headWeight, bodyWeight);

        // Calculate direction from hand to target
        Vector3 rightHandDirection = (targetPosition - rightHandTransform.position).normalized;
        Vector3 leftHandDirection = (targetPosition - leftHandTransform.position).normalized;

        // Calculate rotations for hands to face the target
        // Add a 90-degree offset if the hand's forward axis is not aligned with the gun's barrel
        Quaternion rightHandRotation = Quaternion.LookRotation(rightHandDirection) * Quaternion.Euler(0,0, -offsetNumberThing);
        Quaternion leftHandRotation = Quaternion.LookRotation(leftHandDirection) * Quaternion.Euler(0,0, offsetNumberThing);

        // Apply IK positions and rotations
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, handWeight);
        animator.SetIKPosition(AvatarIKGoal.RightHand, targetPosition);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandRotation);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, handWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, handWeight);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, targetPosition);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandRotation);
    }
}
