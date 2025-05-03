using UnityEngine;

public class PortManager : MonoBehaviour
{
    private GameObject player;
    private BoatController boatController;
    private SFXManager sfxManager;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        boatController = player.GetComponent<BoatController>();
        sfxManager = FindFirstObjectByType<SFXManager>();
    }

    public void PlayerDocked(Transform[] playerDockPositions, Port dockedPort)
    {
        boatController.Dock(UsefulStuff.GetClosestPosition(playerDockPositions, boatController.gameObject));

        sfxManager.PlayerDocked();
    }

    public void PlayerUndocked()
    {
        boatController.UnDock();

        sfxManager.PlayerUndocked();
    }
}