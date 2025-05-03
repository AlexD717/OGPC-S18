using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject instructionsParent;
    private GameObject[] popUps;
    private int popUpIndex = 0;
    [SerializeField] private InputActionAsset inputActions;
    private InputActionMap playerInputActions;
    private InputActionMap mapInputActions;
    [SerializeField] private TutorialTarget tutorialTarget;

    private GameObject player;

    private bool firstFrame = true;

    private void Start()
    {
        playerInputActions = inputActions.FindActionMap("Player");
        mapInputActions = inputActions.FindActionMap("Map");
        playerInputActions.Enable();
        mapInputActions.Enable();

        popUps = new GameObject[instructionsParent.transform.childCount];
        for (int i = 0; i < instructionsParent.transform.childCount; i++)
        {
            popUps[i] = instructionsParent.transform.GetChild(i).gameObject;
        }

        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<BoatHealth>().tutorialLevel = true;
    }

    void Update()
    {
        if (firstFrame)
        {
            // Disable the actions the player hasn't learned yet
            playerInputActions.FindAction("Rotation").Disable();
            playerInputActions.FindAction("SailToggle").Disable();
            Debug.Log("Sail toggle disabled");
            firstFrame = false;
        }

        Debug.Log(popUpIndex);

        // Only show the active pop-up
        for (int i = 0; i < popUps.Length; i++)
            popUps[i].SetActive(i == popUpIndex);

        switch (popUpIndex)
        {
            case 0:
                Time.timeScale = 0f;
                // Teach the player that they can open the map
                if (mapInputActions.FindAction("MapToggle").triggered)
                {
                    Time.timeScale = 1f;
                    popUpIndex++;
                }
                break;

            case 1:
                // Teach the player that they can pan around the map
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    popUpIndex++;
                }
                break;

            case 2:
                // Teach the player that they can zoom in and out
                if (Mathf.Abs(mapInputActions.FindAction("MapZoom").ReadValue<float>()) > 0.1f)
                {
                    popUpIndex++;
                }
                break;

            case 3:
                // Teach the player to disable the map
                if (mapInputActions.FindAction("MapToggle").triggered)
                {
                    playerInputActions.FindAction("Rotation").Enable();
                    popUpIndex++;
                }
                break;

            case 4:
                // Teach the player that they can rotate the boat
                playerInputActions.FindAction("SailToggle").Disable(); // Gets enabled after map close
                Time.timeScale = 0f;
                if (Mathf.Abs(playerInputActions.FindAction("Rotation").ReadValue<float>()) > 0.1f)
                {
                    popUpIndex++;
                    Time.timeScale = 1f;
                }
                break;

            case 5:
                // Teach the player that the sail auto adjusts
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    popUpIndex++;
                }
                break;

            case 6:
                // Teach the player that the sail is more or less effective at certain angles
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Time.timeScale = 1f;
                    popUpIndex++;
                }
                break;

            case 7:
                // Teach the player to not crash into the island
                float yPositionToPass = 150f;
                tutorialTarget.target = new Vector2(0, yPositionToPass);
                if (player.transform.position.y > yPositionToPass)
                {
                    tutorialTarget.target = Vector2.zero; // Disable the arrow
                    popUpIndex++;
                }
                break;

            case 8:
                // Congratulate the player for passing the island
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    playerInputActions.FindAction("SailToggle").Enable();
                    popUpIndex++;
                }
                break;

            default:
                instructionsParent.SetActive(false);
                break;
        }
    }
}