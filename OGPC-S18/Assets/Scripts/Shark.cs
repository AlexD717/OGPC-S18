using UnityEngine;

public class Shark : MonoBehaviour
{
    [Header("Shark Settings")]
    [SerializeField] private float detectionRadius = 10f; // Radius within which the shark detects the player
    [SerializeField] private float swimSpeed;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform waypointParent;

    private Transform currentWaypoint;
    private int currentWaypointIndex = 0;
    private int previousState = -1; // Initialize to an invalid state
    private bool lockedOnTarget = false;
    private bool diveFinished = false;

    private Transform player;

    private Animator animator;
    private enum SharkAnimState
    {
        Swim,
        Dive,
        Attack,
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();

        waypointParent.parent = null; // Detach from parent to avoid unwanted transformations
    }

    private void Update()
    {
        if (diveFinished)
        {
            transform.position = player.position;
            transform.rotation = player.rotation;
            return;
        }

        // Identifies the target state
        int targetState = (int)SharkAnimState.Swim; // Default state
        if (lockedOnTarget)
        {
            targetState = (int)SharkAnimState.Attack;
        }
        else if (Vector2.Distance(player.position, transform.position) <= detectionRadius)
        {
            // shark has detected the player
            targetState = (int)SharkAnimState.Dive;
            lockedOnTarget = true; // Shark has locked onto the player
        }

        if (previousState != targetState)
        {
            if (targetState == (int)SharkAnimState.Dive)
            {
                animator.SetTrigger("Dive");
            }
        }
        previousState = targetState;

        // If the state is "Swim", move towards the next waypoint
        if (targetState == (int)SharkAnimState.Swim)
        {
            MoveTowardsNextWaypoint();
        }
    }

    public void DiveFinished()
    {
        // Called when the dive animation is finished
        diveFinished = true;
        Invoke("JumpOutOfWater", Vector2.Distance(player.position, transform.position) / attackSpeed); // Delay the jump out of water based on distance
    }

    private void JumpOutOfWater()
    {
        animator.SetTrigger("Attack");
    }

    public void AttackFinished()
    {
        // Called when the attack animation is finished
        player.GetComponent<BoatHealth>().TakeDamage(Mathf.Infinity); // Kill the player
    }

    private void MoveTowardsNextWaypoint()
    {
        if (currentWaypoint != null)
        {
            // Move towards the current waypoint
            Vector2 direction = (currentWaypoint.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, currentWaypoint.position, Time.deltaTime * swimSpeed);

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
