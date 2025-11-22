using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions inputActions;
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool GunPressed { get; private set; }
    public bool LockOnHeld { get; private set; }
    private void Awake()
    {
        inputActions = new PlayerInputActions(); Debug.Log("Gameplay input map enabled");
    }
    private void OnEnable()
    {
        inputActions.Gameplay.Enable();
        inputActions.Gameplay.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        inputActions.Gameplay.Move.canceled += ctx => MoveInput = Vector2.zero;
        inputActions.Gameplay.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        inputActions.Gameplay.Look.canceled += ctx => LookInput = Vector2.zero;
        inputActions.Gameplay.Jump.performed += ctx => JumpPressed = true;
        inputActions.Gameplay.Attack.performed += ctx => AttackPressed = true;
        inputActions.Gameplay.Gun.performed += ctx => GunPressed = true;
        inputActions.Gameplay.LockOn.performed += ctx => LockOnHeld = true;
        inputActions.Gameplay.LockOn.canceled += ctx => LockOnHeld = false;
    }
    private void OnDisable() => inputActions.Gameplay.Disable();
    private void Update()
    {
        if (LookInput != Vector2.zero)
            Debug.Log("Look input: " + LookInput);
    }
    private void LateUpdate()
    {
        JumpPressed = AttackPressed = GunPressed = false;
    }
}