using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Estadísticas")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Detección y combate")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public int attackDamage = 15;

    [Header("Patrulla (opcional)")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    private Transform player;
    private NavMeshAgent agent;
    private float lastAttackTime;
    private bool inCombat = false;


    public GameObject arenaPrefab; // Asigna el prefab de la arena
    private DynamicCombatArena activeArena;
    private bool hasStartedCombat = false;


    void Start()
    {
        currentHealth = maxHealth;
        PartyManager partyManager = FindObjectOfType<PartyManager>();
        if (partyManager != null)
        {
            player = partyManager.GetActiveCharacter().transform;
        }

        agent = GetComponent<NavMeshAgent>();
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!hasStartedCombat && distanceToPlayer < detectionRange)
        {
            StartCombatHere();
            inCombat = true;
        }

        if (inCombat)
        {
            if (distanceToPlayer <= attackRange)
            {
                agent.SetDestination(transform.position); // detenerse
                TryAttack();
            }
            else
            {
                agent.SetDestination(player.position);
            }
        }
        else if (patrolPoints.Length > 0)
        {
            Patrol();
        }
    }


    void Patrol()
    {
        if (agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void StartCombatHere()
    {
        hasStartedCombat = true;

        GameObject arenaGO = Instantiate(arenaPrefab, transform.position, Quaternion.identity);
        activeArena = arenaGO.GetComponent<DynamicCombatArena>();
        activeArena.AddEnemy(this);

        Debug.Log("⚔️ Combate iniciado. Arena generada.");
    }


    void TryAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            Debug.Log("Enemy ataca al jugador");

            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} recibió {damage} de daño. Vida restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto.");
        LockOnSystem lockSystem = FindObjectOfType<LockOnSystem>();
        if (lockSystem != null && lockSystem.GetCurrentTarget() == transform)
        {
            lockSystem.UnlockTarget();
        }

        if (activeArena != null)
        {
            activeArena.NotifyEnemyDeath(this);
            Destroy(gameObject); // borra la arena
        }

    }

}
