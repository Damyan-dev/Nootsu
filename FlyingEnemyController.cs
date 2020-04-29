using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class FlyingEnemyController : MonoBehaviour
{
    private Transform target;
    private Vector3 originalPos;
    private Animator animator;
    [HideInInspector]
    public int detectedFrames;
    [HideInInspector]
    public bool instantiated = false;
    private bool attacking, detecting, inRange;
    private LevelManager levelManager;
    private AudioManager audioManager;

    public float maxSpeed, playerSpeedReduction;
    public float aggroDistance;
    public float stopDistance;
    public int detectionFrames;

    private void Awake()
    {
        originalPos = transform.position;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        animator.SetInteger("State", 0);
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        audioManager = GameObject.Find("LevelManager").GetComponent<AudioManager>();
    }

    private void Update()
    {
        if (instantiated && originalPos != transform.position)
        {
            originalPos = transform.position;
        }
    }

    private void FixedUpdate()
    {
        if (!attacking)
        {
            inRange = Vector2.Distance(transform.position, target.position) < aggroDistance && Vector2.Distance(transform.position, target.position) > stopDistance;
            if (inRange && !detecting && detectedFrames == 0)
            {
                detecting = true;
                animator.SetInteger("State", 1);
            }
            else if (inRange && detecting && detectedFrames < detectionFrames)
            {
                detectedFrames++;
                audioManager.PlaySoundComplete("EnemyB");
            }
            else if (!inRange && detecting && 0 < detectedFrames && detectedFrames < detectionFrames)
            {
                detectedFrames--;
                if (detectedFrames == 0)
                {
                    detecting = false;
                }
            }
            else if (inRange && detectedFrames == detectionFrames)
            {
                animator.SetInteger("State", 2);
                transform.position = Vector2.MoveTowards(transform.position, target.position, maxSpeed * Time.fixedDeltaTime);
                audioManager.PlaySoundComplete("EnemyB_Attack");

            }
            else if (Vector2.Distance(transform.position, target.position) < aggroDistance && detectedFrames == detectionFrames)
            {
                return;
            }
            else if (!inRange && detecting && detectionFrames == detectedFrames)
            {
                detectedFrames--;
                animator.SetInteger("State", 1);
                detecting = true;
            }
            else
            {
                animator.SetInteger("State", 0);
                transform.position = Vector2.MoveTowards(transform.position, originalPos, maxSpeed * Time.fixedDeltaTime);

            }
            if (Vector2.Distance(transform.position, target.position) <= stopDistance)
            {
                Attack();
            }
            Flip();
        }
        else
        {
            Attack();
        }
    } 

    private void Flip()
    {
        Vector3 flip = transform.eulerAngles;
        if(transform.position.x > target.position.x)
        {
            flip.y = 180f;
        }
        else
        {
            flip.y = 0f;
        }
        transform.eulerAngles = flip;
    }

    private void Attack()
    {
        if (!attacking)
        {
            attacking = true;
            animator.SetInteger("State", 3);
            GetComponent<CircleCollider2D>().enabled = false;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, target.position, 0.5f);
        }
    }

    private void Destroy()
    {
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<Controller>().maxSpeed > GameObject.FindGameObjectWithTag("Player").GetComponent<Controller>().minSpeed)
        {
            target.gameObject.GetComponent<Controller>().maxSpeed -= playerSpeedReduction;
        }
        levelManager.trackers++;
        Destroy(gameObject);
    }
}
