using UnityEngine;

/// <summary>
/// Zona persistente de una sala especial.
/// El trigger 2D detecta la entrada desde la exploración 2D y activa 3D.
/// El trigger 3D detecta la salida de la sala y restaura 2D.
/// Este objeto debe permanecer activo fuera de World2D y World3D.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(BoxCollider))]
public sealed class PerspectiveRoomZone : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PerspectiveSwitcher perspectiveSwitcher;
    [Tooltip("Transform raíz del jugador, que contiene sus colliders/controladores.")]
    [SerializeField] private Transform playerRoot;

    [Header("Perspectivas")]
    [SerializeField] private GamePerspective perspectiveInsideRoom = GamePerspective.ThreeD;
    [SerializeField] private GamePerspective perspectiveOutsideRoom = GamePerspective.TwoD;

    private BoxCollider2D entryTrigger2D;
    private BoxCollider exitTrigger3D;

    private void Awake()
    {
        entryTrigger2D = GetComponent<BoxCollider2D>();
        exitTrigger3D = GetComponent<BoxCollider>();

        entryTrigger2D.isTrigger = true;
        exitTrigger3D.isTrigger = true;
    }

    // Solo se procesa la ENTRADA 2D. No se procesa OnTriggerExit2D porque
    // el mundo 2D se apaga al entrar y ese apagado no debe restaurar el modo 2D.
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

    // Este collider 3D permanece activo durante todo el modo 3D.
    // Al abandonar el volumen de la sala se vuelve a la perspectiva 2D.
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
                "[PerspectiveRoomZone] Las perspectivas interior y exterior deben ser diferentes.",
                this);
        }
    }
}
