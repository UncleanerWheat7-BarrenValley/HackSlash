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
    public float rotationSpeed = 12f;
    public float jumpForce = 9;
    public float gravity = -20;

    private Vector3 velocity;
    private bool isGrounded;

    private PlayerCombat combat;

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
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleJump();
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
        if(isGrounded && velocity.y <0) velocity.y = -2;

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

        characterController.Move(move * moveSpeed * Time.deltaTime);

        if (move.magnitude > 0.1) 
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        if(animator)
            animator.SetFloat("Speed", move.magnitude);
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
