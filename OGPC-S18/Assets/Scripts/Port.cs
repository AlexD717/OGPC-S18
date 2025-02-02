using TMPro;
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
    private QuestManager questManager;

    [SerializeField] private GameObject dockCanvas;
    private Transform dockPanel;
    private GameObject[] dockPanelMenus;
    private TextMeshProUGUI dockedTextIndicator;

    private void Start()
    {
        rangeSprite = range.GetComponent<SpriteRenderer>();
        rangeSprite.enabled = false;

        portManager = FindFirstObjectByType<PortManager>();
        questManager = FindFirstObjectByType<QuestManager>();

        playerDockPositions = new Transform[playerDockPositionsParent.childCount];
        for (int i = 0; i < playerDockPositionsParent.childCount; i++)
        {
            playerDockPositions[i] = playerDockPositionsParent.GetChild(i);
        }

        worldCanvas.rotation = Quaternion.identity;
        nameText = worldCanvas.GetChild(0).GetComponent<TextMeshProUGUI>();
        nameText.text = portName;

        dockedTextIndicator = dockCanvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        dockedTextIndicator.text = "Docked at " + nameText.text;

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

        dockCanvas.transform.GetChild(2).gameObject.SetActive(false); // Deactivates accepeted quest menu
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
        dockCanvas.SetActive(true);
        SelectMenu(0);
        questManager.PlayerDocked(this, dockCanvas);
        portManager.PlayerDocked(playerDockPositions, dockPanelMenus);
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
    }

}