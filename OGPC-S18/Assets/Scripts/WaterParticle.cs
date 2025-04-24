using UnityEngine;

public class WaterParticle : MonoBehaviour
{
    [SerializeField] private AnimationCurve waterYOffset;
    [SerializeField] private float waterYOffsetMult;
    [SerializeField] private float speedConstant;
    [SerializeField] private Vector2 timeAliveRange;
    private float timeAlive;
    private float startTime;

    [SerializeField] private float scaleDownConstant;

    [HideInInspector] public float currentAngle;
    [HideInInspector] public float currentSpeed;

    private bool disapearing = false;


    private void Start()
    {
        timeAlive = Random.Range(timeAliveRange.x, timeAliveRange.y);
        startTime = Time.time;

        Invoke("Disapear", timeAlive);

        transform.rotation = Quaternion.Euler(0, 0, currentAngle * Mathf.Rad2Deg);

        if (currentSpeed < 1)
        {
            currentSpeed = 1;
            transform.Rotate(0, 0, 180);
        }
    }

    private void Update()
    {
        float yOffset = waterYOffset.Evaluate((Time.time - startTime) / (timeAlive));
        transform.position += transform.up * yOffset * waterYOffsetMult * Time.deltaTime;

        transform.position += transform.right * currentSpeed * speedConstant * Time.deltaTime;

        // Slowly scales down particle when time to hide
        if (disapearing)
        {
            if (transform.localScale.x > 0.1)
            {
                transform.localScale -= new Vector3(1, 1, 0) * Time.deltaTime * scaleDownConstant;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void Disapear()
    {
        disapearing = true;
    }
}
