using UnityEngine;
using UnityEngine.SceneManagement;

//Class is called to load a level based on the name of the button it is attached to
public class LoadScene : MonoBehaviour
{
    // When the scene loads, it will set the audio timer container to the value of 0, so that the audio clip for the player movement won't play with each frame.
    public void Awake()
    {
        
    }

    //Load is called to load a scene
    public void Load()
    {
        //The name of the gameobject is read and if a level with the corresponding name is found then that level is loaded
        SceneManager.LoadScene(gameObject.name, LoadSceneMode.Single);
    }
}
