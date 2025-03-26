using TMPro;
using UnityEngine;

public class PortManager : MonoBehaviour
{
    private GameObject player;
    private BoatController boatController;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        boatController = player.GetComponent<BoatController>();
    }

    public void PlayerDocked(Transform[] playerDockPositions, GameObject[] dockPanelMenus, Port dockedPort)
    {
        boatController.Dock(GetClosestDockPosition(playerDockPositions));
    }

    public void PlayerUndocked()
    {
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