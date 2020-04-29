using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    public void EnterGame()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("currentLevel", 1));
    }

    public void ExitGame()
    {
        Debug.Log("Quit.");
        Application.Quit();
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
