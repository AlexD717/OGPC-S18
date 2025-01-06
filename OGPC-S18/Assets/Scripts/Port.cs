using UnityEngine;
using UnityEngine.InputSystem;

public class Port : MonoBehaviour
{
    private bool playerWithinRange = false;
    private bool playerDocked = false;

    [SerializeField] private GameObject range;
    private SpriteRenderer rangeSprite;

    [SerializeField] private Transform playerDockPositionsParent;
    private Transform[] playerDockPositions;

    [SerializeField] private InputActionAsset inputActions;
    private InputAction interact;

    private GameObject player;
    private BoatController boatController;

    private void Start()
    {
        rangeSprite = range.GetComponent<SpriteRenderer>();
        rangeSprite.enabled = false;

        player = GameObject.FindGameObjectWithTag("Player");

        playerDockPositions = new Transform[playerDockPositionsParent.childCount];
        for (int i = 0; i < playerDockPositionsParent.childCount; i++)
        {
            playerDockPositions[i] = playerDockPositionsParent.GetChild(i).transform;
        }
    }

    private void OnEnable()
    {
        var playerControls = inputActions.FindActionMap("Player");
        interact = playerControls.FindAction("Interact");

        interact.Enable();
    }

    private void OnDisable()
    {
        interact.Disable();
    }

    private void Update()
    {
        if (playerWithinRange)
        {
            if (interact.triggered)
            {
                if (!playerDocked)
                {
                    boatController.Dock(GetClosestDockPosition());
                    playerDocked = true;
                    rangeSprite.enabled = false;
                }
                else
                {
                    boatController.UnDock();
                    playerDocked = false;
                    rangeSprite.enabled = true;
                }
            }
        }
    }

    private Transform GetClosestDockPosition()
    {
        float smallestDistance = Mathf.Infinity;
        Transform closestDock = null;

        foreach (Transform dock in playerDockPositions)
        {
            float distanceBetween = Vector2.Distance(dock.position, player.transform.position);
            if (distanceBetween < smallestDistance)
            {
                smallestDistance = distanceBetween;
                closestDock = dock;
            }
        }

        return closestDock;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       if (collision.tag == "Player")
        {
            playerWithinRange = true;

            rangeSprite.enabled = true;
            boatController = collision.GetComponent<BoatController>();
        } 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerWithinRange = false;

            rangeSprite.enabled = false;
        }
    }
}