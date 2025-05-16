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

        // Desactivar el personaje actual
        partyMembers[currentIndex].GetComponent<PlayerController>().enabled = false;
        //partyMembers[currentIndex].GetComponentInChildren<Camera>().gameObject.SetActive(false);

        // Activar el nuevo personaje
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


}
