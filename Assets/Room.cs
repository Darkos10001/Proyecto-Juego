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
        // Al crear la sala, las puertas inician abiertas (desactivadas) para dejar pasar al jugador
        if (doorTop != null) doorTop.SetActive(false);
        if (doorBottom != null) doorBottom.SetActive(false);
        if (doorLeft != null) doorLeft.SetActive(false);
        if (doorRight != null) doorRight.SetActive(false);
    }

    public void LockDoors() {
        if (doorTop != null) doorTop.SetActive(true);
        if (doorBottom != null) doorBottom.SetActive(true);
        if (doorLeft != null) doorLeft.SetActive(true);
        if (doorRight != null) doorRight.SetActive(true);
        
        Debug.Log($"[{gameObject.name}] Puertas bloqueadas.");
    }

    public void UnlockDoors() {
        if (doorTop != null) doorTop.SetActive(false);
        if (doorBottom != null) doorBottom.SetActive(false);
        if (doorLeft != null) doorLeft.SetActive(false);
        if (doorRight != null) doorRight.SetActive(false);
        
        isCleared = true;
        Debug.Log($"[{gameObject.name}] ¡Sala limpia! Puertas abiertas.");
    }
}