using UnityEngine;

public class RoomTrigger : MonoBehaviour {
    private Room roomScript;
    private GameObject enemySpawner;
    private bool eventTriggered = false;

    void Start() {
        // Obtenemos el script Room del objeto padre
        roomScript = GetComponentInParent<Room>();

        // Buscamos y apagamos el spawner al inicio
        Transform spawnerTransform = transform.parent.Find("Spawner");
        if (spawnerTransform != null) {
            enemySpawner = spawnerTransform.gameObject;
            enemySpawner.SetActive(false);
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
        if (collision.CompareTag("Player") && !eventTriggered && roomScript != null && !roomScript.isCleared) {
            eventTriggered = true;
            roomScript.playerInside = true;

            // 1. Cerrar puertas de esta sala
            roomScript.LockDoors();

            // 2. Activar el spawner de enemigos
            if (enemySpawner != null) {
                enemySpawner.SetActive(true);
            }
        }
    }

    void Update() {
        if (roomScript != null && roomScript.playerInside && !roomScript.isCleared) {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0 && enemySpawner != null && !enemySpawner.activeSelf) {
                roomScript.UnlockDoors();
            }
        }
    }
}