using UnityEngine;

public class RoomBoundsDetector : MonoBehaviour {
    [HideInInspector] public float roomWidth = 26f;
    [HideInInspector] public float roomHeight = 24f;

    void Awake() {
        DetectRoomSize();
    }

    public void DetectRoomSize() {
        Transform leftWall = transform.Find("Wall_Left");
        Transform rightWall = transform.Find("Wall_Right");
        Transform topWall = transform.Find("Wall_Top");
        Transform bottomWall = transform.Find("Wall_Bottom");

        // Si tenemos los muros, calculamos el tamaño total de la habitación midiendo la distancia 
        // absoluta entre los muros opuestos y sumándole el grosor de los muros.
        if (leftWall != null && rightWall != null) {
            float rawWidth = Mathf.Abs(rightWall.position.x - leftWall.position.x);
            // Si el centro de la sala está en 0, el ancho total es el doble de la distancia del centro al muro
            roomWidth = rawWidth; 
        }

        if (topWall != null && bottomWall != null) {
            float rawHeight = Mathf.Abs(topWall.position.y - bottomWall.position.y);
            roomHeight = rawHeight;
        }
    }
}