using UnityEngine;

public class PartyFollower : MonoBehaviour
{
    [Header("Configuración del seguimiento")]
    public Transform targetToFollow; // El personaje activo (el jugador actual)
    public float followDistance = 2.5f; // Distancia mínima antes de moverse
    public float moveSpeed = 5f; // Velocidad de seguimiento

    [Header("Evitar rotación loca")]
    public bool rotateTowardsTarget = true;
    public float rotationSpeed = 8f;

    private Rigidbody rb;

    void Start()
    {
        // Obtener el Rigidbody de este personaje (para moverlo con física)
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Que no se caiga al rotar
    }

    void FixedUpdate()
    {
        if (targetToFollow == null) return;

        // Calcular la distancia entre este personaje y el jugador actual
        float distance = Vector3.Distance(transform.position, targetToFollow.position);

        // Si está demasiado lejos, moverse hacia el jugador
        if (distance > followDistance)
        {
            // Dirección horizontal hacia el jugador
            Vector3 direction = (targetToFollow.position - transform.position).normalized;
            direction.y = 0f; // No subimos ni bajamos

            // Calculamos velocidad, pero conservamos la componente Y (por si está cayendo/saltando)
            Vector3 move = direction * moveSpeed;
            Vector3 finalVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
            rb.linearVelocity = finalVelocity;

            // Gira suavemente hacia la dirección del movimiento
            if (rotateTowardsTarget && direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }
        else
        {
            // Si está cerca, detenemos el movimiento horizontal pero NO tocamos Y (gravedad sigue funcionando)
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }
}
