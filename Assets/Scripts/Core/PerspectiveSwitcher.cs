using System;
using UnityEngine;

/// <summary>
/// Activa una de las dos representaciones del mismo espacio de juego.
/// Los cambios pueden solicitarse desde zonas de sala u otros sistemas.
/// </summary>
public sealed class PerspectiveSwitcher : MonoBehaviour
{
    [Serializable]
    public sealed class PerspectiveSetup
    {
        [Tooltip("Raíz del escenario de esta perspectiva. No debe contener al jugador ni al sistema de cambio.")]
        public GameObject worldRoot;
        [Tooltip("Cámara de esta perspectiva.")]
        public Camera perspectiveCamera;
        [Tooltip("Punto equivalente de aparición al entrar en esta perspectiva.")]
        public Transform playerEntryPoint;
        [Tooltip("Controladores exclusivos de esta perspectiva.")]
        public Behaviour[] enabledBehaviours;
    }

    [Header("Referencias compartidas")]
    [SerializeField] private Transform player;
    [SerializeField] private CharacterController characterController;

    [Header("Configuración de perspectivas")]
    [SerializeField] private PerspectiveSetup twoD;
    [SerializeField] private PerspectiveSetup threeD;
    [SerializeField] private GamePerspective startingPerspective = GamePerspective.TwoD;

    [Header("Depuración opcional")]
    [SerializeField] private bool allowKeyboardSwitch = false;
    [SerializeField] private KeyCode debugSwitchKey = KeyCode.Tab;

    public GamePerspective CurrentPerspective { get; private set; }
    public bool IsInitialized { get; private set; }
    public event Action<GamePerspective> PerspectiveChanged;

    private void Start()
    {
        if (!ValidateSetup())
        {
            enabled = false;
            return;
        }

        ApplyPerspective(startingPerspective, false);
        IsInitialized = true;
    }

    private void Update()
    {
        // Solo para pruebas en Editor/PC. En Android el cambio se realiza
        // mediante PerspectiveRoomZone, no mediante teclado.
        if (allowKeyboardSwitch && Input.GetKeyDown(debugSwitchKey))
            TogglePerspective();
    }

    public void TogglePerspective()
    {
        SetPerspective(CurrentPerspective == GamePerspective.TwoD
            ? GamePerspective.ThreeD
            : GamePerspective.TwoD);
    }

    public void SetPerspective(GamePerspective nextPerspective)
    {
        if (!IsInitialized)
        {
            Debug.LogWarning("[PerspectiveSwitcher] El sistema aún no está inicializado.", this);
            return;
        }

        if (nextPerspective == CurrentPerspective)
            return;

        ApplyPerspective(nextPerspective, true);
    }

    private void ApplyPerspective(GamePerspective nextPerspective, bool movePlayer)
    {
        PerspectiveSetup next = GetSetup(nextPerspective);
        PerspectiveSetup previous = GetSetup(nextPerspective == GamePerspective.TwoD
            ? GamePerspective.ThreeD
            : GamePerspective.TwoD);

        // Apagar primero el controlador y la cámara anteriores evita entradas dobles.
        SetBehaviours(previous.enabledBehaviours, false);
        if (previous.perspectiveCamera != null)
            previous.perspectiveCamera.gameObject.SetActive(false);
        if (previous.worldRoot != null)
            previous.worldRoot.SetActive(false);

        if (next.worldRoot != null)
            next.worldRoot.SetActive(true);

        if (movePlayer && player != null && next.playerEntryPoint != null)
        {
            if (characterController != null)
                characterController.enabled = false;

            player.SetPositionAndRotation(
                next.playerEntryPoint.position,
                next.playerEntryPoint.rotation);

            if (characterController != null)
                characterController.enabled = true;
        }

        if (next.perspectiveCamera != null)
            next.perspectiveCamera.gameObject.SetActive(true);
        SetBehaviours(next.enabledBehaviours, true);

        CurrentPerspective = nextPerspective;
        PerspectiveChanged?.Invoke(CurrentPerspective);
    }

    private PerspectiveSetup GetSetup(GamePerspective perspective)
    {
        return perspective == GamePerspective.TwoD ? twoD : threeD;
    }

    private static void SetBehaviours(Behaviour[] behaviours, bool state)
    {
        if (behaviours == null) return;

        foreach (Behaviour behaviour in behaviours)
        {
            if (behaviour != null)
                behaviour.enabled = state;
        }
    }

    private bool ValidateSetup()
    {
        if (player == null)
        {
            Debug.LogError("[PerspectiveSwitcher] Asigna el Transform del jugador.", this);
            return false;
        }

        if (twoD == null || threeD == null ||
            twoD.perspectiveCamera == null || threeD.perspectiveCamera == null)
        {
            Debug.LogError("[PerspectiveSwitcher] Asigna las dos configuraciones y sus cámaras.", this);
            return false;
        }

        if (twoD.playerEntryPoint == null || threeD.playerEntryPoint == null)
        {
            Debug.LogError("[PerspectiveSwitcher] Asigna ambos puntos de entrada del jugador.", this);
            return false;
        }

        if (twoD.perspectiveCamera == threeD.perspectiveCamera)
        {
            Debug.LogError("[PerspectiveSwitcher] Cada perspectiva necesita su propia cámara.", this);
            return false;
        }

        if (twoD.worldRoot != null && twoD.worldRoot == threeD.worldRoot)
        {
            Debug.LogError("[PerspectiveSwitcher] Las raíces 2D y 3D deben ser objetos diferentes.", this);
            return false;
        }

        return true;
    }
}
