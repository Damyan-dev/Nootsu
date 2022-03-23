using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private LevelManager levelManager;
    public Image grappleImage, tazerImage;
    public GameObject score, collectables, trackers;
    public Sprite[] grappleSprites, tazerSprites;
    public float grappleCooldown;
    private GameObject player, tazer;
    private bool grappleCooling;
    private float grappleTime, grappleMaxTime, tazerTime, playerStartPosX, playerFarthestPointX, winPosX;
    // Start is called before the first frame update
    void Start()
    {
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        player = GameObject.Find("Player");
        tazer = GameObject.Find("Tazer");
        grappleCooling = false;
        grappleMaxTime = player.GetComponent<RopeControl>().grappleCooldownTime;
        if (PlayerPrefs.GetInt("tazerUnlocked", 0) == 0)
        {
            tazerImage.enabled = false;
        }
        playerStartPosX = player.transform.position.x;
        winPosX = GameObject.Find("WinObject").transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        GrappleHandler();
        TazerHandler();
        ScoreHandler();
        CollectableHandler();
        TrackerHandler();
    }

    void GrappleHandler()
    {
        if (player.GetComponent<RopeControl>().rope != null && !grappleCooling)
        {
            grappleCooling = true;
            grappleImage.sprite = grappleSprites[1];
        }
        else if (player.GetComponent<RopeControl>().rope == null && grappleCooling)
        {
            grappleCooling = false;
            grappleImage.sprite = grappleSprites[0];
        }
        else if (grappleCooling)
        {
            grappleTime = player.GetComponent<RopeControl>().grappleCooldown;
            int grappleNumber = Mathf.FloorToInt(grappleTime / (grappleMaxTime / 17));
            grappleImage.sprite = grappleSprites[grappleNumber];
        }
    }

    void TazerHandler()
    {
        if (tazer.GetComponent<Tazer>().tazerCooling == true && Mathf.FloorToInt(tazerTime / (tazer.GetComponent<Tazer>().tazerCooldownTime / tazerSprites.Length)) < tazerSprites.Length - 1)
        {
            tazerTime += Time.deltaTime;
            int tazerNumber = Mathf.FloorToInt(tazerTime / (tazer.GetComponent<Tazer>().tazerCooldownTime / tazerSprites.Length));
            tazerImage.sprite = tazerSprites[tazerNumber];
        }
        else if (tazer.GetComponent<Tazer>().tazerCooling != true && tazerTime != 0.0f)
        {
            tazerTime = 0.0f;
            tazerImage.sprite = tazerSprites[tazerSprites.Length - 1];
        }
    }

    void ScoreHandler()
    {
        if (player.transform.position.x > playerFarthestPointX)
        {
            playerFarthestPointX = player.transform.position.x;
        }
        score.GetComponent<Text>().text = "Score : " + Mathf.FloorToInt(levelManager.score * ((playerFarthestPointX - playerStartPosX) / (winPosX - playerStartPosX)));
    }

    void CollectableHandler()
    {
        if (levelManager.collectables < 10)
        {
            collectables.GetComponent<Text>().text = "0" + levelManager.collectables.ToString();
        }
        else
        {
            collectables.GetComponent<Text>().text = levelManager.collectables.ToString();
        }
    }

    void TrackerHandler()
    {
        trackers.GetComponent<Text>().text = levelManager.trackers.ToString();
    }
}
