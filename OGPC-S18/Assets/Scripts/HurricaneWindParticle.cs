using UnityEngine;

public class HurricaneWindParticle : MonoBehaviour
{
    [SerializeField] private Vector2 particleLenghtRange;
    private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve dirOffset;
    [SerializeField] private float dirOffsetMult;
    [SerializeField] private float loopTime;
    private float randomStartTime;

    [HideInInspector] public float moveSpeed;
    private Transform parent;

    private float angle;
    private float radius;
    private float pathCircumfrence;

    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.time = Random.Range(particleLenghtRange.x, particleLenghtRange.y);

        parent = transform.parent;
        radius = Vector2.Distance(transform.position, parent.position);
        pathCircumfrence = 2 * Mathf.PI * radius;
        angle = Random.Range(0, 360);

        randomStartTime = Random.Range(0, loopTime);
    }

    private void Update()
    {
        angle += moveSpeed * Time.deltaTime / pathCircumfrence;

        float radiusOffset = dirOffset.Evaluate(((Time.time + randomStartTime) % loopTime) / loopTime) * dirOffsetMult;
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (radius + radiusOffset);
        transform.position = (Vector2)parent.position + offset;
    }
}
