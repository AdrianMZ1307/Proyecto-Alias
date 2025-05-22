using UnityEngine;
using System.Collections.Generic;

public class LockOnSystem : MonoBehaviour
{
    public float lockRange = 15f;
    public KeyCode toggleKey = KeyCode.Tab;

    private Transform currentTarget;
    private bool isLocked = false;
    private Transform player;
    private Camera cam;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (isLocked)
            {
                UnlockTarget();
            }
            else
            {
                FindNearestTarget();
            }
        }

        if (isLocked && currentTarget != null)
        {
            // Gira al jugador hacia el objetivo
            Vector3 direction = currentTarget.position - player.position;
            direction.y = 0;
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                player.rotation = Quaternion.Slerp(player.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }

    void FindNearestTarget()
    {
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        float closestDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (EnemyController enemy in enemies)
        {
            float dist = Vector3.Distance(player.position, enemy.transform.position);
            if (dist < closestDistance && dist <= lockRange)
            {
                closestDistance = dist;
                closest = enemy.transform;
            }
        }

        if (closest != null)
        {
            currentTarget = closest;
            isLocked = true;
            Debug.Log("🎯 Enemigo fijado: " + closest.name);
        }
    }

    public void UnlockTarget()
    {
        Debug.Log("🔓 Enemigo liberado");
        currentTarget = null;
        isLocked = false;
    }

    public Transform GetCurrentTarget()
    {
        return currentTarget;
    }

    public bool IsLockedOn()
    {
        return isLocked;
    }

}
