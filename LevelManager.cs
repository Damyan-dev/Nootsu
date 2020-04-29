using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public int maxScore, collectableScoreValue;
    [HideInInspector]
    public int score, bonusScore, finalScore, collectables, scoredCollectables, trackers;
    public bool won = false;
    public int scoreCountDivisions;
    public float timeBetweenScore, timeBetweenBonus, timeAfterBonus;
    private int scoreDisplay = 0, bonusDisplay = 0, i = 0;
    public GameObject scoreGO, bonusGO, finalScoreGO;
    private bool scoreAdded, bonusAdded, adding;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        bonusScore = 0;
        collectables = 0;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!won)
        {
            score = Mathf.FloorToInt((Time.timeSinceLevelLoad + maxScore) / Mathf.Pow(((500f / maxScore) * Time.timeSinceLevelLoad) + 1.0f, 2));
        }
        else
        {
            Win();
        }
    }

    void Win()
    {
        bonusScore = collectables * collectableScoreValue;
        finalScore = score + bonusScore;
        if (PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "Score", finalScore) <= finalScore)
        {
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "Score", finalScore);
        }
        if (i < scoreCountDivisions && !adding && !scoreAdded)
        {
            StartCoroutine(AddScore());
        }
        else if (i == scoreCountDivisions)
        {
            scoreGO.GetComponent<Text>().text = "Score : " + score;
            scoreAdded = true;
            i = 0;
        }
        else if (i < collectables && !adding && !bonusAdded && scoreAdded)
        {
            StartCoroutine(AddBonus());
        }
        else if (bonusAdded)
        {
            StartCoroutine(AddBonus());
        }
        else if (i == collectables && scoreAdded && !adding)
        {
            bonusGO.GetComponent<Text>().text = "Bonus : " + bonusDisplay;
            bonusAdded = true;
            i = 0;
        }
    }

    private IEnumerator AddScore()
    {
        adding = true;
        yield return new WaitForSeconds(timeBetweenScore);
        scoreDisplay += Mathf.FloorToInt(score / scoreCountDivisions);
        scoreGO.GetComponent<Text>().text = "Score : " + scoreDisplay;
        adding = false;
        i++;
    }

    private IEnumerator AddBonus()
    {
        if (bonusAdded)
        {
            adding = true;
            yield return new WaitForSeconds(timeAfterBonus);
            finalScoreGO.GetComponent<Text>().text = "Final Score : " + finalScore;
        }
        else
        {
            adding = true;
            yield return new WaitForSeconds(timeBetweenBonus);
            bonusDisplay += collectableScoreValue;
            bonusGO.GetComponent<Text>().text = "Bonus : " + bonusDisplay;
            adding = false;
            i++;
        }
    }
}
