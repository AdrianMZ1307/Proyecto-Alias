using UnityEngine;

public class PartyManager : MonoBehaviour
{
    //public ThirdPersonCamera cameraScript; 
    public SimpleFollowCamera cameraScript;

    [Header("Personajes del grupo")]
    public GameObject[] partyMembers; // Aquí arrastras Alias, Vanessa, Arthur
    private int currentIndex = 0;

    void Start()
    {
        ActivateCharacter(currentIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeCharacter(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeCharacter(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeCharacter(2);
    }

    void ChangeCharacter(int index)
    {
        if (index == currentIndex || index >= partyMembers.Length) return;

        // Verifica si el nuevo personaje está caído
        var pc = partyMembers[index].GetComponent<PlayerController>();
        if (pc != null && pc.isDown)
        {
            Debug.Log("❌ Ese personaje está fuera de combate.");
            return;
        }

        // Desactivar actual
        partyMembers[currentIndex].GetComponent<PlayerController>().enabled = false;

        // Activar el nuevo
        currentIndex = index;
        ActivateCharacter(currentIndex);
    }


    void ActivateCharacter(int index)
    {
        GameObject newChar = partyMembers[index];

        // Activar controlador del nuevo personaje
        newChar.GetComponent<PlayerController>().enabled = true;
        PartyFollower thisFollower = newChar.GetComponent<PartyFollower>();
        if (thisFollower != null)
        {
            thisFollower.enabled = false;
        }
        // Actualizar cámara si la usas
        cameraScript.target = newChar.transform;

        // Desactivar los demás personajes + asignar target a seguidores
        for (int i = 0; i < partyMembers.Length; i++)
        {
            if (i == index) continue;


            // Desactiva su controlador
            partyMembers[i].GetComponent<PlayerController>().enabled = false;

            // Asigna a quien seguir
            PartyFollower follower = partyMembers[i].GetComponent<PartyFollower>();

            if (follower != null)
            {
                follower.enabled = true; // ¡Actívalo!
                follower.targetToFollow = newChar.transform;
            }
        }

        // ACTUALIZAR UI
        UIManager ui = FindObjectOfType<UIManager>();
        PlayerController newController = newChar.GetComponent<PlayerController>();

        if (ui != null && newController != null)
        {
            ui.UpdateCharacterName(newController.characterStats.characterName);
            ui.UpdateHealthBar(newController.currentHealth, newController.maxHealth);
        }

        // Actualizar índice actual
        currentIndex = index;
    }
    public GameObject GetActiveCharacter()
    {
        return partyMembers[currentIndex];
    }
    public void CheckPartyStatus()
    {
        int alive = 0;

        foreach (GameObject member in partyMembers)
        {
            PlayerController pc = member.GetComponent<PlayerController>();
            if (pc != null && !pc.isDown)
            {
                alive++;
            }
        }

        if (alive == 0)
        {
            Debug.Log("💀 Todos los personajes están fuera de combate. GAME OVER.");
            // Aquí va lógica de Game Over: recarga escena, menú, etc.
        }
    }
    public void SwitchToNextAlive()
    {
        for (int i = 0; i < partyMembers.Length; i++)
        {
            if (i == currentIndex) continue; // no elegir al que acaba de morir

            PlayerController pc = partyMembers[i].GetComponent<PlayerController>();
            if (pc != null && !pc.isDown)
            {
                ChangeCharacter(i); // usa tu función ya existente
                Debug.Log($"🔁 Cambio automático a: {pc.characterStats.characterName}");
                return;
            }
        }

        // Si llegamos aquí, no queda nadie vivo (pero GameOver ya lo maneja CheckPartyStatus)
        Debug.Log("⚠️ No hay personajes vivos a los que cambiar.");
    }
}
