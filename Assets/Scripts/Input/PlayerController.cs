using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    // NUEVA variable pública para referencia a la cámara
    public Transform cameraTransform;

    [Header("Movimiento")]
    public float moveSpeed = 5f;
    private Vector3 inputDirection;

    [Header("Salto")]
    public float jumpForce = 7f;
    public float groundCheckDistance = 0.2f; // Distancia para el raycast
    public LayerMask groundMask;
    private bool isGrounded;

    [Header("Combate")]
    public float attackCooldown = 1f;
    private bool canAttack = true;


    // Variables para rotación suave
    [Header("Rotación hacia cámara")]
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Evita que se caiga al girar
    }

    void Update()
    {
        HandleMovementInput();
        HandleJumpInput();
        HandleAttackInput();
        CheckGround();
    }

    void HandleMovementInput()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Debug.Log($"Input: {moveX}, {moveZ}");

        // Dirección de entrada del jugador (plano XZ)
        Vector3 input = new Vector3(moveX, 0f, moveZ).normalized;

        // Solo procesamos si hay input
        if (input.magnitude >= 0.1f)
        {
            // Dirección relativa a la cámara
            float targetAngle = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            // Suavizar la rotación (para no girar instantáneamente)
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            // Aplicar rotación al personaje
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Calcular dirección en la que caminar
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // Movimiento
            Vector3 velocity = moveDir.normalized * moveSpeed;
            velocity.y = rb.linearVelocity.y;
            rb.linearVelocity = velocity;
        }
        else
        {
            // Si no hay input, solo mantenemos la velocidad vertical (caída, etc.)
            Vector3 stop = rb.linearVelocity;
            stop.x = 0f;
            stop.z = 0f;
            rb.linearVelocity = stop;
        }
    }


    void FixedUpdate()
    {
        // Movimiento continuo con Rigidbody
        Vector3 moveVelocity = inputDirection * moveSpeed;
        moveVelocity.y = rb.linearVelocity.y; // mantenemos la velocidad vertical
        rb.linearVelocity = moveVelocity;
    }

    void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            Debug.Log("¡ATAQUE!");
            canAttack = false;
            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    void CheckGround()
    {
        // Disparamos un rayo desde el centro del jugador hacia abajo
        Ray ray = new Ray(transform.position, Vector3.down);

        // Si toca algo a una pequeña distancia (como el suelo), estamos en el suelo
        isGrounded = Physics.Raycast(ray, groundCheckDistance + 0.1f, groundMask);
    }

    void OnDrawGizmosSelected()
    {
        // Dibuja el rayo en el editor para ver la detección del suelo
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (groundCheckDistance + 0.1f));
    }
}
