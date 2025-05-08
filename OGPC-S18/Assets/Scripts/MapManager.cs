using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class MapManager : MonoBehaviour
{
    private bool mapActive;

    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private CinemachineCamera mapCamera;

    [SerializeField] private InputActionAsset inputActions;
    private InputActionMap playerActionMap;
    private InputAction mapToggle;
    private InputAction mapReset;
    private InputAction mapPan;
    private InputAction mapZoom;
    private InputAction resetLevel;

    [SerializeField] private float panSensitivity;
    [SerializeField] private float zoomSensitivity;
    [SerializeField] private Vector2 zoomLimits;
    [SerializeField] private float zoomAmount;
    [SerializeField] private bool keepOriginalZoomAmount = true;
    private Vector2 originalPosition;

    [SerializeField] private bool cameraZoomsOutOnPlayerPosition;
    [SerializeField] private bool startInMapView = true;
    private GameObject player;

    private GameObject[] showInMapOnly;
    private GameObject[] hideInMap;

    private GameObject canvas;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        canvas = transform.GetChild(0).gameObject;

        originalPosition = mapCamera.transform.position;
        if (keepOriginalZoomAmount)
        {
            zoomAmount = mapCamera.Lens.OrthographicSize;
        }

        showInMapOnly = GameObject.FindGameObjectsWithTag("ShowInMapOnly");
        hideInMap = GameObject.FindGameObjectsWithTag("HideInMap");
        if (startInMapView)
        {
            mapActive = true;
            canvas.SetActive(true);
            playerActionMap.Disable();
            Time.timeScale = 0f;
            ToggleVisibilityBasedOnMap(true);
        }
        else
        {
            mapActive = false;
            canvas.SetActive(false);
            ToggleVisibilityBasedOnMap(false);
        }
        SwitchCameras(mapActive);
    }

    private void OnEnable()
    {
        playerActionMap = inputActions.FindActionMap("Player");

        InputActionMap mapControls = inputActions.FindActionMap("Map");
        mapToggle = mapControls.FindAction("MapToggle");
        mapReset = mapControls.FindAction("MapReset");
        mapPan = mapControls.FindAction("MapPan");
        mapZoom = mapControls.FindAction("MapZoom");
        resetLevel = mapControls.FindAction("ResetLevel");

        mapToggle.Enable();
        mapReset.Enable();
        mapPan.Enable();
        mapZoom.Enable();
        resetLevel.Enable();
    }

    private void OnDisable()
    {
        mapToggle.Disable();
        mapReset.Disable();
        mapPan.Disable();
        mapZoom.Disable();
        resetLevel.Disable();
    }

    private void Update()
    {
        if (mapToggle.triggered)
        {
            canvas.SetActive(false);

            mapActive = !mapActive;
            SwitchCameras(mapActive);
            ResetCamera();

            if (mapActive)
            {
                playerActionMap.Disable();
                Time.timeScale = 0f;
                ToggleVisibilityBasedOnMap(true);
            }
            else
            {
                playerActionMap.Enable();
                Time.timeScale = 1f;
                ToggleVisibilityBasedOnMap(false);
            }
        }

        if (!mapActive) { return; }

        PanCamera();
        ZoomCamera();
        if (mapReset.triggered)
        {
            ResetCamera();
        }

        if (resetLevel.triggered)
        {
            Loader.RestartLevel();
        }
    }

    private void ResetCamera()
    {
        if (startInMapView)
        {
            startInMapView = false;
            return;
        }

        if (cameraZoomsOutOnPlayerPosition)
        {
            mapCamera.transform.position = new Vector2(player.transform.position.x, player.transform.position.y);
        }
        else
        {
            mapCamera.transform.position = originalPosition;
        }
        mapCamera.Lens.OrthographicSize = zoomAmount;
    }

    private void PanCamera()
    {
        Vector2 panValue = mapPan.ReadValue<Vector2>();
        Vector3 moveAmount = Vector2.zero;

        moveAmount.x = panValue.x * panSensitivity * Time.unscaledDeltaTime * 10f;
        moveAmount.y = panValue.y * panSensitivity * Time.unscaledDeltaTime * 10f;

        mapCamera.transform.position += moveAmount;
    }

    private void ZoomCamera()
    {
        float zoomValue = mapZoom.ReadValue<float>();

        mapCamera.Lens.OrthographicSize = Mathf.Clamp(mapCamera.Lens.OrthographicSize + (zoomValue * Time.unscaledDeltaTime * zoomSensitivity), zoomLimits.x, zoomLimits.y);
    }

    private void SwitchCameras(bool mapActive)
    {
        if (mapActive)
        {
            playerCamera.gameObject.SetActive(false);
            mapCamera.gameObject.SetActive(true);
        }
        else
        {
            playerCamera.gameObject.SetActive(true);
            mapCamera.gameObject.SetActive(false);
        }
    }

    private void ToggleVisibilityBasedOnMap(bool mapOn)
    {
        if (mapOn)
        {
            foreach (GameObject obj in showInMapOnly)
            {
                obj.SetActive(true);
            }
            foreach (GameObject obj in hideInMap)
            {
                obj.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject obj in showInMapOnly)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in hideInMap)
            {
                obj.SetActive(true);
            }
        }
    }
}