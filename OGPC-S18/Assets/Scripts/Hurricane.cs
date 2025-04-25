using UnityEngine;
using System.Collections.Generic;

public class Hurricane : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float stormRadius;
    [SerializeField] private float eyeRadius;
    [SerializeField] private AnimationCurve windDistributionCurve;
    [SerializeField] private float densityConst;
    [SerializeField] private Vector2 particleSpeedRange;

    [Header("References")]
    [SerializeField] private GameObject windParticle;
    [SerializeField] private Transform waypointsParent;
    private List<Transform> waypoints;


    private void Start()
    {
        waypoints = new List<Transform>();
        foreach (Transform waypoint in waypointsParent)
        {
            waypoints.Add(waypoint);
        }

        // Spawn particles in a circle around the eye
        float pos = eyeRadius;
        while (pos < stormRadius)
        {
            SpawnParticle(pos);
            float windDistributionCurveValue = windDistributionCurve.Evaluate((pos - eyeRadius) / (stormRadius - eyeRadius));
            float step = windDistributionCurveValue * densityConst;
            step = Mathf.Clamp(step, 0.1f, Mathf.Infinity);

            pos += step;
        }
    }

    private void SpawnParticle(float radiusOfParticle)
    {
        GameObject newParticle = Instantiate(windParticle, transform.position + new Vector3(0, radiusOfParticle, 0), Quaternion.identity);
        HurricaneWindParticle hurricaneWindParticle = newParticle.GetComponent<HurricaneWindParticle>();
        hurricaneWindParticle.rotateSpeed = Random.Range(particleSpeedRange.x, particleSpeedRange.y);
        hurricaneWindParticle.transform.SetParent(transform);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, eyeRadius);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, stormRadius);
    }
}