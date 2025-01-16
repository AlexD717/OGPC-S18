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

    [SerializeField] private RectTransform worldCanvas;

    private PortManager portManager;

    private void Start()
    {
        rangeSprite = range.GetComponent<SpriteRenderer>();
        rangeSprite.enabled = false;

        portManager = FindFirstObjectByType<PortManager>();

        playerDockPositions = new Transform[playerDockPositionsParent.childCount];
        for (int i = 0; i < playerDockPositionsParent.childCount; i++)
        {
            playerDockPositions[i] = playerDockPositionsParent.GetChild(i).transform;
        }

        worldCanvas.rotation = Quaternion.identity;
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
                    Dock();

                }
                else
                {
                    UnDock();
                }
            }
        }
    }

    private void Dock()
    {
        portManager.PlayerDocked(playerDockPositions);
        playerDocked = true;
        rangeSprite.enabled = false;
    }

    private void UnDock()
    {
        portManager.PlayerUndocked();
        playerDocked = false;
        rangeSprite.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       if (collision.tag == "Player")
        {
            playerWithinRange = true;

            rangeSprite.enabled = true;
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