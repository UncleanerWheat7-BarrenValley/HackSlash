using System;
using UnityEngine;

public class LocalForwardHelper : MonoBehaviour
{
    [Header("References")] public PlayerInputHandler input;
    public Camera mainCam;
    

    private void Start()
    {
        mainCam = Camera.main;
    }

    public float GetLocalMoveZ()
    {
        Vector3 moveDirection = GetCameraRelativeMoveDirection();
        Vector3 localMoveDirection = transform.InverseTransformDirection(moveDirection);
        return localMoveDirection.z;
    }

    private Vector3 GetCameraRelativeMoveDirection()
    {
        Vector2 moveInput = input.MoveInput;
        Vector3 camForward = mainCam.transform.forward;
        Vector3 camRight = mainCam.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();
        return (camForward * moveInput.y + camRight * moveInput.x).normalized;
    }
}
