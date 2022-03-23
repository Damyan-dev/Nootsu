using System.Collections.Generic;
using UnityEngine;

public class RopeControl : MonoBehaviour

{
    public GameObject ropeShooter, grappleReticule;
    public SpringJoint2D rope;
    public int maxRopeFrameCount;
    private int ropeFrameCount;
    public LineRenderer lineRenderer;
    public float power, offset, damping, wrapRayAccuracy, grappleCooldownTime;
    public float[] ranges = new float[4];
    [Range(0.0f, 0.49f)]
    public float wrapRayOffset;
    [Range(0.0f, 1.0f)]
    public float wrapRayBuffer;
    public LayerMask layerMask;
    [HideInInspector]
    public List<Vector2> contacts;
    CompositeCollider2D platforms;
    private Vector2[] points;
    [HideInInspector]
    public float grappleCooldown;
    private float range;
    public Camera mainCamera;
    public AudioManager audioManager;

    // Add player position and an empty slot for the first rpe node to contacts array.
    //Set grapple settings and fetch range from PlayerPrefs
    private void Awake()
    {
        contacts.Add(ropeShooter.transform.position);
        contacts.Add(Vector2.one);
        lineRenderer.enabled = false;
        platforms = GameObject.Find("Platforms").GetComponent<CompositeCollider2D>();
        GetCompositeShapes();
        range = ranges[PlayerPrefs.GetInt("grappleRange", 0)];
        audioManager = GameObject.Find("LevelManager").GetComponent<AudioManager>();
    }

    //Create a Vector2 array of all contactable points, to use when rounding corners.  Stops rope 'sticking' to closer points on the same surface
    private void GetCompositeShapes()
    {
        points = new Vector2[platforms.pointCount];
        int currentPoint = 0;
        for (int i = 0; i < platforms.pathCount; i++)
        {
            Vector2[] pathPoints = new Vector2[platforms.GetPathPointCount(i)];
            platforms.GetPath(i, pathPoints);
            for (int j = 0; j < pathPoints.Length; j++)
            {
                points[currentPoint] = pathPoints[j];
                currentPoint++;
            }
        }
    }

