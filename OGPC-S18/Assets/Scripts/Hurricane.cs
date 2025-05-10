using UnityEngine;
using System.Collections.Generic;

public class Hurricane : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float stormRadius;
    [SerializeField] private float eyeRadius;
    [SerializeField] private float instaKillRadius;
    [SerializeField] private float damage;
    [SerializeField] private float damageInterval;
    private float nextDamageTime = 0f;
    [SerializeField] private AnimationCurve windDistributionCurve;
    [SerializeField] private float densityConst;
    [SerializeField] private Vector2 particleSpeedRange;
    [SerializeField] private float hurricaneSpeed;
    [SerializeField] private float hurricaneWindSpeed;
    [SerializeField] private Gradient hurricaneColor;

    [Header("References")]
    [SerializeField] private GameObject windParticle;
    [SerializeField] private Transform waypointsParent;
    [SerializeField] private bool loopWaypoints;
    [SerializeField] private CircleCollider2D instaKillCollider;
    private List<Vector2> waypoints;
    private int waypointIndex = 0;
    private GameObject player;
    private BoatHealth boatHealth;
    private float distanceToPlayer;
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        boatHealth = player.GetComponent<BoatHealth>();

        ResetWaypoints();

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

    public void ResetWaypoints()
    {
        waypoints = new List<Vector2>();
        foreach (Transform waypoint in waypointsParent)
        {
            waypoints.Add(waypoint.transform.position);
        }
    }

    private void SpawnParticle(float radiusOfParticle)
    {
        GameObject newParticle = Instantiate(windParticle, transform.position + new Vector3(0, radiusOfParticle, 0), Quaternion.identity);
        HurricaneWindParticle hurricaneWindParticle = newParticle.GetComponent<HurricaneWindParticle>();
        hurricaneWindParticle.moveSpeed = Random.Range(particleSpeedRange.x * 10f, particleSpeedRange.y * 10f);
        hurricaneWindParticle.particleColor = hurricaneColor.Evaluate((radiusOfParticle-eyeRadius)/(stormRadius-eyeRadius));
        hurricaneWindParticle.transform.SetParent(transform);
    }

    private void Update()
    {
        // Deal Damage to player
        distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
        if (distanceToPlayer <= stormRadius)
        {
            PlayerInHurricane();
        }

        // Move hurricane

        Vector2 nextWaypoint = waypoints[waypointIndex];
        if (Vector2.Distance(transform.position, nextWaypoint) < 0.2f)
        {
            if (waypointIndex == waypoints.Count - 1)
            {
                if (loopWaypoints) { waypointIndex = 0; }
            }
            else
            {
                waypointIndex += 1;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, nextWaypoint, hurricaneSpeed * Time.deltaTime);
    }

    public bool IsPlayerInHurricane()
    {
        return distanceToPlayer <= stormRadius;
    }

    private void PlayerInHurricane()
    {
        if (Time.time > nextDamageTime)
        {
            nextDamageTime = Time.time + damageInterval;
            float damageToDeal = damage * (1 - (distanceToPlayer - eyeRadius) / (stormRadius - eyeRadius));
            Mathf.Clamp(damageToDeal, 0, Mathf.Infinity);
            boatHealth.TakeDamage(damageToDeal);
        }
    }

    public float GetWindDirection()
    {
        // Get child's position relative to the parent (local space)
        Vector3 localPos = transform.InverseTransformPoint(player.transform.position);

        // Convert to 2D (e.g., x-y plane)
        Vector2 pos2D = new Vector2(localPos.x, localPos.y);

        // Radius
        float r = pos2D.magnitude;

        // Angle in radians
        float theta = Mathf.Atan2(pos2D.y, pos2D.x);

        // Convert to degrees (optional)
        float thetaDeg = theta * Mathf.Rad2Deg;

        //Debug.Log($"Polar Coordinates � Radius: {r}, Angle: {theta} rad ({thetaDeg}�)");

        return (360 - thetaDeg) % 360;
    }

    public float GetWindSpeed()
    {
        return hurricaneWindSpeed * (2 - (distanceToPlayer - eyeRadius) / (stormRadius - eyeRadius));
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