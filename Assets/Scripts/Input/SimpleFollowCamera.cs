using UnityEngine;

public class SimpleFollowCamera : MonoBehaviour
{
    public Transform target; // El jugador
    public Vector3 offset = new Vector3(0f, 2f, -5f); // Posición detrás y arriba
    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        // Posición deseada: detrás del jugador con desplazamiento
        Vector3 desiredPosition = target.position + target.rotation * offset;

        // Suavizamos el seguimiento
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);

        // Siempre mira al jugador
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
