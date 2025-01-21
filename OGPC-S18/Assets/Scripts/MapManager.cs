using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Vector2 worldSize;
    [SerializeField] private Canvas map;
    [SerializeField] private GameObject islandIconReference;
    
    GameObject[] islands;
    GameObject[] islandIcons;
    float scaleFactor;
    void Start()
    {
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

        Vector2 mapSize = new Vector2(2*454,2*230);
        xFactor = worldSize.x/mapSize.x;
        yFactor = worldSize.y/mapSize.y;
        realFactor = 1 / Mathf.Max(xFactor,yFactor);

        return realFactor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
