using UnityEngine;
using UnityEngine.SceneManagement; // Essential for switching scenes

public class MenuController : MonoBehaviour
{
    // Call this from the Intro Scene button
    public void LoadTitleScreen() {
        SceneManager.LoadScene("TitleScene");
    }

    public void LoadGameScreen() {
        SceneManager.LoadScene("GameScene");
    }
    public void LoadLearningScreen() {
        SceneManager.LoadScene("LearningScene");
    }
    public void LoadEndScreen() {
        SceneManager.LoadScene("EndScene");
    }

    // Call this from the Title Screen "Play" button
    public void IncrementScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame() {
        Application.Quit();
        Debug.Log("Game Exited"); // Only visible in the editor
    }
}