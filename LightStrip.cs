using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LightStrip : MonoBehaviour
{
    private Tilemap tilemap;
    public bool rainbow;
    public Color defaultColour, fullSpeedColour;
    [Range(0.0f, 1.0f)]
    public float hueShift;
    private float brightVelocity, runVelocity, slideVelocity;
    public Rigidbody2D playerBody;
    private Vector2 velocity;
    public List<Vector3Int> tiles;
    public List<Color> tileColours;
    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        playerBody = GameObject.Find("AnimationContainer").GetComponentInParent<Rigidbody2D>();
        brightVelocity = GameObject.Find("Animator").GetComponent<Animations>().fullFadeVelocity;
        runVelocity = GameObject.Find("Animator").GetComponent<Animations>().runVelocity;
        slideVelocity = GameObject.Find("Animator").GetComponent<Animations>().slideVelocity;
        if (gameObject.name != "LightStrip")
        {
            TileBase[] tilesArray = tilemap.GetTilesBlock(tilemap.cellBounds);
            for (int x = 0; x < tilemap.cellBounds.size.x; x++)
            {
                for (int y = 0; y < tilemap.cellBounds.size.y; y++)
                {
                    TileBase tile = tilesArray[x + y * tilemap.cellBounds.size.x];
                    if (tile != null)
                    {
                        tiles.Add(new Vector3Int(x, y - 2, 0));
                        tileColours.Add(tilemap.GetColor(new Vector3Int(x, y - 2, 0)));
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        velocity = playerBody.velocity;
        if (gameObject.name != "LightStrip" && velocity.magnitude >= runVelocity)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                tilemap.SetColor(tiles[i], Color.Lerp(tileColours[i], defaultColour, (velocity.magnitude - runVelocity) / (slideVelocity - runVelocity)));
            }
        }
        float colourLerpPosition = (velocity.magnitude - runVelocity) / (brightVelocity - runVelocity);
        if (rainbow == true && velocity.magnitude >= runVelocity)
        {
            if (velocity.magnitude < brightVelocity)
            {
                tilemap.color = Color.Lerp(defaultColour, fullSpeedColour, colourLerpPosition);
            }
            else
            {
                float h, s, v, a;
                Color.RGBToHSV(defaultColour, out h, out s, out v);
                a = defaultColour.a;
                h += hueShift;
                defaultColour = Color.HSVToRGB(h, s, v);
                defaultColour.a = a;
                Color.RGBToHSV(fullSpeedColour, out h, out s, out v);
                a = fullSpeedColour.a;
                h += hueShift;
                fullSpeedColour = Color.HSVToRGB(h, s, v);
                fullSpeedColour.a = a;
                tilemap.color = fullSpeedColour;
            }
        }
        else if (velocity.magnitude >= runVelocity)
        {
            tilemap.color = Color.Lerp(defaultColour, fullSpeedColour, colourLerpPosition);
        }
        else tilemap.color = defaultColour;
    }
}
