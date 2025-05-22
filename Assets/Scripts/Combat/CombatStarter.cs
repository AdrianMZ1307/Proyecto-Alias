using UnityEngine;

public class CombatStarter : MonoBehaviour
{
    public CombatArenaManager arenaManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            arenaManager.StartCombat();
            gameObject.SetActive(false); // para que no vuelva a activarse
        }
    }
}
