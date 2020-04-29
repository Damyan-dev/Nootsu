using UnityEngine;
using UnityEngine.UI;

//Class is called to make levels interactable only once the previous level is cleared
public class LevelSelect : MonoBehaviour
{
    //Set each button on screen as a button in an array
    public Button[] levels;

    // OnEnable is called every time the gameobject which the class is attached to is activated
    void OnEnable()
    {
        //Run through the array of buttons and set any after the latest level reached according to PlayerPrefs as uninteractable
        for (int i = 0; i < levels.Length; i++)
        {
            if (i + 1 > PlayerPrefs.GetInt("currentLevel", 1))
            {
                levels[i].interactable = false;
            }
        }
    }
}
