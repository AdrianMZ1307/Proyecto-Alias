using UnityEngine;
using System.Collections.Generic;

public class CombatArenaManager : MonoBehaviour
{
    [Header("Límites de la arena")]
    public GameObject arenaBoundary; // ejemplo: un muro o círculo visual
    public Collider arenaCollider; // para limitar la salida

    [Header("Enemigos en esta arena")]
    public List<EnemyController> enemies = new List<EnemyController>();

    private bool combatActive = false;

    void Start()
    {
        if (arenaBoundary != null)
            arenaBoundary.SetActive(false);

        if (arenaCollider != null)
            arenaCollider.enabled = false;
    }

    public void StartCombat()
    {
        combatActive = true;

        if (arenaBoundary != null)
            arenaBoundary.SetActive(true);

        if (arenaCollider != null)
            arenaCollider.enabled = true;

        Debug.Log("🔒 Arena de combate activada");
    }

    public void CheckCombatEnd()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null) return; // aún hay enemigos vivos
        }

        EndCombat();
    }

    public void EndCombat()
    {
        combatActive = false;

        if (arenaBoundary != null)
            arenaBoundary.SetActive(false);

        if (arenaCollider != null)
            arenaCollider.enabled = false;

        Debug.Log("✅ Arena completada");
        Destroy(gameObject); // opcional: elimina el objeto de la arena
    }
}
