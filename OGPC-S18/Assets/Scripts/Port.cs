using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Port : MonoBehaviour
{
    [SerializeField] private string portName;

    private bool playerWithinRange = false;
    private bool playerDocked = false;

    [SerializeField] private GameObject range;
    private SpriteRenderer rangeSprite;

    [SerializeField] private Transform playerDockPositionsParent;
    private Transform[] playerDockPositions;

    [SerializeField] private InputActionAsset inputActions;
    private InputAction interact;

    [SerializeField] private RectTransform worldCanvas;
    [HideInInspector] public TextMeshProUGUI nameText;

    private PortManager portManager;

    [SerializeField] private GameObject dockCanvas;
    private GameObject selectedQuestPanel;
    private Transform dockPanel;
    private GameObject[] dockPanelMenus;
    private TextMeshProUGUI dockedTextIndicator;
    private TextMeshProUGUI saveTimerText;
    public bool portSaved{ get {return portSaved;} private set { portSaved = value;}}
    [SerializeField] private float timeToSavePort = 5f;
    private float playerDockedTime = 0f;
    GameObject player;
    private void Start()
    {
        portSaved = false;
        player = GameObject.FindWithTag("Player");
        rangeSprite = range.GetComponent<SpriteRenderer>();
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

        dockedTextIndicator = dockCanvas.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        dockedTextIndicator.text = "Docked at " + nameText.text;

        saveTimerText = dockCanvas.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        saveTimerText.text = "Saved in " + timeToSavePort.ToString("F2") + "s";

        selectedQuestPanel = dockCanvas.transform.GetChild(1).gameObject;
        selectedQuestPanel.SetActive(false); // Deactivates accepeted quest menu

        // Gets all the menus under dockPanel and puts them in the panelMenus array
        dockPanel = dockCanvas.transform.GetChild(0);
        dockPanelMenus = new GameObject[dockPanel.childCount];
        for (int i = 0; i < dockPanel.childCount; i++)
        {
            dockPanelMenus[i] = dockPanel.GetChild(i).gameObject;
        }
        dockCanvas.SetActive(false);
        SelectMenu(0);
        /*
         *  panelMenus[0] = Main Menu
         *  paznelMenus[1] = Quests
        */
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
            if (playerDockedTime > timeToSavePort)
            {
                portSaved = true;
                saveTimerText.text = "Saved!"; //Save the port
            }
            else
            {
                saveTimerText.text = "Saved in " + (timeToSavePort - playerDockedTime).ToString("F2") + "s";//Count down time remaining
            }
        }
    }
    private void Dock()
    {
        dockCanvas.SetActive(true);
        SelectMenu(0);
        portManager.PlayerDocked(playerDockPositions, dockPanelMenus, this);
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

    // Makes only one panel active
    public void SelectMenu(int childMenuIndex)
    {
        for (int i = 0; i < dockPanelMenus.Length; i++)
        {
            if (i == childMenuIndex)
            {
                dockPanelMenus[i].SetActive(true);
            }
            else
            {
                dockPanelMenus[i].SetActive(false);
            }
        }

        selectedQuestPanel.SetActive(false);
    }
}