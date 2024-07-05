using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerInput playerInput;

    [HideInInspector] public InputAction moveAction;
    [HideInInspector] public InputAction lookAction;
    [HideInInspector] public InputAction jumpAction;
    [HideInInspector] public InputAction sprintAction;
    [HideInInspector] public InputAction crouchAction;
    [HideInInspector] public InputAction interactAction;

    [HideInInspector] public InputAction aimAction;

    [HideInInspector] public InputAction attackAction;

    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool crouch;

    public bool aim;
    public bool attack;

    public bool isGamepad;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];
        crouchAction = playerInput.actions["Crouch"];
        interactAction = playerInput.actions["Interact"];
        aimAction = playerInput.actions["Aim"];
        attackAction = playerInput.actions["Attack"];
    }

    private void OnEnable()
    {
        moveAction.performed += ctx => move = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => move = ctx.ReadValue<Vector2>();

        lookAction.performed += ctx => look = ctx.ReadValue<Vector2>();
        lookAction.canceled += ctx => look = ctx.ReadValue<Vector2>();

        sprintAction.performed += ctx => jump = true;
        sprintAction.canceled += ctx => jump = false;

        aimAction.performed += ctx => aim = true;
        aimAction.canceled += ctx => aim = false;

        attackAction.performed += ctx => attack = true;
        attackAction.canceled += ctx => attack = false;
    }

    private void OnDisable()
    {
        moveAction.performed -= ctx => move = ctx.ReadValue<Vector2>();
        moveAction.canceled -= ctx => move = ctx.ReadValue<Vector2>();

        lookAction.performed -= ctx => look = ctx.ReadValue<Vector2>();
        lookAction.canceled -= ctx => look = ctx.ReadValue<Vector2>();

        sprintAction.performed -= ctx => jump = true;
        sprintAction.canceled -= ctx => jump = false;

        aimAction.performed -= ctx => aim = true;
        aimAction.canceled -= ctx => aim = false;

        attackAction.performed -= ctx => attack = true;
        attackAction.canceled -= ctx => attack = false;
    }

    private void Update()
    {
        jump = jumpAction.triggered;
        crouch = crouchAction.triggered;

        isGamepad = playerInput.currentControlScheme == "Gamepad";
    }
}
