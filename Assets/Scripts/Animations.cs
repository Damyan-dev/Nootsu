using UnityEngine;
using UnityEngine.UI;

public class Animations : MonoBehaviour
{
    private static readonly string[] states = new string[12] {"idling", "walking", "running", "sliding", "jumping", "hanging", "falling", "grappling", "crashing", "landing", "skidding", "winning"};
    public string state;
    public float groundCheckProtrusion, frontCheckProtrusion, idleVelocityCutoff, runVelocity, slideVelocity, skidDelta, fullFadeVelocity, xScalarMultiplier;
    public int maxFramesTillFall;
    [Range(0.0f, 1.0f)]
    public float fadeFalloff, lollyChance, slideChance, randomChance, grappleSpin;
    public Color fadeMax;
    private float prevXVelocity, xVelocity, idleRandom;
    private int fallTracker, jumpDelay, crashDelay;
    private Animator animator;
    private Image fade;
    private Color defaultFade, currentFade;
    private GameObject player;
    private Vector3 velocity, boundsOffset;
    public LayerMask ignorePlayer;
    private RopeControl ropeScript;
    private Bounds playerBounds;
    private Collider2D[] colliders;
    private bool isJumping;
    private AudioManager audioManager;

    // Define all properties accessed by the script and add all colliders to one bounds object for casting.
    void Start()
    {
        player = transform.parent.transform.parent.gameObject;
        animator = GetComponent<Animator>();
        fade = GetComponentInChildren<Image>();
        xVelocity = 0f;
        state = states[0];
        defaultFade = fade.color;
        ropeScript = GameObject.Find("Player").GetComponent<RopeControl>();
        audioManager = GameObject.Find("LevelManager").GetComponent<AudioManager>();
        colliders = player.GetComponents<Collider2D>();
        playerBounds = new Bounds(transform.position, Vector3.zero);
        foreach (Collider2D collider in colliders)
        {
            playerBounds.Encapsulate(collider.bounds);
        }
        boundsOffset = playerBounds.center - player.transform.position;
        fallTracker = 0;
        audioManager.PlaySoundComplete("Music");
    }

    //Store the velocity of the last frame for direction and speed change calculations.
    void FixedUpdate()
    {
        prevXVelocity = xVelocity;
        playerBounds.center = player.transform.position + boundsOffset;
    }

    // Change all tracking values and flip animations if needed.  Whole animation container is flipped to allow CheckAhead to point in the right direction.  Define random states if required.
    void LateUpdate()
    {
        if (isJumping)
        {
            if (Input.GetAxis("Jump") == 0)
            {
                isJumping = false;
            }
        }
        velocity = player.GetComponent<Controller>().velocity;
        xVelocity = velocity.x;
        if (xVelocity < -0.1f)
        {
            gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (xVelocity > 0.1f)
        {
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        state = AnimTransition(state);
        animator.SetInteger("State", System.Array.IndexOf(states, state));
        if (state == "sliding" || state == "idling" || state == "jumping")
        {
            RandomStateTrigger();
        }
        Fade();
        if (IsGrounded() && fallTracker > 0)
        {
            fallTracker = 0;
        }
    }

    //Create a random number and change relevant values in Animator component based on this.
    private void RandomStateTrigger()
    {
        idleRandom = Random.Range(0, 10000);
        idleRandom /= 10000;
        if (idleRandom < lollyChance)
        {
            animator.SetBool("LollyTrigger", true);
        }
        else
        {
            animator.SetBool("LollyTrigger", false);
        }
        if (idleRandom > 1 - slideChance)
        {
            animator.SetBool("SlideTrigger", true);
        }
        else
        {
            animator.SetBool("SlideTrigger", false);
        }
        if (idleRandom < randomChance)
        {
            animator.SetBool("RandomTrigger", true);
        }
        else
        {
            animator.SetBool("RandomTrigger", false);
        }
    }

    //Make things behind the player darker as speed increases.
    private void Fade()
    {
        currentFade = Color.Lerp(defaultFade, fadeMax, (velocity.magnitude - runVelocity) / (fullFadeVelocity - runVelocity));
        fade.color = Color.Lerp(fade.color, currentFade, fadeFalloff);
    }

    //Check if there are obstacles in front of the player.
    private bool CheckAhead()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(player.transform.position + boundsOffset, new Vector2(playerBounds.size.x, playerBounds.size.y - 0.1f), 0f, new Vector3(transform.localScale.normalized.x, 0, 0), playerBounds.size.x / 2 + frontCheckProtrusion + Mathf.Abs(xVelocity) * xScalarMultiplier, ignorePlayer);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Platforms"))
            {
                return true;
            }
        }
        return false;
    }

