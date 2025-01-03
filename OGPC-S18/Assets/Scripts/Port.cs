using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Port : MonoBehaviour
{
    private bool playerWithinRange = false;
    private bool playerDocked = false;

    [SerializeField] private GameObject range;
    private SpriteRenderer rangeSprite;

    [SerializeField] private Transform playerDockPosition;

    [SerializeField] private InputActionAsset inputActions;
    private InputAction interact;

    private BoatController boatController;

    private void Start()
    {
        rangeSprite = range.GetComponent<SpriteRenderer>();
        rangeSprite.enabled = false;
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
                    boatController.Dock(playerDockPosition);
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