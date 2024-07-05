using UnityEngine;

[ExecuteInEditMode]
public class SpringArm : MonoBehaviour
{
    public Transform cameraRoot;
    public Transform cameraTarget;
    public Camera playerCamera;
    [Range(-1f, 1f)] public float cameraSide = 0.0f;
    [Range(0f, 10f)] public float cameraDistance = 2.5f;
    [Range(0f, 1f)] public float cameraRadius = 0.5f;
    public LayerMask cameraCollisionLayer;

    private void Awake()
    {
        if (cameraTarget == null)
        {
            cameraTarget = transform.Find("CameraTarget");
        }
        else return;

        if (cameraRoot == null) return;

        if (playerCamera == null) return;
    }

    private void LateUpdate()
    {
        transform.position = cameraRoot.position;
        transform.rotation = cameraRoot.rotation;

        float effectiveDistance = cameraDistance - cameraRadius;

        Vector3 localOffset = new Vector3(cameraSide, 0f, 0f);
        Vector3 worldOffset = cameraRoot.TransformDirection(localOffset);
        Vector3 cameraRootPosition = cameraRoot.position + worldOffset;

        if (Physics.Raycast(cameraRootPosition, -cameraRoot.forward, out RaycastHit hitInfo, effectiveDistance, cameraCollisionLayer))
        {
            cameraTarget.position = Vector3.Lerp(cameraTarget.position, hitInfo.point, Time.deltaTime * 50f);
        }
        else
        {
            cameraTarget.position = Vector3.Lerp(cameraTarget.position, cameraRootPosition - cameraRoot.forward * cameraDistance, Time.deltaTime * 50f);
        }

        playerCamera.transform.position = cameraTarget.position;
        playerCamera.transform.rotation = cameraTarget.rotation;
    }
}
