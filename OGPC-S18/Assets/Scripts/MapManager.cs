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

    private Vector2 screenBounds;
    private Vector2 screenOrig;
    private InputActionMap boatActionMap;

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
        boatActionMap = inputs.FindActionMap("Player");
        boatActionMap.Enable();
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
        map.SetActive(true); // allows reference of its components
        mapZoomScale = 1f;
        IconManager = map.transform.GetChild(3);
        mapRect = map.GetComponent<RectTransform>();
        background.localScale = new Vector3(mapRect.rect.width, mapRect.rect.height, 1f);
        Mathf.Clamp(mapZoomScale, mapZoomLimits.x, mapZoomLimits.y);
        worldToMapScalar = DetermineMapScaleFactor();
        map.SetActive(false);
        mapOn = false;

        AddObjectsToMap();

        screenOrig = Camera.main.ScreenToWorldPoint(Vector2.zero);
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    }

    private void AddObjectsToMap()
    {
        islands = GameObject.FindGameObjectsWithTag("Island");
        ports = GameObject.FindGameObjectsWithTag("Port");

        iconScaleFactor = (islands[0].GetComponent<PolygonCollider2D>().bounds.size.x / worldSize.x) * map.GetComponent<RectTransform>().rect.width;
        playerIcon.GetComponent<RectTransform>().localScale = playerIcon.GetComponent<RectTransform>().localScale * iconScaleFactor;
        AddIslandsToMap();
        AddPortsToMap();
    }
    private void AddIslandsToMap()
    {
        islandIcons = new GameObject[islands.Length];
        Image iconImage;
        RectTransform islandRect;
        SpriteRenderer islandSprite;
        Vector3 islandSize;
        Quaternion islandRotation;


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
            islandSize = islands[i].GetComponent<PolygonCollider2D>().bounds.size;

            islandRect = islandIcons[i].GetComponent<RectTransform>();
            islandIcons[i].transform.localPosition = worldToMapScalar * islandCoords * mapZoomScale;
            islandIcons[i].transform.localRotation = islandRotation;
            islandRect.localScale = new Vector3(islandRect.localScale.x * iconScaleFactor,islandRect.localScale.y * iconScaleFactor, 1f);
        }
    }
    
    private void AddPortsToMap()
    {
        portIcons = new GameObject[ports.Length];
        Vector2 portCoords;
        Quaternion portRotation;
        RectTransform portRect;

        Image iconImage;
        SpriteRenderer portSprite;
        Vector3 portSize;

        for (int i = 0; i < ports.Length; i++)
        {
            portCoords = ports[i].transform.position;
            portRotation = ports[i].transform.rotation;
            portIcons[i] = Instantiate(portIconPrefab, IconManager);

            iconImage = portIcons[i].GetComponent<Image>();
            portSprite = ports[i].transform.GetChild(0).GetComponent<SpriteRenderer>();
            iconImage.sprite = portSprite.sprite;
            iconImage.color = portSprite.color;
            portSize = ports[i].transform.GetChild(2).GetComponent<PolygonCollider2D>().bounds.size;
            
            portRect = portIcons[i].GetComponent<RectTransform>();
            portIcons[i].transform.localPosition = worldToMapScalar * portCoords * mapZoomScale;
            portIcons[i].transform.localRotation = portRotation;
            portRect.localScale = new Vector3(portRect.localScale.x * iconScaleFactor,portRect.localScale.y * iconScaleFactor, 1f);
        }
    }

    private void ZoomMap()
    {
        if (mapZoomInput.ReadValue<float>() != 0)
        {
            panLocation = panLocation / mapZoomScale;
            mapZoomScale = mapZoomScale + mapZoomSensitivity * mapZoomInput.ReadValue<float>();
            mapZoomScale = Mathf.Clamp(mapZoomScale, mapZoomLimits.x, mapZoomLimits.y);
            IconManager.localScale = new Vector3(mapZoomScale,mapZoomScale,1f);
            panLocation = panLocation * mapZoomScale;

        }
    }

    private void PanMap()
    {
        if (mapPanInput.ReadValue<Vector2>() != Vector2.zero)
        {
            panLocation = panLocation + mapPanSensitivity * mapPanInput.ReadValue<Vector2>();
            IconManager.localPosition = -panLocation;
        }
    }

    private void ResetMap()
    {
        mapZoomScale = 1f;
        panLocation = new Vector2(0f,0f); //Recenters on player
    }
    private void UpdateMap()
    {
        if (mapReset.triggered)
        {
            ResetMap();
        }     
        ZoomMap();
        PanMap();
    }

    private float DetermineMapScaleFactor()
    {
        float xFactor;
        float yFactor;
        float realFactor;
        Vector2 mapSize = new Vector2(mapRect.rect.width,mapRect.rect.height);
        xFactor = mapSize.x/worldSize.x;
        yFactor = mapSize.y/worldSize.y;
        realFactor = Mathf.Min(xFactor,yFactor);

        return realFactor;
    }

    private void ToggleMap()
    {
        mapOn = !mapOn;
        playerIcon.transform.localPosition = player.transform.position * worldToMapScalar;
        playerIcon.transform.localRotation = player.transform.rotation;
        panLocation = playerIcon.transform.localPosition;
        mapZoomScale = 1f;
        map.SetActive(mapOn);
        UsefulStuff.GamePaused(mapOn);
    }
    private void Update()
    {
        if (mapToggle.triggered)
        {
            ToggleMap();            
            if (mapOn)
            {
                boatActionMap.Disable();
            }
            else
            {
                boatActionMap.Enable();
            }
        }
        if (mapOn) 
        {
            UpdateMap();
            
        }
    }
}