
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    [Header("Camara")]
    public Transform playerCamera;
    public float cameraHeight = 1.6f;
    public float mouseSensitivity = 2f;
    public float minVerticalAngle = -85f;
    public float maxVerticalAngle = 85f;

    private CharacterController controller;
    private Vector3 velocity;
    private float verticalRotation;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
                playerCamera = cam.transform;
        }

        if (playerCamera != null)
        {
            playerCamera.localPosition =
                new Vector3(0f, cameraHeight, 0f);

            playerCamera.localRotation = Quaternion.identity;
        }

        LockCursor();
    }

    void Update()
    {
        HandleCursor();
        HandleCamera();
        HandleMovement();
    }

    void HandleCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButtonDown(0) &&
            Cursor.lockState != CursorLockMode.Locked)
        {
            LockCursor();
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void HandleCamera()
    {
        if (playerCamera == null ||
            Cursor.lockState != CursorLockMode.Locked)
            return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Giro horizontal del jugador
        transform.Rotate(Vector3.up * mouseX);

        // Giro vertical de la camara
        verticalRotation -= mouseY;

        verticalRotation = Mathf.Clamp(
            verticalRotation,
            minVerticalAngle,
            maxVerticalAngle
        );

        playerCamera.localRotation =
            Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move =
            transform.right * moveX +
            transform.forward * moveZ;

        move = Vector3.ClampMagnitude(move, 1f);

        controller.Move(
            move * moveSpeed * Time.deltaTime
        );

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(
            velocity * Time.deltaTime
        );
    }
}
