using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinObject : MonoBehaviour
{
    public GameObject play, win;
    private AudioManager audioManager;
    private LevelManager levelManager;

    private void Start()
    {
        audioManager = GameObject.Find("LevelManager").GetComponent<AudioManager>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            audioManager.PlaySoundComplete("Win");
            Win();
        }
    }

    private void Win()
    {
        play.SetActive(false);
        win.SetActive(true);
        Cursor.visible = true;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("EnemyA");
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<EnemyController>().enabled = false;
        }
        GameObject[] flyingEnemies = GameObject.FindGameObjectsWithTag("EnemyB");
        for (int i = 0; i < flyingEnemies.Length; i++)
        {
            flyingEnemies[i].GetComponent<FlyingEnemyController>().enabled = false;
        }
        GameObject.Find("Player").GetComponent<Controller>().enabled = false;
        GameObject.Find("Animator").GetComponent<Animations>().state = "winning";
        GameObject.Find("Player").GetComponent<RopeControl>().enabled = false;
        levelManager.won = true;
        audioManager.StopAllSounds();
        PlayerPrefs.SetInt("currentLevel", int.Parse(Regex.Replace(SceneManager.GetActiveScene().name, "[^0-9]", "")));

        enabled = false;
    }
}
