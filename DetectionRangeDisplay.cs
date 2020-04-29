using UnityEngine;

public class DetectionRangeDisplay : MonoBehaviour
{
    private bool enemyDetectorEnabled = false;
    private float detectionRange;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("detectorEnabled", 0) == 1)
        {
            enemyDetectorEnabled = true;
        }
        else
        {
            enemyDetectorEnabled = false;
        }
        if (transform.parent.CompareTag("EnemyA"))
        {
            detectionRange = transform.parent.GetComponent<EnemyController>().aggroDistance * 10;
        }
        else
        {
            detectionRange = transform.parent.GetComponent<FlyingEnemyController>().aggroDistance * 10;
        }
        if (enemyDetectorEnabled)
        {
            transform.localScale = new Vector3(detectionRange, detectionRange, 0);
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
