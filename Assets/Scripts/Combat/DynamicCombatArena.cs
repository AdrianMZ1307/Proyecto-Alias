using UnityEngine;
using System.Collections.Generic;

public class DynamicCombatArena : MonoBehaviour
{
    public List<EnemyController> enemiesInArena = new List<EnemyController>();
    public float arenaRadius = 10f;
    public GameObject boundaryVisual; // opcional: efecto o círculo visible

    void Start()
    {
        if (boundaryVisual != null)
            boundaryVisual.SetActive(true);

        // Restringe al jugador dentro del área (opcional: movimiento limitado)
    }

    public void AddEnemy(EnemyController enemy)
    {
        if (!enemiesInArena.Contains(enemy))
            enemiesInArena.Add(enemy);
    }

    public void NotifyEnemyDeath(EnemyController enemy)
    {
        enemiesInArena.Remove(enemy);

        if (enemiesInArena.Count == 0)
        {
            Debug.Log("✔️ Todos los enemigos han sido derrotados. Arena eliminada.");
            Destroy(gameObject); // borra la arena
        }
    }
}
