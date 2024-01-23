using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public void WinGame()
    {
        // You can display a victory UI or other relevant information here

        // Close the game after a delay (optional)
        Invoke("QuitGame", 2f);
    }

    public void LoseGame()
    {
        // You can display a defeat UI or other relevant information here

        // Close the game after a delay (optional)
        Invoke("QuitGame", 2f);
    }

    public void QuitGame()
    {
        // Quit the game (works in standalone builds, not in the Unity Editor)
        Application.Quit();
    }

    public void RestartGame()
    {
        // Restart the game by reloading the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
