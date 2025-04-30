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

    public void PlayerDocked(Transform[] playerDockPositions, Port dockedPort)
    {
        boatController.Dock(UsefulStuff.GetClosestPosition(playerDockPositions, boatController.gameObject));
    }

    public void PlayerUndocked()
    {
        boatController.UnDock();
    }
}