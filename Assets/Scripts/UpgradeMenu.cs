using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{
    public Button[] upgrades;
    public GameObject[] borders, costs;
    public int[] upgradeCosts;
    public Text documents;
    private AudioManager audioManager;

    private void OnEnable()
    {
        for (int i = 0; i < costs.Length; i++)
        {
            costs[i].GetComponent<Text>().text = upgradeCosts[i].ToString();
        }
        Cursor.visible = true;
        LevelManager levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        PlayerPrefs.SetInt("totalCollectables", PlayerPrefs.GetInt("totalCollectables", 0) + levelManager.collectables);
        CalculateState();
        audioManager = GameObject.Find("LevelManager").GetComponent<AudioManager>();
    }

    private void Update()
    {
        documents.text = PlayerPrefs.GetInt("totalCollectables", 0).ToString();
    }

    public void CalculateState()
    {
        for (int i = 0; i < 3; i++)
        {
            if (PlayerPrefs.GetInt("maxSpeed", 0) != i || PlayerPrefs.GetInt("totalCollectables", 0) < upgradeCosts[i])
            {
                upgrades[i].interactable = false;
            }
            else
            {
                upgrades[i].interactable = true;
            }
            if (PlayerPrefs.GetInt("maxSpeed", 0) <= i)
            {
                borders[i].SetActive(false);
            }
            else
            {
                borders[i].SetActive(true);
            }
        }
        for (int i = 3; i < 6; i++)
        {
            if (PlayerPrefs.GetInt("grappleRange", 0) != i - 3 || PlayerPrefs.GetInt("totalCollectables", 0) < upgradeCosts[i])
            {
                upgrades[i].interactable = false;
            }
            else
            {
                upgrades[i].interactable = true;
            }
            if (PlayerPrefs.GetInt("grappleRange", 0) <= i - 3)
            {
                borders[i].SetActive(false);
            }
            else
            {
                borders[i].SetActive(true);
            }
        }
        if (PlayerPrefs.GetInt("detectorEnabled", 0) > 0 || PlayerPrefs.GetInt("totalCollectables", 0) < upgradeCosts[6])
        {
            upgrades[6].interactable = false;
        }
        else
        {
            upgrades[6].interactable = true;
        }
        if (PlayerPrefs.GetInt("detectorEnabled", 0) == 0)
        {
            borders[6].SetActive(false);
        }
        else
        {
            borders[6].SetActive(true);
        }
        if (PlayerPrefs.GetInt("tazerUnlocked", 0) > 0 || PlayerPrefs.GetInt("totalCollectables", 0) < upgradeCosts[7])
        {
            upgrades[7].interactable = false;
        }
        else
        {
            upgrades[7].interactable = true;
        }
        if (PlayerPrefs.GetInt("tazerUnlocked", 0) == 0)
        {
            borders[7].SetActive(false);
        }
        else
        {
            borders[7].SetActive(true);
        }
    }

    public void UpgradeSpeed(int upgrade)
    {
        audioManager.PlaySound("Upgrade");
        PlayerPrefs.SetInt("maxSpeed", PlayerPrefs.GetInt("maxSpeed", 0) + 1);
        PlayerPrefs.SetInt("totalCollectables", PlayerPrefs.GetInt("totalCollectables", 0) - upgradeCosts[upgrade]);
        CalculateState();
    }

    public void UpgradeGrapple(int upgrade)
    {
        audioManager.PlaySound("Upgrade");
        PlayerPrefs.SetInt("grappleRange", PlayerPrefs.GetInt("grappleRange", 0) + 1);
        PlayerPrefs.SetInt("totalCollectables", PlayerPrefs.GetInt("totalCollectables", 0) - upgradeCosts[upgrade]);
        CalculateState();
    }

    public void UpgradeDetector()
    {
        audioManager.PlaySound("Upgrade");
        PlayerPrefs.SetInt("detectorEnabled", PlayerPrefs.GetInt("detectorEnabled", 0) + 1);
        PlayerPrefs.SetInt("totalCollectables", PlayerPrefs.GetInt("totalCollectables", 0) - upgradeCosts[6]);
        CalculateState();
    }

    public void UpgradeTazer()
    {
        audioManager.PlaySound("Upgrade");
        PlayerPrefs.SetInt("tazerUnlocked", PlayerPrefs.GetInt("tazerUnlocked", 0) + 1);
        PlayerPrefs.SetInt("totalCollectables", PlayerPrefs.GetInt("totalCollectables", 0) - upgradeCosts[7]);
        CalculateState();
    }
}
