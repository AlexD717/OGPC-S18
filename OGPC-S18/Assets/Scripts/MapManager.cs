using UnityEngine;
using UnityEngine.InputSystem;
public class MapManager : MonoBehaviour
{
    [SerializeField] private Vector2 worldSize;
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject islandIconReference;

    [SerializeField] private InputActionAsset inputs;
    
    private InputAction mapToggle;
    private bool mapOn;
    GameObject[] islands;
    GameObject[] islandIcons;
    float scaleFactor;
    void Start()
    {
        mapToggle = inputs.FindActionMap("Player").FindAction("MapToggle");
        mapToggle.Enable();

        islands = GameObject.FindGameObjectsWithTag("Island");
        islandIcons = new GameObject[islands.Length];
        scaleFactor = DetermineMapScaleFactor();

        Vector2 islandCoords;
        for (int i = 0; i < islands.Length; i++)
        {
            islandCoords = islands[i].transform.position;
            islandIcons[i] = Instantiate(islandIconReference, map.transform);
            islandIcons[i].transform.localPosition = scaleFactor * islandCoords;
            
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

    // Update is called once per frame
    void Update()
    {
        if (mapToggle.triggered)
        {
            if (mapOn)
            {
                mapOn = false;
                mapToggle.Disable();
            }
            else
            {
                mapOn = true;
                mapToggle.Enable();
            }
            map.SetActive(mapOn);
        }
    }
}
