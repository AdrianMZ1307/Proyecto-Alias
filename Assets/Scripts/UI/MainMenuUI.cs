using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public string firstSceneName = "EscenaPrueba"; // ← Ajusta este nombre a la que ya tienes

    public void PlayGame()
    {
        SceneManager.LoadScene(firstSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("🔴 Cerrando juego...");
        Application.Quit();
    }
}
