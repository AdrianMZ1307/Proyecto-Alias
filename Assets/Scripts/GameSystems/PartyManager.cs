using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public ThirdPersonCamera cameraScript; // La cámara externa (CameraHolder)

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
        var newChar = partyMembers[index];

        // Activar el nuevo controlador
        newChar.GetComponent<PlayerController>().enabled = true;

        // Actualizar cámara
        cameraScript.target = newChar.transform;
        cameraScript.cameraPivot = newChar.transform.Find("CameraPivot");

        // Actualizar seguidores
        for (int i = 0; i < partyMembers.Length; i++)
        {
            if (i == index) continue; // Saltamos al personaje activo

            PartyFollower follower = partyMembers[i].GetComponent<PartyFollower>();
            if (follower != null)
            {
                follower.targetToFollow = newChar.transform;
            }

            // Asegúrate de que los controladores no se crucen
            partyMembers[i].GetComponent<PlayerController>().enabled = false;
        }
    }


}
