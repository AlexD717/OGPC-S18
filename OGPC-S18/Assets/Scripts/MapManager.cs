using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class MapManager : MonoBehaviour
{

    [Header("Configurables")]
    [SerializeField] private Vector2 worldSize;
    [SerializeField] private Vector2 mapZoomLimits;
    [SerializeField] private float mapZoomSensitivity;
    [SerializeField] private float mapPanSensitivity;
    [Header("References")]
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject islandIconReference;
    [SerializeField] private GameObject portIconPrefab;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerIcon;
    [SerializeField] private InputActionAsset inputs;
    [SerializeField] private RectTransform background;

    private InputAction mapZoomInput;
    private float mapZoomScale;
    private InputAction mapPanInput;
    private InputAction mapReset; // Recenters around Player and rescales
    private Vector2 panLocation;
    private InputAction mapToggle;
    private bool mapOn;

    private RectTransform mapRect;
    GameObject[] islands;
    GameObject[] ports;

    GameObject[] islandIcons;
    Vector3[] islandIconsBaseRef; //For storing default location and scale
    GameObject[] portIcons;
    Vector3[] portIconsBaseRef; //For storing default location and scale

    float iconScaleFactor;
    Transform IconManager;
    float worldToMapScalar;

    private void OnEnable()
    { 
        mapToggle = inputs.FindActionMap("Map").FindAction("MapToggle");
        mapToggle.Enable();
        mapZoomInput = inputs.FindActionMap("Map").FindAction("MapZoom");
        mapZoomInput.Enable();
        mapPanInput = inputs.FindActionMap("Map").FindAction("MapPan");
        mapPanInput.Enable();
        mapReset = inputs.FindActionMap("Map").FindAction("mapReset");
        mapReset.Enable();
    }

    private void OnDisable()
    {
        mapToggle.Disable();
        mapZoomInput.Disable();
        mapPanInput.Disable();
        mapReset.Disable();
    }
    private void Start()
    {
        mapZoomScale = 1f;
        IconManager = map.transform.GetChild(4);
        mapRect = map.GetComponent<RectTransform>();
        background.localScale = new Vector3(mapRect.rect.width, mapRect.rect.height, 1f);
        Mathf.Clamp(mapZoomScale, mapZoomLimits.x, mapZoomLimits.y);
        map.SetActive(true); // allows reference of its components
        worldToMapScalar = DetermineMapScaleFactor();
        map.SetActive(false);
        mapOn = false;
        AddIslandsToMap();
        AddPortsToMap();

    }

    private void UpdatePlayerOnMap()
    {
        panLocation = worldToMapScalar * player.transform.position;
        playerIcon.transform.localRotation = player.transform.rotation;
    }
    private void AddIslandsToMap()
    {
        islands = GameObject.FindGameObjectsWithTag("Island");
        islandIcons = new GameObject[islands.Length];
        Image iconImage;
        RectTransform islandRect;
        SpriteRenderer islandSprite;
        Vector3 islandSize;
        Quaternion islandRotation;
        float hexagonYSquishFactor = 34.6875f / 40f;

        islandSize = islands[0].GetComponent<PolygonCollider2D>().bounds.size;
        iconScaleFactor = (islandSize.x / worldSize.x) * map.GetComponent<RectTransform>().rect.width / 100;

        Vector2 islandCoords;
        for (int i = 0; i < islands.Length; i++)
        {
            islandCoords = islands[i].transform.position;
            islandRotation = islands[i].transform.rotation;
            islandIcons[i] = Instantiate(islandIconReference, IconManager);

            iconImage = islandIcons[i].GetComponent<Image>();
            islandSprite = islands[i].GetComponent<SpriteRenderer>();
            iconImage.sprite = islandSprite.sprite;
            iconImage.color = islandSprite.color;
            islandRect = islandIcons[i].GetComponent<RectTransform>();
            islandSize = islands[i].GetComponent<PolygonCollider2D>().bounds.size;

            islandIcons[i].transform.localPosition = worldToMapScalar * islandCoords * mapZoomScale;
            islandIcons[i].transform.localRotation = islandRotation;
            islandRect.localScale = new Vector3(islandRect.localScale.x * iconScaleFactor,islandRect.localScale.y * iconScaleFactor * hexagonYSquishFactor, 1f);
        }
    }
    private void AddPortsToMap()
    {
        ports = GameObject.FindGameObjectsWithTag("Port");
        portIcons = new GameObject[ports.Length];
        Vector2 portCoords;
        Quaternion portRotation;
        RectTransform portRect;

        for (int i = 0; i < ports.Length; i++)
        {
            portCoords = ports[i].transform.position;
            portRotation = ports[i].transform.rotation;
            portIcons[i] = Instantiate(portIconPrefab, IconManager);
            portIcons[i].transform.localPosition = worldToMapScalar * portCoords * mapZoomScale;
            portIcons[i].transform.localRotation = portRotation;
            
            portRect = portIcons[i].GetComponent<RectTransform>();
            portRect.localScale = 1.4f * new Vector3(portRect.localScale.x * iconScaleFactor,portRect.localScale.y * iconScaleFactor, 1f);
        }
    }

    private void ProcessInputs()
    {
        if (mapZoomInput.ReadValue<float>() != 0)
        {
            mapZoomScale = mapZoomScale + mapZoomSensitivity * mapZoomInput.ReadValue<float>();
            mapZoomScale = Mathf.Clamp(mapZoomScale, mapZoomLimits.x, mapZoomLimits.y);
        }
        panLocation = panLocation + mapPanSensitivity * mapPanInput.ReadValue<Vector2>();
        Debug.Log("PanLocation " + panLocation.x.ToString() + " " + panLocation.y.ToString());
        Debug.Log("PanLocationDelta " + mapPanInput.ReadValue<Vector2>().x.ToString() + " " + mapPanInput.ReadValue<Vector2>().y.ToString());

    }
    private void UpdateMap()
    {
        ProcessInputs();
        UpdatePlayerOnMap();
        IconManager.localScale = new Vector3(mapZoomScale,mapZoomScale,1f);
        IconManager.localPosition = -panLocation;
        if (mapReset.triggered)
        {
            mapZoomScale = 1f;
            panLocation = new Vector2(0f,0f); //Recenters on player
        }     
    }

    private float DetermineMapScaleFactor()
    {
        float xFactor;
        float yFactor;
        float realFactor;
        Vector2 mapSize = new Vector2(mapRect.rect.width,mapRect.rect.height);
        xFactor = worldSize.x/mapSize.x;
        yFactor = worldSize.y/mapSize.y;
        realFactor = 1 / Mathf.Max(xFactor,yFactor);

        return realFactor;
    }
    private void Update()
    {

        if (mapToggle.triggered)
        {
            mapOn = !mapOn;
            panLocation = playerIcon.transform.localPosition;
            map.SetActive(mapOn);
            UsefulStuff.GamePaused(mapOn);
        }
        if (mapOn) {UpdateMap();}
    }
}