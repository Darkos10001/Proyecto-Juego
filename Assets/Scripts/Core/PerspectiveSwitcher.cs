using System;
using UnityEngine;

/// <summary>
/// Alterna entre las representaciones 2D y 3D de una misma partida.
/// Coloca las dos versiones del escenario en la escena y configura sus
/// puntos de entrada equivalentes en el Inspector.
/// </summary>
public sealed class PerspectiveSwitcher : MonoBehaviour
{
    [Serializable]
    public sealed class PerspectiveSetup
    {
        public GamePerspective perspective;
        [Tooltip("Raíz del escenario de esta perspectiva. Puede quedar vacía si el escenario siempre está activo.")]
        public GameObject worldRoot;
        [Tooltip("Cámara que debe quedar activa en esta perspectiva.")]
        public Camera perspectiveCamera;
        [Tooltip("Punto equivalente donde aparecerá el jugador al entrar en esta perspectiva.")]
        public Transform playerEntryPoint;
        [Tooltip("Controladores que deben estar habilitados en esta perspectiva.")]
        public Behaviour[] enabledBehaviours;
    }

    [Header("Referencias compartidas")]
    [SerializeField] private Transform player;
    [SerializeField] private CharacterController characterController;

    [Header("Configuración de perspectivas")]
    [SerializeField] private PerspectiveSetup twoD;
    [SerializeField] private PerspectiveSetup threeD;
    [SerializeField] private GamePerspective startingPerspective = GamePerspective.TwoD;

    [Header("Cambio de perspectiva")]
    [SerializeField] private KeyCode switchKey = KeyCode.Tab;
    [SerializeField] private bool allowKeyboardSwitch = true;

    public GamePerspective CurrentPerspective { get; private set; }
    public event Action<GamePerspective> PerspectiveChanged;

    private bool initialized;

    private void Start()
    {
        if (!ValidateSetup())
        {
            enabled = false;
            return;
        }

        ApplyPerspective(startingPerspective, false);
        initialized = true;
    }

    private void Update()
    {
        if (allowKeyboardSwitch && Input.GetKeyDown(switchKey))
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
        if (!initialized)
        {
            Debug.LogWarning("[PerspectiveSwitcher] El sistema todavía no está inicializado.", this);
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

        // Desactivar primero los controladores y la cámara anteriores evita
        // que ambos sistemas de entrada controlen al jugador simultáneamente.
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

        if (twoD == null || threeD == null)
        {
            Debug.LogError("[PerspectiveSwitcher] Configura los dos modos en el Inspector.", this);
            return false;
        }

        if (twoD.perspectiveCamera == null || threeD.perspectiveCamera == null)
        {
            Debug.LogError("[PerspectiveSwitcher] Asigna una cámara para cada perspectiva.", this);
            return false;
        }

        if (twoD.playerEntryPoint == null || threeD.playerEntryPoint == null)
        {
            Debug.LogError("[PerspectiveSwitcher] Asigna los puntos de entrada 2D y 3D.", this);
            return false;
        }

        if (twoD.perspectiveCamera == threeD.perspectiveCamera)
        {
            Debug.LogError("[PerspectiveSwitcher] Cada perspectiva necesita su propia cámara.", this);
            return false;
        }

        return true;
    }
}
