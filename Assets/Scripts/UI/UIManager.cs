using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Image healthBarFill;
    public Image rageBarFill;
    public TextMeshProUGUI characterNameText;

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        float fillAmount = currentHealth / maxHealth;
        Debug.Log($"Barra de vida: {fillAmount}");
        healthBarFill.fillAmount = fillAmount;
    }
    public void UpdateRageBar(float currentRage, float maxRage)
    {
        float fillAmount = currentRage / maxRage;
        Debug.Log($"Barra de ira: {fillAmount}");
        rageBarFill.fillAmount = fillAmount;
    }

    public void UpdateCharacterName(string characterName)
    {
        characterNameText.text = characterName;
    }
}
