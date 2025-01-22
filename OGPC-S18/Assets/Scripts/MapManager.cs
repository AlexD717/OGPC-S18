using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class MapManager : MonoBehaviour
{
    [SerializeField] private Vector2 worldSize;
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject islandIconReference;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerIcon;
    [SerializeField] private InputActionAsset inputs;
    private InputAction mapToggle;
    private bool mapOn;
    GameObject[] islands;
    GameObject[] islandIcons;
    float worldtoMapScalar;

    void OnEnable()
    {
        mapOn = false;
        mapToggle = inputs.FindActionMap("Player").FindAction("MapToggle");
        mapToggle.Enable();
        map.SetActive(false);
    }

    void OnDisable()
    {
        mapToggle.Disable();
    }
    void Start()
    {
        AddIslandsToMap();
    }

    void UpdatePlayerOnMap()
    {
        playerIcon.transform.localPosition = worldtoMapScalar * player.transform.position;
        playerIcon.transform.localRotation = player.transform.rotation;
    }
    void AddIslandsToMap()
    {
        islands = GameObject.FindGameObjectsWithTag("Island");
        islandIcons = new GameObject[islands.Length];
        worldtoMapScalar = DetermineMapScaleFactor();
        Image iconImage;
        SpriteRenderer islandSprite;

        Vector2 islandCoords;
        for (int i = 0; i < islands.Length; i++)
        {
            islandCoords = islands[i].transform.position;
            islandIcons[i] = Instantiate(islandIconReference, map.transform);
            islandIcons[i].transform.localPosition = worldtoMapScalar * islandCoords;
            Debug.Log(worldtoMapScalar.ToString());
            iconImage = islandIcons[i].GetComponent<Image>();
            islandSprite = islands[i].GetComponent<SpriteRenderer>();
            iconImage.sprite = islandSprite.sprite;
            iconImage.color = islandSprite.color;
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
        Debug.Log(worldSize.x.ToString() + " " + worldSize.x.ToString());
        Debug.Log(mapSize.x.ToString() + " " + mapSize.x.ToString());
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