    //Check if the player is approaching the floor.
    private bool IsGrounded()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(player.transform.position + boundsOffset, new Vector2(playerBounds.size.x, playerBounds.size.y), 0f, Vector2.down, groundCheckProtrusion, ignorePlayer);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Platforms"))
            {
                return true;
            }
        }
        return false;
        
    }

    //Change the Animator component's state to falling if the player is in the air for too long.
    private string FallCheck(string currentState)
    {
        fallTracker++;
        if (fallTracker > maxFramesTillFall)
        {
            return "falling";
        }
        else
        {
            return currentState;
        }
    }

    //Change the state of the Animator component based on what the player is doing.
    string AnimTransition(string currentState)
    {
        if (ropeScript.rope != null && state != "grappling" && (ropeScript.contacts[1] - ropeScript.contacts[0]).y > 0)
        {
            currentState = "grappling";
        }
        switch (currentState)
        {
            case "idling":
                audioManager.StopSound("Walk");
                audioManager.StopSound("Run");
                if (!player.GetComponent<CustomPhysics>().grounded)
                {
                    return FallCheck(currentState);
                }
                if (Input.GetAxis("Jump") == 1 && !isJumping)
                {
                    return "jumping";
                }
                else if (Input.GetAxisRaw("Horizontal") != 0 && CheckAhead() == false)
                {
                    return "walking";
                }
                else
                {
                    return currentState;
                }

            case "walking":
                audioManager.PlaySoundComplete("Walk");
                audioManager.StopSound("Run");
                audioManager.StopSound("Slide");
                if (!player.GetComponent<CustomPhysics>().grounded)
                {
                    return FallCheck(currentState);
                }
                else if (Input.GetAxis("Jump") == 1 && !isJumping)
                {
                    return "jumping";
                }
                else if (Mathf.Abs(velocity.x) <= idleVelocityCutoff && Input.GetAxisRaw("Horizontal") == 0)
                {
                    return "idling";
                }
                else if (Mathf.Abs(velocity.x) >= runVelocity)
                {
                    return "running";
                }
                else
                {
                    return currentState;
                }

            case "running":
                audioManager.PlaySoundComplete("Run");
                audioManager.StopSound("Walk");
                audioManager.StopSound("Slide");
                if (!player.GetComponent<CustomPhysics>().grounded)
                {
                    return FallCheck(currentState);
                }
                else if (Input.GetAxis("Jump") == 1 && !isJumping)
                {
                    return "jumping";
                }
                else if (CheckAhead())
                {
                    return "crashing";
                }
                else if (Mathf.Abs(prevXVelocity) - Mathf.Abs(xVelocity) >= skidDelta && Mathf.Abs(xVelocity) < Mathf.Abs(prevXVelocity))
                {
                    return "skidding";
                }
                else if (Mathf.Abs(velocity.x) < runVelocity)
                {
                    return "walking";
                }
                else if (Mathf.Abs(velocity.x) >= slideVelocity)
                {
                    return "sliding";
                }
                else
                {
                    return currentState;
                }

            case "sliding":
                audioManager.StopSound("Walk");
                audioManager.StopSound("Run");
                audioManager.PlaySoundComplete("Slide");
                if (!player.GetComponent<CustomPhysics>().grounded)
                {
                    return FallCheck(currentState);
                }
                else if (Input.GetAxis("Jump") == 1 && !isJumping)
                {
                    return "jumping";
                }
                else if (CheckAhead())
                {
                    return "crashing";
                }
                else if (Mathf.Abs(prevXVelocity) - Mathf.Abs(xVelocity) >= skidDelta && Mathf.Abs(xVelocity) < Mathf.Abs(prevXVelocity))
                {
                    return "skidding";
                }
                else if (Mathf.Abs(velocity.x) < slideVelocity)
                {
                    return "running";
                }
                else
                {
                    return currentState;
                }

            case "skidding":
                audioManager.StopSound("Run");
                if (!player.GetComponent<CustomPhysics>().grounded)
                {
                    return "falling";
                }
                if (Input.GetAxis("Jump") == 1 && !isJumping)
                {
                    return "jumping";
                }
                else if (Mathf.Abs(xVelocity) < Mathf.Abs(prevXVelocity) || CheckAhead() && Mathf.Abs(xVelocity) > idleVelocityCutoff)
                {
                    return currentState;
                }
                else if (Mathf.Abs(xVelocity) < runVelocity)
                {
                    return "walking";
                }
                else if (Mathf.Abs(xVelocity) < idleVelocityCutoff)
                {
                    return "idling";
                }
                else if (Mathf.Abs(xVelocity) > runVelocity)
                {
                    return "running";
                }
                else if (Mathf.Abs(xVelocity) > slideVelocity)
                {
                    return "sliding";
                }
                else
                {
                    return currentState;
                }


            case "jumping":
                if (!isJumping)
                {
                    audioManager.StopSound("Walk");
                    audioManager.StopSound("Slide");
                    audioManager.StopSound("Run");
                    isJumping = true;
                }
                if (jumpDelay == 10)
                {
                    jumpDelay = 0;
                    return "hanging";
                }
                else
                {
                    jumpDelay++;
                    return currentState;
                }

            case "hanging":
                if (IsGrounded())
                {
                    return "landing";
                }
                else if (FallCheck(currentState) == "falling")
                {
                    return "falling";
                }
                else
                {
                    return currentState;
                }

            case "grappling":
                audioManager.StopSound("Walk");
                audioManager.StopSound("Slide");
                audioManager.StopSound("Run");
                if (ropeScript.rope == null)
                {
                    gameObject.transform.localEulerAngles = Vector3.zero;
                    return "falling";
                }
                else
                {
                    Vector2 ropeVector = ropeScript.contacts[1] - ropeScript.contacts[0];
                    float ropeAngle;
                    if (ropeVector.y < 0)
                    {
                        ropeAngle = 0.0f;
                    }
                    else
                    {
                        ropeAngle = Vector3.SignedAngle(Vector3.up, ropeVector, Vector3.forward);
                    }
                    gameObject.transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.localEulerAngles.z, ropeAngle, grappleSpin));
                    return currentState;
                }


            case "falling":
                audioManager.StopSound("Walk");
                audioManager.StopSound("Slide");
                audioManager.StopSound("Run");
                if (IsGrounded())
                {
                    return "landing";
                }
                else
                {
                    return currentState;
                }


            case "crashing":
                if (crashDelay == 10)
                {
                    crashDelay = 0;
                    return "hanging";
                }
                else
                {
                    crashDelay++;
                    return "idling";
                }

            case "landing":
                if (IsGrounded())
                {
                    if (Mathf.Abs(velocity.x) >= slideVelocity)
                    {
                        return "sliding";
                    }
                    else if (Mathf.Abs(velocity.x) >= runVelocity)
                    {
                        return "running";
                    }
                    else if (Mathf.Abs(velocity.x) >= idleVelocityCutoff)
                    {
                        return "walking";

                    }
                    else
                    {
                        return "idling";
                    }
                }
                else
                {
                    return currentState;
                }

            case "winning":
                return currentState;
                

            default:
                return currentState;
        }
    }
}