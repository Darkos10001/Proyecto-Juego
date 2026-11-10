using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform cameraTransform;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float gravity = -20f;

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 5f;
    [SerializeField] private float staminaDrainPerSecond = 1f;
    [SerializeField] private float staminaRecoveryPerSecond = 1.5f;
    [SerializeField] private float recoveryDelay = 1f;

    [Header("Salto")]
    [SerializeField] private float jumpHeight = 1.5f;

    [Header("Mirar")]
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private bool invertY = false;

    [Header("Entrada de escritorio")]
    [SerializeField] private bool useLegacyDesktopInput = true;
    [SerializeField] private bool lockCursorOnStart = true;

    private CharacterController characterController;
    private Vector2 mobileMoveInput;
    private Vector2 mobileLookInput;
    private float verticalVelocity;
    private float cameraPitch;
    private float currentStamina;
    private float staminaRecoveryTimer;
    private bool lookTouchActive;
    private bool jumpRequested;
    private bool sprintRequested;

    /// <summary>Stamina actual, útil para actualizar una barra de UI.</summary>
    public float CurrentStamina => currentStamina;

    /// <summary>Stamina máxima.</summary>
    public float MaxStamina => maxStamina;

    /// <summary>Valor entre 0 y 1 para llenar una barra de stamina.</summary>
    public float StaminaNormalized => maxStamina <= 0f ? 0f : currentStamina / maxStamina;

    /// <summary>Indica si el personaje está corriendo actualmente.</summary>
    public bool IsSprinting { get; private set; }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        currentStamina = maxStamina;

        if (cameraTransform == null)
        {
            Camera childCamera = GetComponentInChildren<Camera>();
            if (childCamera != null)
                cameraTransform = childCamera.transform;
        }

        if (cameraTransform == null)
            Debug.LogError("Asigna la cámara FPS en Camera Transform.", this);
    }

    private void Start()
    {
        if (lockCursorOnStart && Application.isEditor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (cameraTransform != null)
            cameraPitch = NormalizeAngle(cameraTransform.localEulerAngles.x);
    }

    private void Update()
    {
        Vector2 moveInput = mobileMoveInput;
        Vector2 lookInput = mobileLookInput;

        if (useLegacyDesktopInput && Application.isEditor)
        {
            moveInput = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );

            lookInput += new Vector2(
                Input.GetAxis("Mouse X"),
                Input.GetAxis("Mouse Y")
            );

            sprintRequested = Input.GetKey(KeyCode.LeftShift);

            if (Input.GetButtonDown("Jump"))
                Jump();
        }

        UpdateStamina(moveInput);
        Move(moveInput);
        Look(lookInput);

        // El desplazamiento táctil de mirada se consume una sola vez.
        mobileLookInput = Vector2.zero;
    }

    private void UpdateStamina(Vector2 moveInput)
    {
        bool hasMovementInput = moveInput.sqrMagnitude > 0.01f;
        IsSprinting = sprintRequested && hasMovementInput && currentStamina > 0.01f;

        if (IsSprinting)
        {
            currentStamina -= staminaDrainPerSecond * Time.deltaTime;
            currentStamina = Mathf.Max(0f, currentStamina);
            staminaRecoveryTimer = recoveryDelay;

            if (currentStamina <= 0f)
                IsSprinting = false;
        }
        else
        {
            if (staminaRecoveryTimer > 0f)
                staminaRecoveryTimer -= Time.deltaTime;
            else
                currentStamina = Mathf.Min(
                    maxStamina,
                    currentStamina + staminaRecoveryPerSecond * Time.deltaTime
                );
        }
    }

    private void Move(Vector2 input)
    {
        input = Vector2.ClampMagnitude(input, 1f);

        float currentSpeed = IsSprinting ? sprintSpeed : moveSpeed;
        Vector3 direction = transform.right * input.x + transform.forward * input.y;
        Vector3 velocity = direction * currentSpeed;

        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (jumpRequested)
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        jumpRequested = false;
        verticalVelocity += gravity * Time.deltaTime;
        velocity.y = verticalVelocity;

        characterController.Move(velocity * Time.deltaTime);
    }

    private void Look(Vector2 input)
    {
        if (cameraTransform == null)
            return;

        float yaw = input.x * lookSensitivity;
        float pitchDirection = invertY ? 1f : -1f;
        float pitchChange = input.y * lookSensitivity * pitchDirection;

        transform.Rotate(0f, yaw, 0f);

        cameraPitch = Mathf.Clamp(
            cameraPitch + pitchChange,
            minPitch,
            maxPitch
        );

        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    public void SetMoveInput(Vector2 input)
    {
        mobileMoveInput = Vector2.ClampMagnitude(input, 1f);
    }

    public void AddLookInput(Vector2 delta)
    {
        if (lookTouchActive)
            mobileLookInput += delta;
    }

    public void SetLookTouchActive(bool active)
    {
        lookTouchActive = active;

        if (!active)
            mobileLookInput = Vector2.zero;
    }

    /// <summary>Conecta al botón táctil de salto.</summary>
    public void Jump()
    {
        if (characterController != null && characterController.isGrounded)
            jumpRequested = true;
    }

    /// <summary>
    /// Para Android: llama con true al presionar el botón de correr
    /// y con false al soltarlo.
    /// </summary>
    public void SetSprintInput(bool active)
    {
        sprintRequested = active;
    }

    public void SetMoveSpeed(float value)
    {
        moveSpeed = Mathf.Max(0f, value);
    }

    public void SetSprintSpeed(float value)
    {
        sprintSpeed = Mathf.Max(moveSpeed, value);
    }

    private static float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }

    private void OnDisable()
    {
        mobileMoveInput = Vector2.zero;
        mobileLookInput = Vector2.zero;
        lookTouchActive = false;
        jumpRequested = false;
        sprintRequested = false;
        IsSprinting = false;
    }
}
