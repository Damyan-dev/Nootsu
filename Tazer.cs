using UnityEngine;
using System.Collections;

public class Tazer : MonoBehaviour
{
    public float tazerCooldownTime;
    private bool tazerUnlocked = false;
    public bool tazerCooling = false;
    private float playerStartSpeed;
    public SpriteRenderer sparks;
    public Animator sparkAnimator;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("tazerUnlocked", 0) == 1)
        {
            tazerUnlocked = true;
            playerStartSpeed = GameObject.Find("Player").GetComponent<Controller>().maxSpeeds[PlayerPrefs.GetInt("maxSpeed", 0)];
        }
        else
        {
            tazerUnlocked = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tazerUnlocked && !tazerCooling)
        {
            if (Input.GetAxis("Fire2") == 1.0f)
            {
                GameObject.Find("Player").GetComponent<Controller>().maxSpeed = playerStartSpeed;
                GameObject.Find("LevelManager").GetComponent<LevelManager>().trackers = 0;
                sparks.enabled = true;
                sparkAnimator.enabled = true;
                tazerCooling = true;
                StartCoroutine("TazerCooldown");
            }
        }
    }

    private IEnumerator TazerCooldown()
    {
        yield return new WaitForSeconds(tazerCooldownTime);
        tazerCooling = false;
    }

    public void DeactivateSparks()
    {
        sparks.enabled = false;
        sparkAnimator.enabled = false;
    }
}
