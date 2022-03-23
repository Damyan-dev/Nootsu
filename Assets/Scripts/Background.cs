using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ParallaxEffect
{
    public float x = 0f;
    public float y = 0f;
}

public class Background : MonoBehaviour
{
    private GameObject player;
    public float width, height;
    private Vector3 playerStartPos;
    public List<Vector3> startPos;
    public ParallaxEffect[] parallaxEffects;
    public List<Transform> backgrounds;
    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.transform.parent.gameObject;
        playerStartPos = new Vector2(player.transform.position.x, player.transform.position.y);
        for (int i = 0; i < parallaxEffects.Length; i++)
        {
            backgrounds.Add(GameObject.Find("Background" + i).transform);
        }
        width = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.x;
        height = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.y;
        foreach (Transform background in backgrounds)
        {
            startPos.Add(background.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 displacement = new Vector2(player.transform.position.x - playerStartPos.x, player.transform.position.y - playerStartPos.y);
        for (int i = 0; i < backgrounds.Count; i++)
        {
            backgrounds[i].position = new Vector3(startPos[i].x - displacement.x * parallaxEffects[i].x, Mathf.Clamp(startPos[i].y - displacement.y * parallaxEffects[i].y, (player.transform.position.y - playerStartPos.y) - ((height / 2) * (1 - parallaxEffects[i].y)  ), (player.transform.position.y - playerStartPos.y) + ((height / 2) *(1 - parallaxEffects[i].y)  )), startPos[i].z);
        }
        for (int i = 0; i < backgrounds.Count; i++)
        {
            Vector2 screenDisplacement = new Vector2((player.transform.position.x - playerStartPos.x) * (1 + parallaxEffects[i].x), (player.transform.position.y - playerStartPos.y) * (1 + parallaxEffects[i].y));
            if (screenDisplacement.x > startPos[i].x + width)
            {
                startPos[i] = new Vector3(startPos[i].x + width, startPos[i].y, startPos[i].z);
            }
            else if (screenDisplacement.x < startPos[i].x - width)
            {
                startPos[i] = new Vector3(startPos[i].x - width, startPos[i].y, startPos[i].z);
            }
        }
    }
}
