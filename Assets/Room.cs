using UnityEngine;

public class Room : MonoBehaviour {
    [Header("Referencias a las Puertas")]
    public GameObject doorTop;
    public GameObject doorBottom;
    public GameObject doorLeft;
    public GameObject doorRight;

    [Header("Estado de la Sala")]
    public bool isCleared = false;
    [HideInInspector] public bool playerInside = false;

    void Start() {
        // Al iniciar la sala, las puertas se abren (desactivan) para dejar pasar al jugador
        SetDoorsActive(false);
    }

    public void LockDoors() {
        SetDoorsActive(true);
        Debug.Log($"[{gameObject.name}] Puertas bloqueadas (Cerradas).");
    }

    public void UnlockDoors() {
        SetDoorsActive(false);
        isCleared = true;
        Debug.Log($"[{gameObject.name}] ¡Sala limpia! Puertas abiertas.");
    }

    private void SetDoorsActive(bool state) {
        if (doorTop != null) doorTop.SetActive(state);
        if (doorBottom != null) doorBottom.SetActive(state);
        if (doorLeft != null) doorLeft.SetActive(state);
        if (doorRight != null) doorRight.SetActive(state);
    }
}