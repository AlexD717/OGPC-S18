using UnityEngine;

public class WindParticle : MonoBehaviour
{
    [SerializeField] private AnimationCurve windYOffset;
    [SerializeField] private float windYOffsetMult;
    [SerializeField] private float speedConstant;
    [SerializeField] private Vector2 timeAliveRange;
    [SerializeField] private Vector2 lenghtRange;
    private float timeAlive;
    private float startTime;

    [HideInInspector] public float windAngle;
    [HideInInspector] public float windSpeed;

    private TrailRenderer trailRenderer;

    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.time = Random.Range(lenghtRange.x, lenghtRange.y);

        timeAlive = Random.Range(timeAliveRange.x, timeAliveRange.y);
        startTime = Time.time;

        Destroy(gameObject, timeAlive);

        transform.rotation = Quaternion.Euler(0, 0, windAngle * Mathf.Rad2Deg);
    }

    private void Update()
    {
        float yOffset = windYOffset.Evaluate((Time.time-startTime)/(timeAlive));
        transform.position += transform.up * yOffset * windYOffsetMult * Time.deltaTime;

        transform.position += transform.right * windSpeed * speedConstant * Time.deltaTime;
    }
}
