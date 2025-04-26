using UnityEngine;

public class HurricaneWindParticle : MonoBehaviour
{
    [SerializeField] private Vector2 particleLenghtRange;
    private TrailRenderer trailRenderer;

    [HideInInspector] public float moveSpeed;
    private Transform parent;
    private float pathCircumfrence;

    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.time = Random.Range(particleLenghtRange.x, particleLenghtRange.y);

        parent = transform.parent;
        pathCircumfrence = Vector2.Distance(transform.position, parent.position) * 2 * Mathf.PI;

        transform.RotateAround(parent.position, new Vector3(0, 0, 1), Random.Range(0, 360)); // Random start angle
    }

    private void Update()
    {
        transform.RotateAround(parent.position, new Vector3(0, 0, 1), moveSpeed * Time.deltaTime / pathCircumfrence);
    }
}
