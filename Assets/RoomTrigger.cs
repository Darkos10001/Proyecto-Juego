using UnityEngine;

public class RoomTrigger : MonoBehaviour {
    private Room roomScript;
    private EnemySpawner enemySpawner;
    private bool eventTriggered = false;

    void Start() {
        // Obtenemos el script Room del objeto padre
        roomScript = GetComponentInParent<Room>();

        // Buscamos el spawner hijo de la sala y lo apagamos al inicio
        Transform spawnerTransform = transform.parent.Find("Spawner");
        if (spawnerTransform != null) {
            enemySpawner = spawnerTransform.GetComponent<EnemySpawner>();
            spawnerTransform.gameObject.SetActive(false);
        }

        // Ajustamos automáticamente el tamaño del collider del trigger para que cubra la sala
        BoxCollider2D triggerCol = GetComponent<BoxCollider2D>();
        RoomBoundsDetector detector = GetComponentInParent<RoomBoundsDetector>();
        if (triggerCol != null && detector != null) {
            // Un poco más pequeño que el total para que detecte al entrar al área jugable
            triggerCol.size = new Vector2(detector.roomWidth - 4f, detector.roomHeight - 4f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (eventTriggered || roomScript == null || roomScript.isCleared) return;
        if (!collision.CompareTag("Player")) return;

        eventTriggered = true;
        roomScript.playerInside = true;

        // Sala sin spawner: no hay nada que pelear, así que no la bloqueamos
        if (enemySpawner == null) {
            roomScript.UnlockDoors();
            return;
        }

        // 1. Cerrar puertas de esta sala
        roomScript.LockDoors();

        // 2. Avisarnos cuando el spawner termine y activarlo
        enemySpawner.AllEnemiesDefeated += OnRoomCleared;
        enemySpawner.gameObject.SetActive(true);
    }

    void OnRoomCleared() {
        enemySpawner.AllEnemiesDefeated -= OnRoomCleared;
        roomScript.UnlockDoors();
        enemySpawner.gameObject.SetActive(false);
    }

    void OnDestroy() {
        if (enemySpawner != null) enemySpawner.AllEnemiesDefeated -= OnRoomCleared;
    }
}
