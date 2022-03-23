using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class HighScores : MonoBehaviour
{
    public GameObject[] scoreBoxes;
    private List<int> highScores;
    private int newScore, replacementPosition, highScoreCounter = 0;
    private bool moreScores = true;

    // Start is called before the first frame update
    void Start()
    {
        GetHighScores();
        newScore = PlayerPrefs.GetInt("Level1Score", 0) + PlayerPrefs.GetInt("Level2Score", 0) + PlayerPrefs.GetInt("Level3Score", 0);
        replacementPosition = highScores.Count();
        for (int i = highScores.Count - 1; i >= 0; i--)
        {
            if (newScore > highScores[i])
            {
                replacementPosition = i;
            }
        }
        if (replacementPosition == highScores.Count)
        {
            PlayerPrefs.SetInt("highScore" + highScores.Count.ToString(), newScore);
        }
        else if (highScores.Count == 0)
        {
            PlayerPrefs.SetInt("highScore0", newScore);
        }
        else
        {
            for (int i = highScores.Count; i > replacementPosition ; i--)
            {
                PlayerPrefs.SetInt("highScore" + i.ToString(), PlayerPrefs.GetInt("highScore" + (i - 1).ToString()));
            }
            PlayerPrefs.SetInt("highScore" + replacementPosition.ToString(), newScore);
        }
        GetHighScores();
        for (int i = 0; i < scoreBoxes.Length; i++)
        {
            if (highScores.Count > i)
            {
                scoreBoxes[i].GetComponent<Text>().text = (i + 1).ToString() + " : " + highScores[i].ToString();
            }
            else
            {
                scoreBoxes[i].GetComponent<Text>().text = (i + 1).ToString() + " : No Value Yet";
            }
        }
    }

    private void GetHighScores()
    {
        highScores = new List<int>();
        highScoreCounter = 0;
        while (moreScores)
        {
            if (PlayerPrefs.GetInt("highScore" + highScoreCounter.ToString(), 0) != 0)
            {
                highScores.Add(PlayerPrefs.GetInt("highScore" + highScoreCounter.ToString(), 0));
                highScoreCounter++;
            }
            else
            {
                moreScores = false;
            }
        }
        moreScores = true;
    }
}
