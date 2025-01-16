using TMPro;
using UnityEngine;

public class PortManager : MonoBehaviour
{
    [SerializeField] private GameObject dockCanvas;
    private Transform dockPanel;
    private GameObject[] dockPanelMenus;
    private TextMeshProUGUI dockedTextIndicator;

    private GameObject player;
    private BoatController boatController;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        boatController = player.GetComponent<BoatController>();

        dockedTextIndicator = dockCanvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        // Gets all the menus under dockPanel and puts them in the panelMenus array
        dockPanel = dockCanvas.transform.GetChild(0);
        dockPanelMenus = new GameObject[dockPanel.childCount];
        for (int i = 0; i < dockPanel.childCount; i++)
        {
            dockPanelMenus[i] = dockPanel.GetChild(i).gameObject;
        }
        /*
         *  panelMenus[0] = Main Menu
         *  paznelMenus[1] = Quests
        */
        SelectMenu(0); // Sets Main Menu as the active menu

        dockCanvas.SetActive(false);
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

    public void PlayerDocked(Transform[] playerDockPositions, string portName)
    {
        dockCanvas.SetActive(true);
        dockedTextIndicator.text = "Docked at " + portName;
        boatController.Dock(GetClosestDockPosition(playerDockPositions));
    }

    public void PlayerUndocked()
    {
        SelectMenu(0);
        dockCanvas.SetActive(false);
        boatController.UnDock();
    }

    // Returns transform of closest dock position from the player, giving possible player dock positions
    private Transform GetClosestDockPosition(Transform[] playerDockPositions)
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
}