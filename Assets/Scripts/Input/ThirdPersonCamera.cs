using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target; // El jugador
    public Transform cameraPivot; // Un punto detrás del jugador (puede ser hijo del jugador)

    [Header("Distancia y ángulos")]
    public float distance = 4f; // Qué tan lejos está la cámara
    public float height = 2f;   // Qué tan arriba está la cámara
    public float rotationSpeed = 5f; // Suavidad de rotación

    [Header("Rotación libre")]
    public float mouseSensitivity = 2f;
    public float minY = -35f;
    public float maxY = 60f;

    private float currentYaw = 0f; // rotación horizontal
    private float currentPitch = 10f; // rotación vertical

    [Header("Fijado (lock-on suave)")]
    public bool followBehind = false;

    void LateUpdate()
    {
        // Modo de cámara libre (cuando no está fijada)
        if (!Input.GetMouseButton(1))
        {
            // Obtener movimiento del ratón
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Actualizar ángulos
            currentYaw += mouseX;
            currentPitch -= mouseY;
            currentPitch = Mathf.Clamp(currentPitch, minY, maxY);
        }
        else
        {
            // Fijar la cámara detrás del jugador
            Vector3 forwardFlat = target.forward;
            forwardFlat.y = 0f;
            forwardFlat.Normalize();

            currentYaw = Quaternion.LookRotation(forwardFlat).eulerAngles.y;
        }

        // Calcular rotación deseada
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

        // Calcular posición deseada de la cámara
        Vector3 targetPosition = cameraPivot.position - rotation * Vector3.forward * distance + Vector3.up * height;

        // Mover la cámara hacia esa posición
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * rotationSpeed);

        // Mirar siempre al jugador
        transform.LookAt(cameraPivot.position + Vector3.up * 1.5f);
    }
}
