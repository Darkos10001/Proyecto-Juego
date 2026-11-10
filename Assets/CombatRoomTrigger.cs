using UnityEngine;

public class CombatRoomTrigger : MonoBehaviour {
    private Room roomScript;
    private bool eventTriggered = false;

    void Start() {
        roomScript = GetComponentInParent<Room>();

        BoxCollider2D triggerCol = GetComponent<BoxCollider2D>();
        if (triggerCol != null) {
            triggerCol.isTrigger = true;
            triggerCol.size = new Vector2(22f, 20f); // Tamaño adaptable al interior de la sala
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        // Si el jugador entra, la sala no está limpia y no se ha activado el evento
        if (collision.CompareTag("Player") && !eventTriggered && roomScript != null && !roomScript.isCleared) {
            eventTriggered = true;
            roomScript.playerInside = true;

            // 1. Cerramos las puertas de esta sala en el mapa 2D
            roomScript.LockDoors();

            // 2. Llamamos al gestor para saltar a la escena 3D de combate
            if (SceneTransitionManager.Instance != null) {
                // Pasamos el nombre único de este objeto sala para recordarlo al volver
                string roomId = transform.parent.name;
                SceneTransitionManager.Instance.EnterCombatRoom(roomId);
            } else {
                Debug.LogWarning("Falta colocar el SceneTransitionManager en la escena.");
            }
        }
    }
}