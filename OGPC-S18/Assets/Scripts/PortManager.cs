using UnityEngine;

public class PortManager : MonoBehaviour
{
    [SerializeField] private GameObject questCanvas;
    private GameObject player;
    private BoatController boatController;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        boatController = player.GetComponent<BoatController>();

        questCanvas.SetActive(false);
    }

    public void PlayerDocked(Transform[] playerDockPositions)
    {
        questCanvas.SetActive(true);
        boatController.Dock(GetClosestDockPosition(playerDockPositions));
    }

    public void PlayerUndocked()
    {
        questCanvas.SetActive(false);
        boatController.UnDock();
    }

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
