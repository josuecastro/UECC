using UnityEngine;

public class PlayerLookController : MonoBehaviour
{
    private PlayerInputManager _input;

    private float yaw;
    private float pitch;

    private float smoothYaw;
    private float smoothPitch;

    public float sensitivity = 1f;
    public float smoothing = 40f;
    public bool rawInput;

    public Transform cameraRoot;
    public Transform meshCameraPosition;

    private void Awake()
    {
        _input = GetComponent<PlayerInputManager>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        if (meshCameraPosition != null)
        {
            cameraRoot.position = meshCameraPosition.position;
        }

        float deltaMultiplier = _input.isGamepad ? Time.deltaTime : 1f;

        if (_input.look != Vector2.zero)
        {
            yaw += _input.look.x * sensitivity * deltaMultiplier;
            pitch += -_input.look.y * sensitivity * deltaMultiplier;

            if (yaw > 360f) yaw += 360f;
            if (yaw < -360f) yaw -= -360f;

            pitch = Mathf.Clamp(pitch, -70f, 70f);
        }

        if (rawInput)
        {
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            cameraRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
        else
        {
            smoothYaw = Mathf.LerpAngle(smoothYaw, yaw, Time.deltaTime * smoothing);
            smoothPitch = Mathf.LerpAngle(smoothPitch, pitch, Time.deltaTime * smoothing);

            if (smoothYaw > 360f) smoothYaw += 360f;
            if (smoothYaw < -360f) smoothYaw -= -360f;

            smoothPitch = Mathf.Clamp(smoothPitch, -70f, 70f);

            transform.rotation = Quaternion.Euler(0f, smoothYaw, 0f);
            cameraRoot.localRotation = Quaternion.Euler(smoothPitch, 0f, 0f);
        }
    }
}
