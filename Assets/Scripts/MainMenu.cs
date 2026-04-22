using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Load scene by name
    public void PlayGameByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Quit application
    public void QuitGame()
    {
        Debug.Log("QUIT GAME"); // shows in editor
        Application.Quit();
    }
}