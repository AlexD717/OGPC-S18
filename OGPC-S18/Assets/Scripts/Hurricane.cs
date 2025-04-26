using UnityEngine;
using System.Collections.Generic;

public class Hurricane : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float stormRadius;
    [SerializeField] private float eyeRadius;
    [SerializeField] private float instaKillRadius;
    [SerializeField] private AnimationCurve windDistributionCurve;
    [SerializeField] private float densityConst;
    [SerializeField] private Vector2 particleSpeedRange;
    [SerializeField] private float hurricaneSpeed;

    [Header("References")]
    [SerializeField] private GameObject windParticle;
    [SerializeField] private Transform waypointsParent;
    [SerializeField] private CircleCollider2D instaKillCollider;
    private List<Transform> waypoints;

    private void Start()
    {
        List<Transform> tempChildren = new List<Transform>();
        foreach (Transform waypoint in waypointsParent)
        {
            tempChildren.Add(waypoint);
        }

        waypoints = new List<Transform>();
        foreach (Transform waypoint in tempChildren)
        {
            waypoints.Add(waypoint);
            waypoint.transform.SetParent(null);
        }

        // Spawn particles in a circle around the eye
        float pos = eyeRadius;
        while (pos < stormRadius)
        {
            SpawnParticle(pos);
            float windDistributionCurveValue = windDistributionCurve.Evaluate((pos - eyeRadius) / (stormRadius - eyeRadius));
            float step = windDistributionCurveValue * densityConst;
            step = Mathf.Clamp(step, 0.05f, Mathf.Infinity);

            pos += step;
        }

        instaKillCollider.radius = instaKillRadius;
    }

    private void SpawnParticle(float radiusOfParticle)
    {
        GameObject newParticle = Instantiate(windParticle, transform.position + new Vector3(0, radiusOfParticle, 0), Quaternion.identity);
        HurricaneWindParticle hurricaneWindParticle = newParticle.GetComponent<HurricaneWindParticle>();
        hurricaneWindParticle.moveSpeed = Random.Range(particleSpeedRange.x * 10f, particleSpeedRange.y * 10f);
        hurricaneWindParticle.transform.SetParent(transform);
    }

    private void Update()
    {
        // Move hurricane
        if (waypoints.Count == 0) { return; }

        Transform nextWaypoint = waypoints[0];
        if (Vector2.Distance(transform.position, nextWaypoint.position) < 0.2f)
        {
            waypoints.RemoveAt(0);
            if (waypoints.Count == 0)
            {
                Debug.Log("Hurricane has reached the last waypoint");
            }
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, nextWaypoint.position, hurricaneSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, eyeRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stormRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, instaKillRadius);
    }
}