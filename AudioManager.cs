using System.Collections;
using UnityEngine;

//Serialize sounds in game
[System.Serializable]

//Class for individual sounds, allowing for individual settings, references and control.
public class Sound
{
    //Initialise name and audioclip and source
    public string name;
    public AudioClip clip;
    private AudioSource source;

    //Sliders and floats to affect sound playback
    [Range(0f,1f)]
    public float volume = 1f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1f;
    [Range(0f, 2.0f)]
    public float volumeDeviation = 0.1f;
    [Range(0f, 0.5f)]
    public float pitchDeviation = 0.5f;
    public float fadeTime = 0.5f;

    //Set if sound will loop
    public bool loop = false;

    public bool playOnAwake = false;

    //Setsource is called when a sound is created to assign its components
    public void SetSource(AudioSource inputSource)
    {
        source = inputSource;
        source.clip = clip;
        source.loop = loop;
        source.playOnAwake = playOnAwake;
    }

    //All the following methods affect the playback of their sound using the parameters set above

    //Play is called to start playback of a sound
    public void Play()
    {
        source.volume = volume * (1 + Random.Range(-volumeDeviation / 2f, volumeDeviation / 2f));
        source.pitch = pitch * (1 + Random.Range(-pitchDeviation / 2f, pitchDeviation / 2f));
        source.Play();
    }

    //PlayOneShot is called to start the playback of a sound but only if that sound is not already playing
    public void PlayOneShot()
    {
        source.volume = volume * (1 + Random.Range(-volumeDeviation / 2f, volumeDeviation / 2f));
        source.pitch = pitch * (1 + Random.Range(-pitchDeviation / 2f, pitchDeviation / 2f));
        if (!source.isPlaying)
        {
            source.Play();
        }
    }

    //Fade is called to fade out a sound so that it doesn't cut off abruptly
    public IEnumerator Fade()
    {
        source.volume = volume;
        //While source is still audible
        while (source.volume >= 0f)
        {
            //Reduce volume with respect to the ammount of time passed, in order to keep fade smooth
            source.volume -= volume * (Time.deltaTime / fadeTime);
            //Keep co-routine running till sound is silent
            yield return null;
        }
        source.volume = volume;
        //When source is silent stop it
        source.Stop();
    }

    //Stop is called to end sound playback abruptly
    public void Stop()
    {
        source.Stop();
    }
}

//Class for handling the playing of sounds
public class AudioManager : MonoBehaviour
{
    //Set a public array to store and set up sounds
    public Sound[] sounds;
    //Set where sounds should be stored
    public Transform soundContainer;
    //Initialise a master volume multiplier
    [Range(0.0f, 1.0f)]
    public float masterVolume;

    // Start is called before the first frame update
    private void Start()
    {
        //Assign masterVolume from PlayerPrefs, set from main menu
        masterVolume = PlayerPrefs.GetFloat("masterVolume", 0.5f);
        //For each sound in the array, create a new game object and add an audiosource component to play that sound.  Put each sound in the sound container and adjust their volume based on the masterVolume.
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject go = new GameObject(sounds[i].name + " Sound");
            sounds[i].SetSource(go.AddComponent<AudioSource>());
            soundContainer = GameObject.Find("Sounds").transform;
            sounds[i].volume *= masterVolume;
        }
    }

    //Each of the following methods is called to find the desired sound and Play / Fade / Stop it through it's own class
    //StopAllCoroutines has been used to ensure fade is cancelled before the sound is played again, to ensure it wont be faded out immediately after starting
    public void PlaySound (string name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == name)
            {
                StopAllCoroutines();
                sounds[i].Play();
                return;
            }
        }

        Debug.Log("AudioManager : No sound with name - " + name);
    }

    public void PlaySoundComplete(string name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == name)
            {
                StopAllCoroutines();
                sounds[i].PlayOneShot();
                return;
            }
        }

        Debug.Log("AudioManager : No sound with name - " + name);
    }

    public void StopSound(string name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == name)
            {
                sounds[i].Stop();
                return;
            }
        }

        Debug.Log("AudioManager : No sound with name - " + name);
    }

    public void FadeSound(string name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == name)
            {
                StartCoroutine(sounds[i].Fade());
                return;
            }
        }

        Debug.Log("AudioManager : No sound with name - " + name);
    }

    public void StopAllSounds()
    {
        foreach (Sound sound in sounds)
        {
            sound.Stop();
        }
    }
}
