using UnityEngine;

/// <summary>
/// Zona persistente de una sala especial.
/// - El trigger 2D detecta la entrada desde la exploración 2D y activa 3D.
/// - El trigger 3D detecta la salida de la sala y restaura 2D.
///
/// Este objeto debe permanecer activo y fuera de las raíces World2D/World3D.
/// </summary>
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Collider))]
public sealed class PerspectiveRoomZone : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PerspectiveSwitcher perspectiveSwitcher;
    [Tooltip("Transform raíz del jugador. Puede ser el objeto que contiene Rigidbody2D/CharacterController.")]
    [SerializeField] private Transform playerRoot;

    [Header("Opciones")]
    [Tooltip("Perspectiva que se activa al entrar desde el mundo 2D.")]
    [SerializeField] private GamePerspective perspectiveInsideRoom = GamePerspective.ThreeD;
    [Tooltip("Perspectiva que se restaura al salir del volumen 3D.")]
    [SerializeField] private GamePerspective perspectiveOutsideRoom = GamePerspective.TwoD;

    private Collider2D entryTrigger2D;
    private Collider exitTrigger3D;

    private void Awake()
    {
        entryTrigger2D = GetComponent<Collider2D>();
        exitTrigger3D = GetComponent<Collider>();

        entryTrigger2D.isTrigger = true;
        exitTrigger3D.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other.transform))
            return;

        if (perspectiveSwitcher != null &&
            perspectiveSwitcher.CurrentPerspective != perspectiveInsideRoom)
        {
            perspectiveSwitcher.SetPerspective(perspectiveInsideRoom);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other.transform))
            return;

        if (perspectiveSwitcher != null &&
            perspectiveSwitcher.CurrentPerspective != perspectiveOutsideRoom)
        {
            perspectiveSwitcher.SetPerspective(perspectiveOutsideRoom);
        }
    }

    private bool IsPlayer(Transform candidate)
    {
        if (playerRoot == null || candidate == null)
            return false;

        return candidate == playerRoot ||
               candidate.IsChildOf(playerRoot) ||
               playerRoot.IsChildOf(candidate);
    }

    private void OnValidate()
    {
        if (perspectiveInsideRoom == perspectiveOutsideRoom)
        {
            Debug.LogWarning(
                "[PerspectiveRoomZone] La perspectiva interior y exterior deben ser diferentes.",
                this);
        }
    }
}
