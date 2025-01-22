using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class MapManager : MonoBehaviour
{
    [SerializeField] private Vector2 worldSize;
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject islandIconReference;
    [SerializeField] private GameObject portIconReference;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerIcon;
    [SerializeField] private InputActionAsset inputs;
    private InputAction mapToggle;
    private bool mapOn;
    GameObject[] islands;
    GameObject[] ports;

    GameObject[] islandIcons;
    GameObject[] portIcons;
    float worldToMapScalar;

    void OnEnable()
    { 
        map.SetActive(true); // allows reference of its components
        worldToMapScalar = DetermineMapScaleFactor();
        map.SetActive(false);
        mapOn = false;
        mapToggle = inputs.FindActionMap("Player").FindAction("MapToggle");
        mapToggle.Enable();
    }

    void OnDisable()
    {
        mapToggle.Disable();
    }
    void Start()
    {
        AddIslandsToMap();
        AddPortsToMap();
    }

    void UpdatePlayerOnMap()
    {
        playerIcon.transform.localPosition = worldToMapScalar * player.transform.position;
        playerIcon.transform.localRotation = player.transform.rotation;
    }
    void AddIslandsToMap()
    {
        islands = GameObject.FindGameObjectsWithTag("Island");
        islandIcons = new GameObject[islands.Length];
        Image iconImage;
        SpriteRenderer islandSprite;

        Vector2 islandCoords;
        for (int i = 0; i < islands.Length; i++)
        {
            islandCoords = islands[i].transform.position;
            islandIcons[i] = Instantiate(islandIconReference, map.transform);
            islandIcons[i].transform.localPosition = worldToMapScalar * islandCoords;
            Debug.Log(worldToMapScalar.ToString());
            iconImage = islandIcons[i].GetComponent<Image>();
            islandSprite = islands[i].GetComponent<SpriteRenderer>();
            iconImage.sprite = islandSprite.sprite;
            iconImage.color = islandSprite.color;
        }
    }
    void AddPortsToMap()
    {
        ports = GameObject.FindGameObjectsWithTag("Port");
        portIcons = new GameObject[ports.Length];
        Image iconImage;
        Vector2 portCoords;
        Quaternion portRotation;
        
        for (int i = 0; i < ports.Length; i++)
        {
            portCoords = ports[i].transform.position;
            portRotation = ports[i].transform.localRotation;
            portIcons[i] = Instantiate(portIconReference, map.transform);
            portIcons[i].transform.localPosition = worldToMapScalar * portCoords;
            portIcons[i].transform.localRotation = portRotation;
            Debug.Log(worldToMapScalar.ToString());
            iconImage = portIcons[i].GetComponent<Image>();
        }
    }

    float DetermineMapScaleFactor()
    {
        float xFactor;
        float yFactor;
        float realFactor;
        RectTransform mapRect = map.GetComponent<RectTransform>();

        Vector2 mapSize = new Vector2(mapRect.rect.width,mapRect.rect.height);
        xFactor = worldSize.x/mapSize.x;
        yFactor = worldSize.y/mapSize.y;
        realFactor = 1 / Mathf.Max(xFactor,yFactor);

        return realFactor;
    }
    void Update()
    {
        if (mapToggle.triggered)
        {
            mapOn = !mapOn;
            map.SetActive(mapOn);
        }
        if (mapOn)
        {
            UpdatePlayerOnMap();
        }
    }
}
