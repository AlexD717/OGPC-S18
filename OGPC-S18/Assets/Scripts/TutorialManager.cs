using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject instructionsParent;
    private GameObject[] popUps;
    private int popUpIndex = 0;
    private float waitTime;
    private int onDeathPopUpIndex = 0;
    private Vector2 lastSavePos;
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
                    mapInputActions.FindAction("MapToggle").Disable();
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
                    mapInputActions.FindAction("MapToggle").Enable();
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
                onDeathPopUpIndex = popUpIndex + 1; // Set the index to the next pop-up
                float yPositionToPass = 112f;
                tutorialTarget.target = new Vector2(0, 160f);
                if (player.transform.position.y > yPositionToPass)
                {
                    tutorialTarget.target = Vector2.zero; // Disable the arrow
                    popUpIndex += 2;
                }
                break;

            case 9:
                // Player crashed into the island
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Time.timeScale = 1f;
                    player.transform.position = new Vector3(0, 50, 0);
                    popUpIndex--;
                }
                break;

            case 10:
                // Congratulate the player for passing the island
                Time.timeScale = 0f;
                onDeathPopUpIndex = 0; // Reset the onDeathPopUpIndex
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Time.timeScale = 1f;
                    popUpIndex++;
                }
                break;

            case 11:
                // Explain why the player might want to toggle the sail
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Time.timeScale = 1f;
                    playerInputActions.FindAction("SailToggle").Enable();
                    popUpIndex++;
                }
                break;

            case 12:
                // Teach the player that they can toggle the sail
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    playerInputActions.FindAction("SailToggle").Disable();
                    waitTime = 5f;
                    popUpIndex++;
                }
                break;

            case 13:
                // Let the player experiment without the sail
                Time.timeScale = 1f;
                if (waitTime > 0f)
                {
                    waitTime -= Time.deltaTime;
                }
                else
                {
                    playerInputActions.FindAction("SailToggle").Enable();
                    popUpIndex++;
                }
                break;

            case 14:
                // Tell the player how to turn off the sail
                Time.timeScale = 0f;
                if (playerInputActions.FindAction("SailToggle").triggered)
                {
                    popUpIndex++;
                }
                break;

            case 15:
                // Explain the purpose of ports
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    playerInputActions.FindAction("Interact").Disable();
                    lastSavePos = player.transform.position;
                    popUpIndex++;
                }
                break;

            case 16:
                // tell the player to go to the port
                Time.timeScale = 1f;
                tutorialTarget.target = firstPort.transform.position;
                onDeathPopUpIndex = popUpIndex + 1; // Set the index to the next pop-up
                if (Vector2.Distance(player.transform.position, firstPort.transform.position) < 15f)
                {
                    tutorialTarget.target = Vector2.zero; // Disable the arrow
                    playerInputActions.FindAction("Interact").Enable();
                    onDeathPopUpIndex = 0; // Reset the onDeathPopUpIndex
                    popUpIndex += 2;
                }
                break;

            case 17:
                // Player crashed on the way to the port
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Time.timeScale = 1f;
                    player.transform.position = lastSavePos;
                    popUpIndex--;
                }
                break;

            case 18:
                // Teach the player how to dock to the port
                Time.timeScale = 0f;
                if (playerInputActions.FindAction("Interact").triggered)
                {
                    playerInputActions.FindAction("Interact").Disable();
                    popUpIndex++;
                }
                break;

            case 19:
                // Tell the player that it takes some time for the port to be saved
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    waitTime = 2.6f;
                    popUpIndex++;
                }
                break;

            case 20:
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

            case 21:
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

    public void PlayerDied()
    {
        Debug.Log($"Player died. On death popUpIndex is {onDeathPopUpIndex}");
        if (onDeathPopUpIndex != 0)
        {
            popUpIndex = onDeathPopUpIndex;
            player.GetComponent<Rigidbody2D>().angularVelocity = 0f;
            player.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
        }
    }
}