using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using System.Collections;

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

    [SerializeField] private float panSensitivity;
    [SerializeField] private float zoomSensitivity;
    [SerializeField] private Vector2 zoomLimits;
    private Vector2 originalPosition;
    private float originalZoom;

    [SerializeField] private bool cameraZoomsOutOnPlayerPosition;
    private GameObject player;

    private void Start()
    {
        mapActive = false;

        originalPosition = mapCamera.transform.position;
        originalZoom = mapCamera.Lens.OrthographicSize;

        player = GameObject.FindGameObjectWithTag("Player");

        ResetCamera();
    }

    private void OnEnable()
    {
        playerActionMap = inputActions.FindActionMap("Player");

        var mapControls = inputActions.FindActionMap("Map");
        mapToggle = mapControls.FindAction("MapToggle");
        mapReset = mapControls.FindAction("MapReset");
        mapPan = mapControls.FindAction("MapPan");
        mapZoom = mapControls.FindAction("MapZoom");

        mapToggle.Enable();
        mapReset.Enable();
        mapPan.Enable();
        mapZoom.Enable();
    }

    private void OnDisable()
    {
        mapToggle.Disable();
        mapReset.Disable();
        mapPan.Disable();
        mapZoom.Disable();
    }

    private void Update()
    {
        if (mapToggle.triggered)
        {
            mapActive = !mapActive;
            SwitchCameras(mapActive);

            if (mapActive) 
            { 
                playerActionMap.Disable();
                Time.timeScale = 0f; 
            }
            else 
            {
                playerActionMap.Enable();
                Time.timeScale = 1f; 
            }
        }

        if (!mapActive) { return; }

        PanCamera();
        ZoomCamera();
        if (mapReset.triggered)
        {
            ResetCamera();
        }
    }

    private void ResetCamera()
    {
        if (cameraZoomsOutOnPlayerPosition)
        {
            mapCamera.transform.position = new Vector2(player.transform.position.x, player.transform.position.y);
        }
        else
        {
            mapCamera.transform.position = originalPosition;
        }
        mapCamera.Lens.OrthographicSize = originalZoom;
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
            ResetCamera();
            playerCamera.gameObject.SetActive(false);
            mapCamera.gameObject.SetActive(true);
        }
        else
        {
            playerCamera.gameObject.SetActive(true);
            mapCamera.gameObject.SetActive(false);
        }
    }
}
