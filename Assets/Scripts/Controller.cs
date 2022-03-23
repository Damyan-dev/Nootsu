using UnityEngine;

public class Controller : CustomPhysics
{
    public float jumpSpeed = 10;
    public float maxSpeed, minSpeed;
    public float[] maxSpeeds = new float[4];
    public AnimationCurve accelerationCurve;
    public float accelerationPeriod, decelerationMultiplier, playerDecelerationMultiplier;
    public bool moving;
    public float moveTime, deceleration;
    private Vector2 movement = Vector2.zero;
    private AnimationCurve inverseAccelerationCurve;
    private AudioManager audioManager;

    void Start()
    {

        audioManager = GameObject.Find("LevelManager").GetComponent<AudioManager>();
        maxSpeed = maxSpeeds[PlayerPrefs.GetInt("maxSpeed", 0)];
        inverseAccelerationCurve = new AnimationCurve();
        for (int i = 0; i < accelerationCurve.length; i++)
        {
            Keyframe inverseKey = new Keyframe(accelerationCurve.keys[i].value, accelerationCurve.keys[i].time, accelerationCurve.keys[i].outTangent, accelerationCurve.keys[i].inTangent, accelerationCurve.keys[i].outWeight, accelerationCurve.keys[i].inWeight);
            inverseAccelerationCurve.AddKey(inverseKey);
        }
    }
    
    protected override void CalculateVelocity()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 && (Mathf.Sign(Input.GetAxisRaw("Horizontal")) == Mathf.Sign(velocity.x) || velocity.x == 0.0f))
        {
            if (Mathf.Abs(velocity.x) < 0.0001f)
            {
                moveTime = 0.0f;
            }
            if (moving)
            {
                if (!grappling)
                {
                    moveTime = Mathf.Clamp(moveTime + Time.deltaTime, 0, accelerationPeriod);
                }
                movement.x = Input.GetAxisRaw("Horizontal") * accelerationCurve.Evaluate(moveTime / accelerationPeriod);
            }
            else
            {
                moving = true;
                moveTime = 0.0f;
                movement.x = Input.GetAxisRaw("Horizontal") * accelerationCurve.Evaluate(moveTime / accelerationPeriod);
            }
        }
        else
        {
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                deceleration = playerDecelerationMultiplier * Time.deltaTime;
            }
            else
            {
                deceleration = decelerationMultiplier * Time.deltaTime;
            }
            moveTime = Mathf.Clamp(moveTime - deceleration, 0, accelerationPeriod);
            if (movement.x < 0)
            {
                movement.x = Mathf.Clamp(-accelerationCurve.Evaluate(moveTime / accelerationPeriod), -accelerationPeriod, 0);
            }
            else if (movement.x > 0)
            {
                movement.x = Mathf.Clamp(accelerationCurve.Evaluate(moveTime / accelerationPeriod), 0, accelerationPeriod);
            }
            else
            {
                moving = false;
            }
        }

        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = jumpSpeed;
            audioManager.PlaySoundComplete("Jump");
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > 0)
            {
                velocity.y *= 0.5f;
            }
        }
        targetVelocity = movement * maxSpeed;

    }

    private void LateUpdate()
    {
        if (GetComponent<RopeControl>().rope != null)
        {
            if (!grappling)
            {
                grappling = true;
                GetComponent<Rigidbody2D>().velocity *= grappleVelocityMultiplier;
                moveTime = 0.0f;
            }
            if (Input.GetAxis("Horizontal") != 0.0f)
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(Input.GetAxis("Horizontal"), 0.0f), ForceMode2D.Force);
            }
        }
        else
        {
            if (grappling)
            {
                
                moveTime = inverseAccelerationCurve.Evaluate((Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) / maxSpeed) * accelerationPeriod);
                movement.x = GetComponent<Rigidbody2D>().velocity.x / maxSpeed;
                velocity = GetComponent<Rigidbody2D>().velocity;
                targetVelocity = GetComponent<Rigidbody2D>().velocity;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Collectable" && GetComponent<BoxCollider2D>().IsTouching(collision))
        {
            audioManager.PlaySoundComplete("Document");
            GameObject.Find("LevelManager").GetComponent<LevelManager>().collectables++;
            Destroy(collision.gameObject);
        }
    }
}
