using System.Collections.Generic;
using UnityEngine;

public class CustomPhysics : MonoBehaviour
{
    public Vector2 velocity;
    // Determines where the object is trying to move
    protected Vector2 targetVelocity;
    protected Vector2 groundVector;
    protected Rigidbody2D rb;
    // Allows to choose which results we want to use from the array
    protected ContactFilter2D conFilter;
    // Array that stores the collision results
    protected RaycastHit2D[] collisions = new RaycastHit2D[16];
    // Adds the RaycastHit2D results to it's own list
    protected List<RaycastHit2D> collisionsList = new List<RaycastHit2D>(16);
    // Minimal distance objects can move
    protected const float minMovDist = 0.001f;
    // Ensures that the player does not get stuck by an object
    protected const float unstucker = 0.01f;
    [HideInInspector]
    public bool grounded, grappling;

    public float gravityChange = 0.9f;
    public float groundCheckY = 0.65f;
    public float grappleVelocityMultiplier;
    

    void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        conFilter.useTriggers = false;
        // Uses the Physics2D settings to choose what layer it is going to check collision against
        conFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        conFilter.useLayerMask = true;
    }

    // Update is called once per frame
    void Update()
    {
        // It will stop using the argument from the previous frame
        targetVelocity = Vector2.zero;
        CalculateVelocity();


    }
    
    
    protected virtual void CalculateVelocity()
    {

    }

    // Purpose of this function is to move an object downwards every frame
    void FixedUpdate()
    {
        if (!(CompareTag("Player") && GetComponent<RopeControl>().rope != null))
        {
            velocity += gravityChange * Physics2D.gravity * Time.fixedDeltaTime;
        }
        velocity.x = targetVelocity.x;
        grounded = false;
        
        // Determines the position of where an object lands
        Vector2 dPos = velocity * Time.fixedDeltaTime;
        // Allows the player to move up on slopes
        Vector2 moveOnGround = new Vector2(groundVector.y, -groundVector.x);
        // Horizontal movement
        
        Vector2 movement = moveOnGround * dPos.x;
        // Calling the Movement function with a false argument because we are not moving along the Y axis 
        Movement(movement, false);
        
        // Vertical movement
        movement = Vector2.up * dPos.y;
        // Calling the function
        Movement(movement, true);
    }
    
    // Function acts out the movement of objects
    void Movement(Vector2 movement, bool verticalMovement)
    {
        
        // Determines the distance it is trying to move
        float distance = movement.magnitude;

        if(distance > minMovDist)
        {
            // Checks if any of the colliders are going to overlap
            int restore = rb.Cast(movement, conFilter, collisions, distance + unstucker);
            // Clearing old data stored in the collision list
            collisionsList.Clear();
            // Looping the collision check, ensuring that only col. entries that have 
            // anything in them gets added to the Raycast2D list
            for(int i = 0; i < restore; i++)
            {
                // All new entries from the collisions array get copied over to the list
                collisionsList.Add(collisions[i]);
            }
            // Checks which objects can overlap and the angle they collide with
            for(int i = 0; i < collisionsList.Count; i++)
            {
                // Checks the normal of all the entries in the list
                Vector2 currentVector = collisionsList[i].normal;
                if (currentVector.y > groundCheckY)
                {
                    grounded = true;
                    if (verticalMovement)
                    {
                        groundVector = currentVector;
                        currentVector.x = 0;
                    }
                }
                // Getting the difference between the velocity and the current vector
                // to stop the player from clashing with another collider
                float projection = Vector2.Dot(velocity, currentVector);
                if (projection < 0)
                {              
                    velocity -= projection * currentVector;
                }
                float changeDist = collisionsList[i].distance - unstucker;
                distance = changeDist < distance ? changeDist : distance;
            }

        }
        if (CompareTag("Player"))
        {
            if (GetComponent<RopeControl>().rope == null)
            {
                if (grappling)
                {
                    grappling = false;
                }
                rb.velocity = velocity;
            }
        }
        else
        {
            rb.velocity = velocity;
        }
    }
}
