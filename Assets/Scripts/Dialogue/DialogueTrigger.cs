using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueLine dialogueToStart;
    public GameObject talkPromptUI; // ← arrastra el texto aquí

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            FindObjectOfType<DialogueManager>().StartDialogue(dialogueToStart);
            talkPromptUI.SetActive(false);
            playerInRange = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            talkPromptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            talkPromptUI.SetActive(false);
        }
    }
    public bool IsPlayerStillInRange() => playerInRange;

    public void ShowPrompt()
    {
        if (talkPromptUI != null)
            talkPromptUI.SetActive(true);
    }

}
