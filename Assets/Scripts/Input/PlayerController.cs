using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Datos del personaje")]
    public CharacterStats characterStats;

    private Rigidbody rb;

    public Transform cameraTransform;

    [Header("Movimiento")]
    private float moveSpeed = 5f;
    private float runSpeed = 1.5f;
    private Vector3 moveDirection;

    [Header("Salto")]
    public float jumpForce = 7f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;
    private bool isGrounded;

    [Header("Combate")]
    public float attackCooldown = 1f;
    private bool canAttack = true;

    [Header("Rotación estilo tanque")]
    public float turnSpeed = 120f; // grados por segundo


    [Header("Rotación hacia cámara")]
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [Header("Saltos en el aire")]
    private int extraJumps = 1; // 1 = doble salto, 2 = triple salto
    private int jumpsLeft;

    [Header("UI")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float currentRage = 0f;

    private UIManager uiManager;

    public DialogueManager dialogueManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Stat Variables
        moveSpeed = characterStats.moveSpeed;
        runSpeed = characterStats.runSpeed;
        extraJumps = characterStats.extraJumps;

        // Buscamos el UIManager
        uiManager = FindObjectOfType<UIManager>();

        // Al comenzar, actualizamos la UI
        uiManager.UpdateHealthBar(currentHealth, maxHealth);
        uiManager.UpdateCharacterName(characterStats.characterName);
        uiManager.UpdateRageBar(currentRage, 100);
    }

    void Update()
    {
        if (dialogueManager != null && dialogueManager.IsDialogueActive())
            return; // ⛔ Detener todo si hay diálogo

        HandleMovementInput();
        HandleJumpInput();
        HandleAttackInput();
        CheckGround();
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10f); // Quítate 10 de vida
        }
    }

    void HandleMovementInput()
    {
        float vertical = Input.GetAxis("Vertical");   // W y S
        float horizontal = Input.GetAxis("Horizontal"); // A y D

        // Movimiento hacia adelante o atrás
        Vector3 move = transform.forward * vertical;

        // Guardamos la dirección para usar en FixedUpdate
        moveDirection = move.normalized;

        // Rotación con A y D
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            float rotationAmount = horizontal * turnSpeed * Time.deltaTime;
            transform.Rotate(0f, rotationAmount, 0f);
        }
    }


    void FixedUpdate()
    {
        float speed = IsRunning() ? moveSpeed * runSpeed : moveSpeed;
        Vector3 velocity = moveDirection * speed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;

    }
    bool IsRunning()
    {
        return Input.GetKey(KeyCode.LeftShift) && isGrounded;
    }
    void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded || coyoteTimeCounter > 0f)
            {
                Jump();
            }
            else if (jumpsLeft > 0)
            {
                Jump();
                jumpsLeft--;
            }
        }
    }

    void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            Debug.Log("¡ATAQUE!");
            canAttack = false;
            Invoke(nameof(ResetAttack), attackCooldown);

            PerformAttack();
        }
    }
    void PerformAttack()
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;
        float radius = 2.5f;          // Qué tan lejos alcanza el ataque
        float maxAngle = 60f;         // Grados del cono (30° a cada lado)
        int damage = 25;

        // Dibujo visual en la escena (esfera)
        Debug.DrawRay(origin, transform.forward * radius, Color.red, 0.5f);

        // Detectamos todos los colliders cercanos
        Collider[] hitColliders = Physics.OverlapSphere(origin, radius);

        foreach (Collider collider in hitColliders)
        {
            // Revisamos si es un enemigo
            EnemyDummy enemy = collider.GetComponent<EnemyDummy>();
            if (enemy == null)
            {
                EnemyController enemyC = collider.GetComponent<EnemyController>();
                if (enemyC != null)
                {
                    // Dirección desde el jugador al enemigo
                    Vector3 directionToEnemy = (collider.transform.position - origin).normalized;
                    directionToEnemy.y = 0f; // ignorar altura

                    // Ángulo entre el forward del jugador y la dirección al enemigo
                    float angle = Vector3.Angle(transform.forward, directionToEnemy);

                    if (angle <= maxAngle / 2f)
                    {
                        // Está dentro del cono, aplicamos daño
                        enemyC.TakeDamage(damage);
                        if (characterStats.transformUnlocked)
                        {
                            FillRage(5);
                        }
                    }
                    else
                    {
                        Debug.Log($"{collider.name} está cerca, pero fuera del ángulo de ataque");
                    }
                }
            }
            if (enemy != null)
            {
                // Dirección desde el jugador al enemigo
                Vector3 directionToEnemy = (collider.transform.position - origin).normalized;
                directionToEnemy.y = 0f; // ignorar altura

                // Ángulo entre el forward del jugador y la dirección al enemigo
                float angle = Vector3.Angle(transform.forward, directionToEnemy);

                if (angle <= maxAngle / 2f)
                {
                    // Está dentro del cono, aplicamos daño
                    enemy.TakeDamage(damage);
                    if (characterStats.transformUnlocked)
                    {
                        FillRage(5);
                    }
                }
                else
                {
                    Debug.Log($"{collider.name} está cerca, pero fuera del ángulo de ataque");
                }
            }
        }
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    void CheckGround()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.1f, groundMask);

        if (isGrounded)
        {
            // Reset cuando tocamos el suelo
            coyoteTimeCounter = coyoteTime;
            jumpsLeft = extraJumps;
        }
        else
        {
            // Contador de tiempo en el aire
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (groundCheckDistance + 0.1f));
    }
    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        uiManager.UpdateHealthBar(currentHealth, maxHealth);
        if (characterStats.transformUnlocked)
        {
            FillRage(15);
        }
    }

    public void FillRage(float amount)
    {
        currentRage += amount;
        currentRage = Mathf.Clamp(currentRage, 0, 100);
        uiManager.UpdateRageBar(currentRage, 100);
    }
}
