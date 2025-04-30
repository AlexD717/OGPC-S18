using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Port : MonoBehaviour
{
    [SerializeField] private string portName;

    private bool playerWithinRange = false;
    private bool playerDocked = false;
    private SpriteRenderer rangeSprite;

    [SerializeField] private Transform playerDockPositionsParent;
    private Transform[] playerDockPositions;

    [SerializeField] private InputActionAsset inputActions;
    private InputAction interact;

    [SerializeField] private RectTransform worldCanvas;
    [HideInInspector] public TextMeshProUGUI nameText;

    private PortManager portManager;

    [SerializeField] private GameObject dockCanvas;
    private TextMeshProUGUI dockedTextIndicator;
    private TextMeshProUGUI saveTimerText;

    public bool portSaved { get; private set; } = false;
    [SerializeField] private float timeToSavePort;
    private float playerDockedTime = 0f;
    GameObject player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        rangeSprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
        rangeSprite.enabled = false;

        portManager = FindFirstObjectByType<PortManager>();

        playerDockPositions = new Transform[playerDockPositionsParent.childCount];
        for (int i = 0; i < playerDockPositionsParent.childCount; i++)
        {
            playerDockPositions[i] = playerDockPositionsParent.GetChild(i);
        }

        worldCanvas.rotation = Quaternion.identity;
        nameText = worldCanvas.GetChild(0).GetComponent<TextMeshProUGUI>();
        nameText.text = portName;

        dockedTextIndicator = dockCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        dockedTextIndicator.text = "Docked at " + nameText.text;

        saveTimerText = dockCanvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        saveTimerText.text = "Saved in " + timeToSavePort.ToString("F2") + "s";

        dockCanvas.SetActive(false);
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
            if (playerDocked)
            {
                UnDock();
            }
            else
            {
                Dock();
            }
        }
        if (playerDocked && !portSaved)
        {
            playerDockedTime += Time.deltaTime;
            playerDockedTime = Mathf.Clamp(playerDockedTime, 0, timeToSavePort);
            if (playerDockedTime >= timeToSavePort)
            {
                portSaved = true;
                saveTimerText.text = "Saved!"; //Save the port
            }
            else
            {
                saveTimerText.text = "Saved in " + (timeToSavePort - playerDockedTime).ToString("F2") + "s";//Count down time remaining
            }
        }
        if (playerDocked && portSaved)
        {
            Debug.Log("Port saved!");
        }
    }
    private void Dock()
    {
        dockCanvas.SetActive(true);
        portManager.PlayerDocked(playerDockPositions, this);
        playerDocked = true;
        rangeSprite.enabled = false;
        nameText.enabled = false;
    }

    private void UnDock()
    {
        dockCanvas.SetActive(false);
        portManager.PlayerUndocked();
        playerDocked = false;
        rangeSprite.enabled = true;
        nameText.enabled = true;
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