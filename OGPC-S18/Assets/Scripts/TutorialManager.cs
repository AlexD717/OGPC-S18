using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject instructionsParent;
    private GameObject[] popUps;
    private int popUpIndex = 0;
    private float waitTime;
    private int onDeathPopUpIndex = 0;
    private Vector2 lastSavePos;
    private Vector2 hurricanePos;
    [SerializeField] private InputActionAsset inputActions;
    private InputActionMap playerInputActions;
    private InputActionMap mapInputActions;
    [SerializeField] private TutorialTarget tutorialTarget;
    [SerializeField] private GameObject firstPort;
    [SerializeField] private GameObject hurricane;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameObject endPortArrow;

    private GameObject player;

    private bool firstFrame = true;
    private bool playerDockedToEndPort = false;

    private void Start()
    {
        playerInputActions = inputActions.FindActionMap("Player");
        mapInputActions = inputActions.FindActionMap("Map");
        playerInputActions.Enable();
        mapInputActions.Enable();
        endPortArrow.SetActive(false);
        hurricane.SetActive(false);
        levelManager.gameObject.SetActive(false);

        popUps = new GameObject[instructionsParent.transform.childCount];
        for (int i = 0; i < instructionsParent.transform.childCount; i++)
        {
            popUps[i] = instructionsParent.transform.GetChild(i).gameObject;
        }

        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<BoatHealth>().tutorialLevel = true;
        levelManager.tutorialLevel = true;

        hurricanePos = hurricane.transform.position;
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
                // Introduction 1
                Time.timeScale = 0f;
                mapInputActions.FindAction("MapToggle").Disable();
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    popUpIndex++;
                }
                break;

            case 1:
                // Introduction 2
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    popUpIndex++;
                }
                break;

            case 2:
                // Introduction 3
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    mapInputActions.FindAction("MapToggle").Enable();
                    popUpIndex++;
                }
                break;

            case 3:
                Time.timeScale = 0f;
                // Teach the player that they can open the map
                if (mapInputActions.FindAction("MapToggle").triggered)
                {
                    mapInputActions.FindAction("MapToggle").Disable();
                    popUpIndex++;
                }
                break;

            case 4:
                // Teach the player that they can pan around the map
                endPortArrow.SetActive(false);
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    popUpIndex++;
                }
                break;

            case 5:
                // Teach the player that they can zoom in and out
                if (Mathf.Abs(mapInputActions.FindAction("MapZoom").ReadValue<float>()) > 0.1f)
                {
                    mapInputActions.FindAction("MapToggle").Enable();
                    popUpIndex++;
                }
                break;

            case 6:
                // Teach the player to disable the map
                if (mapInputActions.FindAction("MapToggle").triggered)
                {
                    mapInputActions.FindAction("MapToggle").Disable();
                    playerInputActions.FindAction("Rotation").Enable();
                    popUpIndex++;
                }
                break;

            case 7:
                // Teach the player that they can rotate the boat
                playerInputActions.FindAction("SailToggle").Disable(); // Gets enabled after map close
                Time.timeScale = 0f;
                if (Mathf.Abs(playerInputActions.FindAction("Rotation").ReadValue<float>()) > 0.1f)
                {
                    waitTime = 5f;
                    popUpIndex++;
                }
                
                break;

            case 8:
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

            case 9:
                // Teach the player that the sail auto adjusts
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    popUpIndex++;
                }
                break;

            case 10:
                // Teach the player that the sail is more or less effective at certain angles
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    popUpIndex++;
                }
                break;

            case 11:
                // Tell the player why they shouldn't crash into the island
                tutorialTarget.target = new Vector2(0, 160f);
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Time.timeScale = 1f;
                    popUpIndex++;
                }
                break;

            case 12:
                // Have the player try and sail around the island
                instructionsParent.GetComponent<Image>().enabled = false;
                onDeathPopUpIndex = popUpIndex + 1; // Set the index to the next pop-up
                float yPositionToPass = 112f;
                if (player.transform.position.y > yPositionToPass)
                {
                    instructionsParent.GetComponent<Image>().enabled = true;
                    tutorialTarget.target = Vector2.zero; // Disable the arrow
                    popUpIndex += 2;
                }
                break;

            case 13:
                // Player crashed into the island
                instructionsParent.GetComponent<Image>().enabled = true;
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Time.timeScale = 1f;
                    player.transform.position = new Vector3(0, 50, 0);
                    popUpIndex--;
                }
                break;

            case 14:
                // Congratulate the player for passing the island
                Time.timeScale = 0f;
                onDeathPopUpIndex = 0; // Reset the onDeathPopUpIndex
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Time.timeScale = 1f;
                    popUpIndex++;
                }
                break;

            case 15:
                // Explain why the player might want to toggle the sail
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Time.timeScale = 1f;
                    playerInputActions.FindAction("SailToggle").Enable();
                    popUpIndex++;
                }
                break;

            case 16:
                // Teach the player that they can toggle the sail
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    playerInputActions.FindAction("SailToggle").Disable();
                    waitTime = 3f;
                    popUpIndex++;
                }
                break;

            case 17:
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

            case 18:
                // Tell the player how to turn off the sail
                Time.timeScale = 0f;
                if (playerInputActions.FindAction("SailToggle").triggered)
                {
                    popUpIndex++;
                }
                break;

            case 19:
                // Explain the purpose of ports
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    if (player.transform.position.x > 0)
                    {
                        lastSavePos = new Vector2(56.2f, 117.2f);
                    }
                    else
                    {
                        lastSavePos = new Vector2(-38.7f, 135.9f);
                    }
                    popUpIndex++;
                }
                break;

            case 20:
                // tell the player to go to the port
                tutorialTarget.target = firstPort.transform.position;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    playerInputActions.FindAction("Interact").Disable();
                    popUpIndex++;
                }
                break;

            case 21:
                // tell the player to go to the port
                Time.timeScale = 1f;
                onDeathPopUpIndex = popUpIndex + 1; // Set the index to the next pop-up
                instructionsParent.GetComponent<Image>().enabled = false;
                if (Vector2.Distance(player.transform.position, firstPort.transform.position) < 15f)
                {
                    instructionsParent.GetComponent<Image>().enabled = true;
                    tutorialTarget.target = Vector2.zero; // Disable the arrow
                    playerInputActions.FindAction("Interact").Enable();
                    onDeathPopUpIndex = 0; // Reset the onDeathPopUpIndex
                    popUpIndex += 2;
                }
                break;

            case 22:
                // Player crashed on the way to the port
                Time.timeScale = 0f;
                instructionsParent.GetComponent<Image>().enabled = true;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Time.timeScale = 1f;
                    player.transform.position = lastSavePos;
                    popUpIndex--;
                }
                break;

            case 23:
                // Teach the player how to dock to the port
                Time.timeScale = 0f;
                if (playerInputActions.FindAction("Interact").triggered)
                {
                    playerInputActions.FindAction("Interact").Disable();
                    popUpIndex++;
                }
                break;

            case 24:
                // Tell the player that it takes some time for the port to be saved
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    waitTime = 2.6f;
                    popUpIndex++;
                }
                break;

            case 25:
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

            case 26:
                // Tell the player how to undock from the port
                if (playerInputActions.FindAction("Interact").triggered)
                {
                    waitTime = 1.5f;
                    popUpIndex++;
                }
                break;

            case 27:
                // Let the player leave the port
                mapInputActions.FindAction("MapToggle").Disable();
                if (waitTime > 0f)
                {
                    waitTime -= Time.deltaTime;
                }
                else
                {
                    popUpIndex++;
                    Time.timeScale = 0f;
                }
                break;

            case 28:
                // Storm approaches
                mapInputActions.FindAction("MapToggle").Disable();
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    hurricane.SetActive(true);
                    levelManager.gameObject.SetActive(true);
                    Time.timeScale = 1f;
                    Invoke("NextPopUpIndex", 1f);
                }
                break;

            case 29:
                // Open map to see whats happening
                Time.timeScale = 0f;
                mapInputActions.FindAction("MapToggle").Enable();
                if (mapInputActions.FindAction("MapToggle").triggered)
                {
                    mapInputActions.FindAction("MapToggle").Disable();
                    popUpIndex++;
                }
                break;

            case 30:
                // Storm dramatic text
                endPortArrow.SetActive(false);
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    popUpIndex++;
                }
                break;

            case 31:
                // Plan where to go
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    popUpIndex++;
                }
                break;

            case 32:
                // Dont see port
                endPortArrow.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    mapInputActions.FindAction("MapToggle").Enable();
                    popUpIndex++;
                }
                break;

            case 33:
                // close map and resume
                if (mapInputActions.FindAction("MapToggle").triggered)
                {
                    lastSavePos = new Vector2(20.9f, 146.4f);
                    popUpIndex++;
                }
                break;

            case 34:
                // sail to the port
                instructionsParent.GetComponent<Image>().enabled = false;
                onDeathPopUpIndex = popUpIndex + 1; // Set the index to the next pop-up
                if (playerDockedToEndPort)
                {
                    instructionsParent.GetComponent<Image>().enabled = true;
                    tutorialTarget.target = Vector2.zero; // Disable the arrow
                    onDeathPopUpIndex = 0; // Reset the onDeathPopUpIndex
                    popUpIndex += 2;
                }
                break;

            case 35:
                // Player died on the way to the port
                Time.timeScale = 0f;
                instructionsParent.GetComponent<Image>().enabled = true;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Time.timeScale = 1f;
                    hurricane.transform.position = hurricanePos;
                    hurricane.GetComponent<Hurricane>().ResetWaypoints();
                    levelManager.SetCountdown(45f);
                    player.transform.position = lastSavePos;
                    popUpIndex--;
                }
                break;

            case 36:
                // Player got to end port
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    popUpIndex++;
                }
                break;

            case 37:
                // Player told they are on their own
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Debug.Log("Loading Main Menu");
                    Loader.LoadByName("MainMenu");
                }
                break;

            default:
                instructionsParent.SetActive(false);
                break;
        }
    }

    private void NextPopUpIndex()
    {
        popUpIndex++;
    }

    public void PlayerDied()
    {
        Debug.Log($"Player died. On death popUpIndex is {onDeathPopUpIndex}");
        if (onDeathPopUpIndex != 0)
        {
            popUpIndex = onDeathPopUpIndex;
            player.GetComponent<BoatHealth>().ResetHealth();
            player.GetComponent<Rigidbody2D>().angularVelocity = 0f;
            player.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
        }
    }

    public void PlayerReachedEndPort()
    {
        playerDockedToEndPort = true;
    }
}