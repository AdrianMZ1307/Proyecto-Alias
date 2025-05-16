using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Line")]
[System.Serializable]
public class DialogueLine : ScriptableObject
{
    public string speakerName;
    [TextArea(3, 10)]
    public string[] lines;
    public bool hasChoices;
    public DialogueOption[] options;
}
