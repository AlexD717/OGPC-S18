using UnityEngine;

public class MapManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    //Not production ready, just figuring out how this works

    [SerializeField] private Transform island;
    [SerializeField] private RectTransform islandIcon;
    void Start()
    {
        Vector2 islandCoords;
        islandCoords = island.position;
        Debug.Log(islandCoords.x.ToString() + " " + islandCoords.y.ToString());
        islandIcon.transform.localPosition = islandCoords;
        Debug.Log(islandIcon.position.x.ToString() + " " + islandIcon.position.y.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
