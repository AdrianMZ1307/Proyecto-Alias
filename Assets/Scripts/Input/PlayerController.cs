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

    [Header("Rotación hacia cámara")]
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [Header("Saltos en el aire")]
    private int extraJumps = 1; // 1 = doble salto, 2 = triple salto
    private int jumpsLeft;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Stat Variables
        moveSpeed = characterStats.moveSpeed;
        runSpeed = characterStats.runSpeed;
        extraJumps = characterStats.extraJumps;
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

        Vector3 input = new Vector3(moveX, 0f, moveZ).normalized;

        if (input.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
        else
        {
            moveDirection = Vector3.zero;
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
}
