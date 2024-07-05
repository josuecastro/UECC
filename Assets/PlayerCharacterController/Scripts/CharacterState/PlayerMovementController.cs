using System.Collections;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    private PlayerInputManager _input;
    private CharacterController _controller;
    public bool hasTarget;
    public Transform debugger;

    [Header("Components")]
    public Transform mesh;

    [Header("Speed")]
    public float walkSpeed = 2f;
    public float sprintSpeed = 5f;

    [Header("Ground Check")]
    public float groundCheckRadius = 0.3f;
    public float groundcheckOffset = 0.25f;
    public LayerMask groundLayer;

    [Header("Jump")]
    public float jumpForce = 4f;

    [Header("Gravity")]
    public float gravityScale = 1.5f;
    private float gravity = -9.81f;
    public float terminalVelocity = -20f;

    [Header("States")]
    public bool isGrounded = true;
    public bool isStanding = true;
    public bool isAiming;
    public bool isVaulting;
    public bool isCrouching;
    public bool canJump;
    public bool canVault;
    public bool canCrouch;
    public bool canStand;

    // public enum pose {
    //     STANDING,
    //     CROUCHING,
    // }

    // public pose currentPose = pose.STANDING;
    
    // player
    private float speed;
    private Quaternion meshRotation;
    private Vector3 playerVelocity;
    private float verticalVelocity;

    private void Awake()
    {
        _input = GetComponent<PlayerInputManager>();
        _controller = GetComponent<CharacterController>();

        meshRotation = mesh.rotation;
    }

    private void Start()
    {
        isGrounded = true;

        groundCheckRadius = _controller.radius;
    }

    private void Update()
    {
        GroundCheck();
        Gravity();
        Speed();
        Movement();

        // canJump = isGrounded && !isAiming && !isVaulting && currentPose == pose.STANDING;
        // canVault = isGrounded && !isAiming && currentPose == pose.STANDING;
        // canCrouch = isGrounded && !isVaulting && currentPose == pose.STANDING;
        // canStand = isGrounded && !isVaulting && currentPose == pose.CROUCHING;

        switch (CharacterState.Instance.currentBodyPosition)
        {
            case BodyPosition.STANDING:
                Jump();
                break;
            case BodyPosition.CROUCHING:
                break;
            case BodyPosition.VAULTING:
                break;
            default:
                break;
        }
        
        Vector3 controllerVelocity = (playerVelocity * Time.deltaTime) + (new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);

        if (!isVaulting)
        {
            _controller.Move(controllerVelocity);
        }

        if (_input.jump && (canJump || canVault))
        {
            Vector3 vaultPosition = transform.position + _controller.height * transform.up + 0.75f * mesh.forward;

            if (Physics.Raycast(vaultPosition, -transform.up, out RaycastHit hitInfo, _controller.height - _controller.stepOffset, ~gameObject.layer))
            {
                if (Physics.Raycast(transform.position, mesh.forward, out RaycastHit forwardHitInfo, 1f, ~gameObject.layer))
                {
                    Vector3 wallDirection = -forwardHitInfo.normal;
                    Quaternion targetRotation = Quaternion.LookRotation(wallDirection);
                    StartCoroutine(LookAt(targetRotation, hitInfo.point.y - transform.position.y, 0.25f));
                }
            }
            else
            {
                Jump();
            }
        }
    }

    private IEnumerator LookAt(Quaternion targetRotation, float targetHeight, float duration)
    {
        CharacterState.Instance.SetCurrentBodyPosition(BodyPosition.VAULTING);

        // LOOK AT WALL
        float time = 0f;
        Quaternion startRotation = mesh.rotation;
        
        while (time < duration)
        {
            mesh.rotation = Quaternion.Slerp(startRotation, targetRotation, time / duration);
            meshRotation = mesh.rotation;
            time += Time.deltaTime;
            yield return null;
        }

        mesh.rotation = targetRotation;
        meshRotation = mesh.rotation;

        // CHANGE POSITION
        float positionTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + 0.75f * mesh.forward + targetHeight * transform.up;

        while (positionTime < 0.5f)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, positionTime / 0.5f);
            positionTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        CharacterState.Instance.SetCurrentBodyPosition(BodyPosition.STANDING);
    }

    private void LateUpdate()
    {
        Rotation();
    }

    private void GroundCheck()
    {
        if (_controller.isGrounded)
        {
            isGrounded = _controller.isGrounded;
        }
        else
        {
            isGrounded = Physics.CheckSphere(transform.position + groundcheckOffset * transform.up, groundCheckRadius, groundLayer);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position + groundcheckOffset * transform.up, groundCheckRadius);
    }

    private void Gravity()
    {
        if (isGrounded)
        {
            if (verticalVelocity <= 0f)
            {
                verticalVelocity = -1f - new Vector2(_controller.velocity.x, _controller.velocity.z).magnitude;
            }
        }
        else
        {
            if (verticalVelocity > terminalVelocity)
            {
                verticalVelocity += gravity * gravityScale * Time.deltaTime;
            }
            else
            {
                verticalVelocity = terminalVelocity;
            }

            if (_controller.velocity.y == 0f && verticalVelocity > 0f)
            {
                verticalVelocity = 0f;
            }
        }
    }

    private void Jump()
    {
        if (isGrounded && canJump)
        {
            if (_input.jump)
            {
                verticalVelocity = jumpForce * gravityScale;
                playerVelocity *= 1.2f;
            }
        }
    }

    private void Crouch()
    {

    }

    private void Speed()
    {
        float targetSpeed = _input.sprint && _input.move != Vector2.zero ? sprintSpeed : walkSpeed;
        if (_input.move == Vector2.zero) targetSpeed = 0f;
        if (isAiming) targetSpeed = walkSpeed;
        if (isVaulting) targetSpeed = 0f;

        speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime * 8f);
    }

    private void Movement()
    {
        Vector3 inputDirection = new Vector3(_input.move.x, 0f, _input.move.y);
        Vector3 movementDirection = transform.TransformDirection(inputDirection);

        float deltaMultiplier = isGrounded ? 8f : 1f;

        playerVelocity = Vector3.Lerp(playerVelocity, movementDirection * speed, Time.deltaTime * deltaMultiplier);
    }

    private void Rotation()
    {
        Vector3 inputDirection = new Vector3(_input.move.x, 0f, _input.move.y);
        Vector3 movementDirection = transform.TransformDirection(inputDirection);

        if (isVaulting) return;

        isAiming = _input.aim;

        if (isAiming)
        {
            Quaternion targetDirection = Quaternion.LookRotation(transform.forward);
            meshRotation = Quaternion.Slerp(meshRotation, targetDirection, Time.deltaTime * 25f);
        }
        else
        {
            if (inputDirection != Vector3.zero)
            {
                Quaternion targetDirection = Quaternion.LookRotation(movementDirection);
                meshRotation = Quaternion.Slerp(meshRotation, targetDirection, Time.deltaTime * 8f);
            }
        }

        mesh.rotation = meshRotation;
    }
}
