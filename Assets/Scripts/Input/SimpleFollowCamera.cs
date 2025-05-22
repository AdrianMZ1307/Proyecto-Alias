using UnityEngine;

public class SimpleFollowCamera : MonoBehaviour
{
    public Transform target; // El jugador
    public Vector3 offset = new Vector3(0f, 2f, -5f); // Posición detrás y arriba
    public float followSpeed = 5f;
    private LockOnSystem lockOn;


    void LateUpdate()
    {
        if (target == null) return;

        if (lockOn == null)
            lockOn = FindObjectOfType<LockOnSystem>();

        // Posición deseada: detrás del jugador con desplazamiento
        Vector3 desiredPosition = target.position + target.rotation * offset;

        // Suavizamos el seguimiento
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);

        // 🔄 CAMBIO: mirar al enemigo si hay lock-on, si no al jugador
        if (lockOn != null && lockOn.IsLockedOn() && lockOn.GetCurrentTarget() != null)
        {
            transform.LookAt(lockOn.GetCurrentTarget().position + Vector3.up * 1.5f);
        }
        else
        {
            transform.LookAt(target.position + Vector3.up * 1.5f);
        }
    }

}
