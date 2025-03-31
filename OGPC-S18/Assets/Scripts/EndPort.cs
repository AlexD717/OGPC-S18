using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class EndPort : MonoBehaviour
{
    [SerializeField] private string portName;

    private bool playerWithinRange = false;
    private bool playerDocked = false;

    [SerializeField] private GameObject range;
    private SpriteRenderer rangeSprite;

    [SerializeField] private Transform playerDockPositionsParent;
    private Transform[] playerDockPositions;
    private BoatController boatController;
    private LevelManager levelManager;

    [SerializeField] private InputActionAsset inputActions;
    private InputAction interact;

    [SerializeField] private RectTransform worldCanvas;
    [HideInInspector] public TextMeshProUGUI nameText;
    GameObject player;


    private void Start()
    {
        rangeSprite = range.GetComponent<SpriteRenderer>();
        rangeSprite.enabled = false;

        playerDockPositions = new Transform[playerDockPositionsParent.childCount];
        for (int i = 0; i < playerDockPositionsParent.childCount; i++)
        {
            playerDockPositions[i] = playerDockPositionsParent.GetChild(i);
        }

        worldCanvas.rotation = Quaternion.identity;
        nameText = worldCanvas.GetChild(0).GetComponent<TextMeshProUGUI>();
        nameText.text = portName;

        boatController = GameObject.FindGameObjectWithTag("Player").GetComponent<BoatController>();
        levelManager = FindFirstObjectByType<LevelManager>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnEnable()
    {
        InputActionMap playerControls = inputActions.FindActionMap("Player");
        interact = playerControls.FindAction("Interact");

        interact.Enable();
    }

    private void OnDisable()
    {
        interact.Disable();
    }

    private void Update()
    {
        nameText.transform.rotation = Quaternion.Euler(0,0,player.transform.rotation.eulerAngles.z);
        if (playerWithinRange && interact.triggered)
        {
            if (!playerDocked)
            {
                Dock();
                boatController.Dock(UsefulStuff.GetClosestPosition(playerDockPositions, boatController.gameObject));
            }
        }
    }

    private void Dock()
    {
        playerDocked = true;
        rangeSprite.enabled = false;
        nameText.enabled = false;
        levelManager.PlayerReachedEndPort();
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