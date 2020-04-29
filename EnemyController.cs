using System.Collections;
using UnityEngine;



public class EnemyController : CustomPhysics
{
    private Transform target;
    public float speed, aggroDistance, fireDistance, leftLimit, rightLimit, patrolWidth, timeBetweenShots;
    public int detectionFrames;
    public LayerMask layerMask;
    public GameObject EnemyB;
    private int detectedFrames = 0;
    private bool detecting, firing;
    private Animator animator;
    private AudioManager audioManager;

  
    protected float direction = 1.0f;
    

    protected Vector2 patrolAmount;
    
    private void Start()
    {
        audioManager = GameObject.Find("LevelManager").GetComponent<AudioManager>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();

        leftLimit = transform.position.x - patrolWidth / 2;
        rightLimit = transform.position.x + patrolWidth / 2;
    }

    protected override void CalculateVelocity()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        float targetDistance = Vector2.Distance(transform.position, target.position);

        if(targetDistance < aggroDistance)
        {
            Aggro();
        }
        else
        {
            Deaggro();
        }

        patrolAmount.x = direction * speed * Time.deltaTime;

        if (direction > 0.0f && transform.position.x >= rightLimit && !detecting)
        {
            direction = -1.0f;
        }
        else if(direction < 0.0f && transform.position.x <= leftLimit && !detecting)
        {
            direction = 1.0f;
        }
        transform.Translate(patrolAmount);
        Flip();
    }

    private void Aggro()
    {
        if (!detecting && detectedFrames == 0)
        {
            detecting = true;
            animator.SetInteger("State", 1);
            direction = 0.0f;
        }
        else if (detecting && detectedFrames < detectionFrames)
        {
            detectedFrames++;
            audioManager.PlaySoundComplete("EnemyA");
        }
        else if (detectedFrames == detectionFrames && Vector2.Distance(transform.position, target.position) > fireDistance)
        {
            animator.SetInteger("State", 2);
            if (transform.position.x < target.position.x)
            {
                direction = 1.0f;
                rb.velocity = new Vector2(speed, 0);

            }
            else
            {
                direction = -1.0f;
                rb.velocity = new Vector2(-speed, 0);
            }
        }
        else if (Vector2.Distance(transform.position, target.position) < fireDistance && detectedFrames == detectionFrames && !firing)
        {
            StartCoroutine(Fire());
            audioManager.PlaySoundComplete("Shooting");
        }
        else if (detectedFrames == detectionFrames && Vector2.Distance(transform.position, target.position) < fireDistance)
        {
            animator.SetInteger("State", 2);
            direction = 0.0f;
        }
    }

    private void Deaggro()
    {
        rb.velocity = Vector2.zero;
        if (firing)
        {
            firing = false;
        }
        if (detecting && 0 < detectedFrames && detectedFrames < detectionFrames)
        {
            detectedFrames--;
            if (detectedFrames == 0)
            {
                detecting = false;
                animator.SetInteger("State", 0);
            }
        }
        else if (detecting && detectionFrames == detectedFrames)
        {
            detectedFrames--;
            animator.SetInteger("State", 1);
            direction = 0.0f;
        }
        else if (direction == 0.0f && detectedFrames == 0)
        {
            animator.SetInteger("State", 0);
            if (transform.position.x - rightLimit > transform.position.x - leftLimit)
            {
                direction = 1.0f;
            }
            else
            {
                direction = -1.0f;
            }
        }
    }

    private IEnumerator Fire()
    {
        animator.SetInteger("State", 3);
        firing = true;
        direction = 0.0f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, target.position - transform.position, aggroDistance, layerMask);
        if (hit.collider.CompareTag("Player"))
        {
            GameObject newEnemyB = Instantiate(EnemyB, transform.position + Vector3.up * 0.1f, Quaternion.identity);
            FlyingEnemyController script = newEnemyB.GetComponent<FlyingEnemyController>();
            script.detectedFrames = script.detectionFrames;
            script.instantiated = true;
        }
        yield return new WaitForSeconds(timeBetweenShots);
        firing = false;
    }

    private void Flip()
    {
        SpriteRenderer flip = GetComponent<SpriteRenderer>();
        if ((transform.position.x > target.position.x && (firing || detecting)) || direction == -1.0f)
        {
            flip.flipX = true;
        }
        else
        {
            flip.flipX = false;
        }
    }
}