    // Check for grapple activation, cooldown the grapple if recently activated and position the reticule on screen.
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) == true)
        {
            Fire ();
        }
        else if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            GameObject.DestroyImmediate(rope);
            ropeFrameCount = 0;
        }
        
 
        
        if (grappleCooldown > 0)
        {
            grappleCooldown = Mathf.Clamp(grappleCooldown - Time.deltaTime, 0.0f, grappleCooldownTime);
        }
        ReticulePosition();
    }


    void LateUpdate()
    {
        if (rope != null)
        {
            //Enable visibility of rope.
            contacts[0] = ropeShooter.transform.position;
            lineRenderer.enabled = true;
            //Check if the first contact point is visible from player position.  If not, add a node to the rope and set it as the first contact point.
            RaycastHit2D hit = Physics2D.Raycast(contacts[0], contacts[1] - contacts[0], (contacts[1] - contacts[0]).magnitude, layerMask);
            if (hit.point != contacts[1] && hit != false && !contacts.Contains(hit.point))
            {
                contacts.Insert(1, GetClosestPoint(hit.point));
                SetRope(false);
            }
            //If rope is longer than 2 nodes check if the second node is visible.
            if (contacts.Count > 2)
            {
                RaycastHit2D checkHit = Physics2D.Raycast(contacts[0], contacts[2] - contacts[0], (contacts[2] - contacts[0]).magnitude, layerMask);
                if (checkHit.point == contacts[2] && checkHit != false)
                {
                    //Ensure that there is no terrain in the way of the rope, to stop it unwrapping through terrain.
                    bool obstructed = false;
                    for (int i = 0; i < wrapRayAccuracy; i++)
                    {
                        float wrapRayRange = 1.0f - 2 * wrapRayOffset;
                        RaycastHit2D[] blockCheck = Physics2D.RaycastAll(contacts[0], Vector2.Lerp(contacts[1] - contacts[0], contacts[2] - contacts[0], wrapRayOffset + ((wrapRayRange * i) / wrapRayAccuracy)), (contacts[1] - contacts[0]).magnitude - wrapRayBuffer, layerMask);
                        Debug.DrawRay(contacts[0], Vector2.Lerp(contacts[1] - contacts[0], contacts[2] - contacts[0], wrapRayOffset + ((wrapRayRange * i) / wrapRayAccuracy)), Color.red, 1);
                        foreach (RaycastHit2D contact in blockCheck)
                        {
                            if (contact.collider.gameObject.CompareTag("Platforms"))
                            {
                                obstructed = true;
                            }
                        }
                    }
                    //If not obstructed by terrain and second node is visible remove the first node and set the second as first.
                    if (!obstructed)
                    {
                        contacts.RemoveAt(1);
                        contacts.TrimExcess();
                        SetRope(false);
                    }
                }
            }
            //Set line renderer visibility based onb if there is a rope or not.
            lineRenderer.positionCount = contacts.Count;
            for (int i = 0; i < contacts.Count; i++)
            {
                lineRenderer.SetPosition(i, contacts[i]);
            }
        }
        else
        {
            lineRenderer.enabled = false;
            if (contacts.Count > 2)
            {
                contacts.RemoveRange(1, contacts.Count - 2);
            }
        }
    }

    //While there is a rope count the frames until max frames are reached, then destroy the rope.
    void FixedUpdate()
    {
        if (rope != null)
        {
            ropeFrameCount++;

            if(ropeFrameCount > maxRopeFrameCount)
            {
                GameObject.DestroyImmediate(rope);
                ropeFrameCount = 0;
            }
        }
    }

    //check that the grapple hit something in range and set up the rope accordingly.
    void Fire()
    {
        if (grappleCooldown == 0)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position = ropeShooter.transform.position;
            Vector3 direction = mousePosition - position;

            RaycastHit2D hit = Physics2D.Raycast(position, direction, range, layerMask);

            if (hit.collider != null)
            {

                audioManager.PlaySoundComplete("Grapple");
                contacts[1] = hit.point + (hit.normal.normalized * 0.1f);
                contacts[0] = ropeShooter.transform.position;
                contacts.RemoveRange(1, contacts.Count - 2);
                contacts.TrimExcess();
                SetRope(true);
                ropeFrameCount = 0;
                grappleCooldown = grappleCooldownTime;
                GameObject.Find("Player").GetComponent<Controller>().velocity.y = 0.0f;
            }

        }
    }

    //Ensure that the spring joint is only set between first 2 nodes.
    void SetRope(bool applyOffset)
    {
        SpringJoint2D newRope = ropeShooter.AddComponent<SpringJoint2D>();
        newRope.enableCollision = false;
        newRope.frequency = power;
        newRope.connectedAnchor = contacts[1];
        newRope.distance = (contacts[1] - contacts[0]).magnitude;
        if (applyOffset)
        {
            newRope.distance *= 1.0f - offset;
        }
        newRope.dampingRatio = damping;
        newRope.autoConfigureDistance = false;
        newRope.enabled = true;

        if (rope != null)
        {
            GameObject.DestroyImmediate(rope);
        }
        rope = newRope;
    }

    //Used to find the closest corner when rope wraps around terrain.
    private Vector2 GetClosestPoint(Vector2 hit)
    {
        Vector2 closestPoint = hit;
        float distance = Mathf.Infinity;
        foreach (Vector2 point in points)
        {
            float pointDistance = Vector2.Distance(hit, point);
            if (distance == Mathf.Infinity || pointDistance < distance)
            {
                distance = pointDistance;
                closestPoint = point;
            }
        }
        return closestPoint;
    }

    //Used to attach reticule to mouse, within grapple range.
    private void ReticulePosition()
    {
        Vector2 mousePos = ((Vector2) mainCamera.ScreenToWorldPoint(Input.mousePosition) - (Vector2) ropeShooter.transform.position) / 4;
        grappleReticule.transform.localPosition = Vector2.Lerp(grappleReticule.transform.localPosition, Vector2.ClampMagnitude(mousePos, range / 4), 0.5f);
    }
}
