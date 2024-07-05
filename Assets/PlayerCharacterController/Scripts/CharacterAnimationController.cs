using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private CharacterController _controller;
    private PlayerMovementController _movementController;
    private PlayerInputManager _input;

    public Animator animator;

    private float animationSpeed;
    private Vector3 animationVelocity;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _movementController = GetComponent<PlayerMovementController>();
        _input = GetComponent<PlayerInputManager>();

        if (animator == null)
        {
            animator = transform.GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        float controllerSpeed = new Vector2(_controller.velocity.x, _controller.velocity.z).magnitude;
        animationSpeed = Mathf.Lerp(animationSpeed, controllerSpeed, Time.deltaTime * 8f);

        Vector3 controllerVelocity = new Vector3(_input.move.x * animationSpeed, 0f, _input.move.y * animationSpeed);
        animationVelocity = Vector3.Lerp(animationVelocity, controllerVelocity, Time.deltaTime * 8f);

        animator.SetBool("Aiming", _movementController.isAiming);
        animator.SetBool("Grounded", _movementController.isGrounded);
        animator.SetBool("ClimbUp", _movementController.isVaulting);
        animator.SetFloat("Speed", animationSpeed);
        animator.SetFloat("X", animationVelocity.x);
        animator.SetFloat("Y", animationVelocity.z);

        // if (_movementController.vaulting)
        // {
        //     animator.applyRootMotion = true;
        // } else
        // {
        //     animator.applyRootMotion = false;
        //     animator.transform.position = transform.position;
        // }
    }
}
