using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public PlayerInputHandler input;
    public Transform cameraTransform;

    private CharacterController characterController;
    private Animator animator;

    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float runSpeed = 7;
    public float walkSpeed = 3;
    public float rotationSpeed = 12f;
    public float jumpForce = 9;
    public float gravity = -20;

    private Vector3 velocity;
    private bool isGrounded;

    private PlayerCombat combat;
    LockOnSystem lockOnSystem;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        combat = GetComponent<PlayerCombat>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        lockOnSystem = GetComponent<LockOnSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleJump();
    }

    private void LateUpdate()
    {
        if (lockOnSystem.LockMode || lockOnSystem.SoftLockMode)
        {
            if (lockOnSystem.currentTarget != null)
            {
                RotateToTarget(lockOnSystem.currentTarget);
            }
            return;
        }
    }

    private void HandleMovement()
    {
        if (combat != null && combat.IsPlanted)
        {
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
            if (animator)
            {
                animator.SetFloat("Speed", 0);
            }

            return;
        }

        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2;

        Vector2 moveInput = input.MoveInput;
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        if (cameraTransform != null)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();
            move = camForward * moveInput.y + camRight * moveInput.x;
        }

        if (move.magnitude > 1) move.Normalize();

        moveSpeed = lockOnSystem.LockMode ? walkSpeed : runSpeed;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        HandleRotation(move);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        print(move.magnitude * moveSpeed);

        if (animator)
            animator.SetFloat("Speed", move.magnitude * moveSpeed);
    }

    private void HandleRotation(Vector3 move)
    {
        if (move.magnitude > 0.1)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void RotateToTarget(Transform currentTarget)
    {
        Vector3 dir = lockOnSystem.currentTarget.position - transform.position;
        dir.y = 0;

        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10f);
    }

    private void HandleJump()
    {
        if (input.JumpPressed && isGrounded)
        {
            velocity.y = jumpForce;
            if (animator)
                animator.SetTrigger("Jump");
        }
    }


}
