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

    /// <summary>
    /// Abre un hueco en la pared de cada lado que tenga una sala vecina y crea ahí una puerta.
    /// Los lados sin vecino quedan como pared sólida. DungeonGenerator lo llama al crear la sala.
    /// </summary>
    public void BuildDoorways(bool top, bool bottom, bool left, bool right, float doorWidth, Color doorColor) {
        // Las puertas que venían en el prefab se reemplazan por las generadas aquí
        RemoveOldDoor(ref doorTop);
        RemoveOldDoor(ref doorBottom);
        RemoveOldDoor(ref doorLeft);
        RemoveOldDoor(ref doorRight);

        if (top)    doorTop    = CreateDoorway("Wall_Top",    true,  doorWidth, doorColor);
        if (bottom) doorBottom = CreateDoorway("Wall_Bottom", true,  doorWidth, doorColor);
        if (left)   doorLeft   = CreateDoorway("Wall_Left",   false, doorWidth, doorColor);
        if (right)  doorRight  = CreateDoorway("Wall_Right",  false, doorWidth, doorColor);
    }

    void RemoveOldDoor(ref GameObject door) {
        if (door != null) Destroy(door);
        door = null;
    }

    GameObject CreateDoorway(string wallName, bool horizontal, float doorWidth, Color doorColor) {
        Transform wall = transform.Find(wallName);
        if (wall == null) {
            Debug.LogWarning($"[Room] No encontré {wallName} en {name}; no se pudo abrir el paso.");
            return null;
        }

        // Largo y centro de la pared a lo largo de su eje (X para Top/Bottom, Y para Left/Right)
        float length = horizontal ? wall.localScale.x : wall.localScale.y;
        float center = horizontal ? wall.localPosition.x : wall.localPosition.y;

        float gap = Mathf.Min(doorWidth, length - 1f);
        float segmentLength = (length - gap) / 2f;
        float offset = gap / 2f + segmentLength / 2f;

        // Copias creadas ANTES de modificar la pared original
        Transform wallB = Instantiate(wall.gameObject, transform).transform;
        wallB.name = wallName + "_B";

        Transform door = Instantiate(wall.gameObject, transform).transform;
        door.name = wallName.Replace("Wall", "Door") + "_Auto";

        // La pared se parte en dos trozos con el hueco en medio
        SetSegment(wall, horizontal, center - offset, segmentLength);
        SetSegment(wallB, horizontal, center + offset, segmentLength);

        // La puerta rellena el hueco (se activa para bloquear, se desactiva para abrir)
        SetSegment(door, horizontal, center, gap);
        SpriteRenderer sr = door.GetComponent<SpriteRenderer>();
        if (sr != null) {
            sr.color = doorColor;
            sr.sortingOrder += 1;
        }
        door.gameObject.SetActive(false);

        return door.gameObject;
    }

    void SetSegment(Transform t, bool horizontal, float newCenter, float newLength) {
        Vector3 p = t.localPosition;
        Vector3 s = t.localScale;
        if (horizontal) { p.x = newCenter; s.x = newLength; }
        else            { p.y = newCenter; s.y = newLength; }
        t.localPosition = p;
        t.localScale = s;
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
