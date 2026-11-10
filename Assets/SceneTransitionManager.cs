using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour {
    public static SceneTransitionManager Instance;

    [Header("Configuración de Escenas")]
    [SerializeField] private string combatSceneName = "CombatScene3D"; // Nombre de tu escena 3D en Build Settings

    private string currentActiveRoomName;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    // Método que llamaremos desde el RoomTrigger al entrar a una sala
    public void EnterCombatRoom(string roomIdentifier) {
        currentActiveRoomName = roomIdentifier;
        
        // Guardamos la posición actual del jugador lógicamente si lo deseas, 
        // o simplemente cargamos la escena de combate de forma aditiva o directa:
        Debug.Log($"Entrando a combate desde la sala: {roomIdentifier}");
        
        // Cargamos la escena de combate en primera persona
        SceneManager.LoadScene(combatSceneName);
    }

    // Método para regresar al mapa 2D al ganar el combate
    public void ReturnToMap(string mapSceneName = "SampleScene") {
        SceneManager.LoadScene(mapSceneName);
        // Aquí podrías marcar la sala como "Cleared" usando el identificador
    }
}