using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera aimCamera;
    private PlayerMovementController _movementController;

    private void Awake()
    {
        _movementController = GetComponent<PlayerMovementController>();
        aimCamera.gameObject.SetActive(false);
    }

    private void Update()
    {
        aimCamera.gameObject.SetActive(_movementController.isAiming);
    }
}
