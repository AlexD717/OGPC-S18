using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject instructionsParent;
    private GameObject[] popUps;
    private int popUpIndex = 0;
    private float waitTime;
    [SerializeField] private InputActionAsset inputActions;
    private InputActionMap playerInputActions;
    private InputActionMap mapInputActions;
    [SerializeField] private TutorialTarget tutorialTarget;
    [SerializeField] private GameObject firstPort;

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
                    waitTime = 5f;
                    popUpIndex++;
                }
                
                break;

            case 5:
                // waits for the player to experiment with rotating the boat
                Time.timeScale = 1f;
                if (waitTime > 0f)
                {
                    waitTime -= Time.deltaTime;
                }
                else
                {
                    popUpIndex++;
                }
                break;

            case 6:
                // Teach the player that the sail auto adjusts
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    popUpIndex++;
                }
                break;

            case 7:
                // Teach the player that the sail is more or less effective at certain angles
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Time.timeScale = 1f;
                    popUpIndex++;
                }
                break;

            case 8:
                // Teach the player to not crash into the island
                float yPositionToPass = 150f;
                tutorialTarget.target = new Vector2(0, yPositionToPass + 5f);
                if (player.transform.position.y > yPositionToPass)
                {
                    tutorialTarget.target = Vector2.zero; // Disable the arrow
                    popUpIndex++;
                }
                break;

            case 9:
                // Congratulate the player for passing the island
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Time.timeScale = 1f;
                    popUpIndex++;
                }
                break;

            case 10:
                // Explain why the player might want to toggle the sail
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Time.timeScale = 1f;
                    playerInputActions.FindAction("SailToggle").Enable();
                    popUpIndex++;
                }
                break;

            case 11:
                // Teach the player that they can toggle the sail
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    waitTime = 5f;
                    popUpIndex++;
                }
                break;

            case 12:
                // Let the player experiment without the sail
                Time.timeScale = 1f;
                if (waitTime > 0f)
                {
                    waitTime -= Time.deltaTime;
                }
                else
                {
                    popUpIndex++;
                }
                break;

            case 13:
                // Explain the purpose of ports
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    popUpIndex++;
                }
                break;

            case 14:
                // tell the player to go to the port
                Time.timeScale = 1f;
                tutorialTarget.target = firstPort.transform.position;
                if (Vector2.Distance(player.transform.position, firstPort.transform.position) < 15f)
                {
                    tutorialTarget.target = Vector2.zero; // Disable the arrow
                    popUpIndex++;
                }
                break;

            case 15:
                // Teach the player how to dock to the port
                Time.timeScale = 0f;
                if (playerInputActions.FindAction("Interact").triggered)
                {
                    popUpIndex++;
                }
                break;

            case 16:
                // Tell the player that it takes some time for the port to be saved
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    waitTime = 2.6f;
                    popUpIndex++;
                }
                break;

            case 17:
                // Wait for the port to be saved
                Time.timeScale = 1f;
                playerInputActions.Disable();
                mapInputActions.Disable();
                if (waitTime > 0f)
                {
                    waitTime -= Time.deltaTime;
                }
                else
                {
                    playerInputActions.Enable();
                    mapInputActions.Enable();
                    popUpIndex++;
                }
                break;

            case 18:
                // Tell the player how to undock from the port
                if (playerInputActions.FindAction("Interact").triggered)
                {
                    popUpIndex++;
                }
                break;

            default:
                instructionsParent.SetActive(false);
                break;
        }
    }
}