using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public GameObject choicesPanel;
    public GameObject choiceButtonPrefab;

    private DialogueLine currentDialogue;


    private string[] currentLines;
    private int currentIndex;
    private bool isDialogueActive = false;

    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.E))
        {
            ShowNextLine();
        }
    }

    public void StartDialogue(DialogueLine dialogue)
    {
        currentDialogue = dialogue;
        currentLines = dialogue.lines;
        currentIndex = 0;
        isDialogueActive = true;

        speakerNameText.text = dialogue.speakerName;
        dialoguePanel.SetActive(true);

        if (currentLines.Length > 0)
            dialogueText.text = currentLines[currentIndex];
        else if (dialogue.hasChoices)
            ShowChoices();
    }





    void ShowNextLine()
    {
        currentIndex++;

        if (currentIndex >= currentLines.Length)
        {
            // Si hay elecciones al final
            if (currentDialogue.hasChoices)
            {
                ShowChoices();
                return;
            }

            EndDialogue();
            return;
        }

        dialogueText.text = currentLines[currentIndex];
    }

    void ShowChoices()
    {
        choicesPanel.SetActive(true);

        // Limpiar opciones anteriores
        foreach (Transform child in choicesPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Crear botón por opción
        foreach (DialogueOption option in currentDialogue.options)
        {
            GameObject btn = Instantiate(choiceButtonPrefab, choicesPanel.transform);
            //btn.GetComponent<Image>().gameObject.SetActive(true);
            //btn.GetComponent<Button>().gameObject.SetActive(true);
            btn.GetComponentInChildren<UnityEngine.UI.Button>(true).gameObject.SetActive(true);
            btn.GetComponentInChildren<UnityEngine.UI.Image>(true).gameObject.SetActive(true);
            btn.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.SetActive(true);

            btn.GetComponentInChildren<TextMeshProUGUI>().text = option.optionText;

            btn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                choicesPanel.SetActive(false);
                StartDialogue(option.nextDialogue);
            });
        }
    }


    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false;
        DialogueTrigger currentTrigger = FindObjectOfType<DialogueTrigger>();
        if (currentTrigger != null && currentTrigger.IsPlayerStillInRange())
        {
            currentTrigger.ShowPrompt(); // Método que tú creas para reactivar el texto
        }
    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }

}
