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

    private void OnEnable()
    {
        playerInputActions = inputActions.FindActionMap("Player");
        mapInputActions = inputActions.FindActionMap("Map");
        playerInputActions.Enable();
        mapInputActions.Enable();
    }

    private void Start()
    {
        popUps = new GameObject[instructionsParent.transform.childCount];
        for (int i = 0; i < instructionsParent.transform.childCount; i++)
        {
            popUps[i] = instructionsParent.transform.GetChild(i).gameObject;
        }
    }

    void Update()
    {
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
                if (Input.GetKeyDown(KeyCode.A))
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
                    popUpIndex++;
                }
                break;

            default:
                instructionsParent.SetActive(false);
                break;
        }
    }
}
