using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineCamera cmCam;
    
    [Header("Camera Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float zoomSensitivity = 2f;

    [SerializeField] private Controller controller;

    private CinemachineFollow followComponent;

    private float targetZoomHeight = 10f;
    private readonly float minHeight = 0.8f;
    private readonly float maxHeight = 10f;
    private readonly float zoomSpeed = 15f;

    private readonly float minRotationX = 0f;
    private readonly float maxRotationX = 45f;

    private void Start()
    {
        InitCinemachine();
        UpdateCameraPosition();
        UpdateCameraRotation();
    }

    private void InitCinemachine()
    {
        if (cmCam == null)
        {
            cmCam = Object.FindFirstObjectByType<CinemachineCamera>();
            if (cmCam == null)
            {
                Debug.LogError("No cinemachine Camera found in the scene");
                enabled = false;
                return;
            }
        }

        followComponent = cmCam.GetComponent<CinemachineFollow>();
        if (followComponent == null)
        {
            Debug.LogError("CinemachineFollow component is missing on the camera");
            enabled = false;
        }
    }

    private void Update()
    {
        HandleCameraMovement();
        HandleCameraRotation();
        HandleCameraZoom();
    }

    private void HandleCameraMovement()
    {
        Vector3 moveVector = transform.forward * controller.inputMoveDirection.y + transform.right * controller.inputMoveDirection.x;
        transform.position += moveVector * moveSpeed * Time.fixedDeltaTime;
    }

    private void HandleCameraRotation()
    {
        float horizontalRotation = controller.inputRotationDirection.x * rotationSpeed * Time.fixedDeltaTime;
        transform.Rotate(0, horizontalRotation, 0);
    }

    private void HandleCameraZoom()
    {
        float zoomInput = Input.mouseScrollDelta.y * zoomSensitivity;

        if (Mathf.Approximately(zoomInput, 0f)) return;

        targetZoomHeight = Mathf.Clamp(targetZoomHeight - zoomInput * zoomSpeed * Time.deltaTime,
            minHeight, maxHeight);

        UpdateCameraPosition();
        UpdateCameraRotation();
    }

    private void UpdateCameraPosition()
    {
        Vector3 followOffset = followComponent.FollowOffset;
        followOffset.y = targetZoomHeight;
        followComponent.FollowOffset = followOffset;
    }

    private void UpdateCameraRotation()
    {
        float t = Mathf.InverseLerp(minHeight, maxHeight, targetZoomHeight);
        float targetXRotation = Mathf.Lerp(minRotationX, maxRotationX, t);

        cmCam.transform.rotation = Quaternion.Euler(
            targetXRotation,
            transform.eulerAngles.y,
            transform.eulerAngles.z
        );
    }
}
