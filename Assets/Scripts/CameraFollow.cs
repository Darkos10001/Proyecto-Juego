using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Arrastraremos al Player aquí
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            // Mueve la cámara suavemente hacia la posición del jugador
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }
}