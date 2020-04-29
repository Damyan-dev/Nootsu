using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    //Through the Inspector, you drag and drop the Menu Panel into the newly created..
    //..pauseScreen object.
    public GameObject pauseScreen, playScreen;
    //Adds a boolean variable that is later used for the PauseGame and ResumeGame methods.
    public static bool LevelIsStopped = false;

    void Update()
    {
        //Simple statement that allows the player to access the menu when pressing the Escape key.
        //It further breaks down in executing methods that are written below.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (LevelIsStopped)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }

        }
    }

    void Start()
    {

        Cursor.visible = true;
    }

    //A method that checks if pauseScreen is unlisted.
    public void ResumeGame()
    {

        Cursor.visible = false;
        playScreen.SetActive(true);
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;
        LevelIsStopped = false;
    }

    //A method that checks if pauseScreen is listed.
    void PauseGame()
    {
        playScreen.SetActive(false);
        pauseScreen.SetActive(true);
        //Freezes the game.
        Time.timeScale = 0f;
        LevelIsStopped = true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    //Once the Restart button in the menu is pressed, it reloads
    //the timer, sets the Time Scale to 1 and reloads the game.
    public void restartGame()
    {
        playScreen.SetActive(true);
        pauseScreen.gameObject.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    //Changes the scene to the main menu once the back button is pressed.
    public void goToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
