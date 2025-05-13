using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Referencia al componente Rigidbody del jugador (para moverlo con física)
    private Rigidbody rb;

    // Velocidad de movimiento del personaje
    [Header("Movimiento")]
    public float moveSpeed = 5f;

    // Fuerza con la que el jugador salta
    [Header("Salto")]
    public float jumpForce = 7f;

    // Para saber si el jugador está tocando el suelo
    private bool isGrounded = false;

    // Detección de suelo: esto es para que el jugador no salte en el aire
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    // Golpe: cooldown entre ataques
    [Header("Combate")]
    public float attackCooldown = 1f;
    private bool canAttack = true;

    void Start()
    {
        // Obtenemos el Rigidbody para aplicarle fuerzas o moverlo
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Movimiento del personaje
        Move();

        // Salto cuando presionamos espacio
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Golpe cuando presionamos clic izquierdo
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            Attack();
        }

        // Revisar si estamos en el suelo cada frame
        CheckGround();
    }

    void Move()
    {
        // Tomamos la entrada de teclas (WASD o flechas)
        float moveX = Input.GetAxis("Horizontal"); // A/D o ←/→
        float moveZ = Input.GetAxis("Vertical");   // W/S o ↑/↓

        // Creamos un vector con esa dirección
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Mantenemos la velocidad vertical (por si estamos cayendo o saltando)
        Vector3 velocity = move * moveSpeed;
        velocity.y = rb.velocity.y;

        // Aplicamos la velocidad al Rigidbody
        rb.velocity = velocity;
    }

    void Jump()
    {
        // Aplicamos una fuerza hacia arriba
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void Attack()
    {
        // Aquí iría la animación o el sistema de combate (de momento, solo mostramos un mensaje)
        Debug.Log("¡ATAQUE!");

        // Ponemos el cooldown para evitar spamear el ataque
        canAttack = false;
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void ResetAttack()
    {
        // Permitimos atacar de nuevo después del cooldown
        canAttack = true;
    }

    void CheckGround()
    {
        // Usamos un SphereCast para verificar si estamos tocando el suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    // Esto dibuja un gizmo en el editor para ver dónde se revisa el suelo
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
