using UnityEngine;

public class Shark : MonoBehaviour
{
    [SerializeField] private Transform waypointParent;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    private Transform currentWaypoint;
    private int currentWaypointIndex = 0;

    private void Start()
    {
        waypointParent.parent = null; // Detach from parent to avoid unwanted transformations
    }

    private void Update()
    {
        if (currentWaypoint != null)
        {
            // Move towards the current waypoint
            Vector2 direction = (currentWaypoint.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, currentWaypoint.position, Time.deltaTime * speed);

            // Smoothly rotate towards the target angle
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Check if the shark has reached the current waypoint
            if (Vector2.Distance(transform.position, currentWaypoint.position) < 0.1f)
            {
                // Move to the next waypoint
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypointParent.childCount)
                {
                    currentWaypointIndex = 0; // Loop back to the first waypoint
                }
                currentWaypoint = waypointParent.GetChild(currentWaypointIndex);
            }
        }
        else
        {
            // Start moving towards the first waypoint
            currentWaypoint = waypointParent.GetChild(currentWaypointIndex);
        }
    }
}
