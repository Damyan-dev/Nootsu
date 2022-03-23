using UnityEngine;
using UnityEngine.UI;

//Class handles behaviour of buttons in the settings menu
public class SettingsMenu : MonoBehaviour
{
    //Set references to on screen slider and to the audiomanager and set the mute bool
    private bool mute;
    public Slider slider;
    public AudioSource audioManager;
    public Toggle toggle;

    // Start is called before the first frame update
    private void Start()
    {
        //Set slider to the correct position, as set by user or to 0.5 if no value is saved
        slider.value = PlayerPrefs.GetFloat("initVolume", 0.5f);
        if(PlayerPrefs.GetInt("isMuted", 0) == 1)
        {
            mute = true;
            toggle.isOn = false;
            SetVolume();
        }
        else
        {
            mute = false;
            toggle.isOn = true;
            SetVolume();
        }


    }

    private void Update()
    {
        if (!mute)
        {
            SetVolume();
        }
    }

    //Mute is called when the player selects the mute check box
    public void Mute(bool isMuted)
    {
        //Set the global mute bool to that of the checkbox and set the volume
        mute = !isMuted;
        if (mute)
        {
            PlayerPrefs.SetInt("isMuted", 1);
        }
        else
        {
            PlayerPrefs.SetInt("isMuted", 0);
        }
        SetVolume();
    }

    //SetVolume is called when the volume is changed
    public void SetVolume()
    {
        //If volume is not muted then set the master volume to the volume on the slider and play a test sound at this new volume before reseting the volume of the sound
        if (!mute)
        {
            float volume = slider.value;
            PlayerPrefs.SetFloat("masterVolume", volume);
            PlayerPrefs.SetFloat("initVolume", volume);
            audioManager.volume = volume;
        }
        //If volume is muted then set volume to zero
        else
        {
            PlayerPrefs.SetFloat("masterVolume", 0.0f);
            audioManager.volume = 0.0f;
        }
    }

    //Reset is called to remove all game progress
    public void Reset()
    {
        //Set the current level in PlayerPrefs to the first level, locking the ones above it
        PlayerPrefs.SetInt("currentLevel", 1);
        PlayerPrefs.SetInt("maxSpeed", 0);
        PlayerPrefs.SetInt("grappleRange", 0);
        PlayerPrefs.SetInt("detectorEnabled", 0);
        PlayerPrefs.SetInt("tazerUnlocked", 0);
        PlayerPrefs.SetInt("totalCollectables", 0);
        PlayerPrefs.SetInt("Level1Score", 0);
        PlayerPrefs.SetInt("Level2Score", 0);
        PlayerPrefs.SetInt("Level3Score", 0);
    }
}
