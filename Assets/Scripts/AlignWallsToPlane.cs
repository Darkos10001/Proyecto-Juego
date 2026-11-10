using UnityEngine;

[ExecuteInEditMode] // Esto hace que se ajuste también mientras lo editas en el editor (opcional)
public class AlignWallsToPlane : MonoBehaviour {
    [Header("Referencias de las Paredes")]
    public Transform wallTop;
    public Transform wallBottom;
    public Transform wallLeft;
    public Transform wallRight;

    [Header("El Plano de la Arena")]
    public MeshRenderer planeRenderer; // Arrastra aquí el MeshRenderer del Plane

    void Start() {
        AjustarParedes();
    }

    public void AjustarParedes() {
        if (planeRenderer == null) return;

        // Obtenemos el tamaño real del plano en el mundo (en Unity, un Plane por defecto mide 10x10 unidades en escala 1)
        Bounds planeBounds = planeRenderer.bounds;
        
        float minX = planeBounds.min.x;
        float maxX = planeBounds.max.x;
        float minZ = planeBounds.min.z;
        float maxZ = planeBounds.max.z;

        float centerX = planeBounds.center.x;
        float centerZ = planeBounds.center.z;

        float widthX = planeBounds.size.x;
        float depthZ = planeBounds.size.z;

        // Posicionar y escalar las paredes para que calcquen exactamente con los bordes
        if (wallTop != null) {
            wallTop.position = new Vector3(centerX, wallTop.position.y, maxZ);
            wallTop.localScale = new Vector3(widthX, wallTop.localScale.y, wallTop.localScale.z);
        }

        if (wallBottom != null) {
            wallBottom.position = new Vector3(centerX, wallBottom.position.y, minZ);
            wallBottom.localScale = new Vector3(widthX, wallBottom.localScale.y, wallBottom.localScale.z);
        }

        if (wallRight != null) {
            wallRight.position = new Vector3(maxX, wallRight.position.y, centerZ);
            wallRight.localScale = new Vector3(wallRight.localScale.x, wallRight.localScale.y, depthZ);
        }

        if (wallLeft != null) {
            wallLeft.position = new Vector3(minX, wallLeft.position.y, centerZ);
            wallLeft.localScale = new Vector3(wallLeft.localScale.x, wallLeft.localScale.y, depthZ);
        }

        Debug.Log("¡Paredes ajustadas asimétricamente al tamaño del plano!");
    }
}