using UnityEngine;

public class WindManager : MonoBehaviour
{
    [SerializeField] private float windDirection; // 0 is North, 90 is East, 180 is South, and 270 is Wesy
    [SerializeField] private float windSpeed;

    [SerializeField] private Transform windIndicator;

    private void Start()
    {
        windDirection = 0f;
        Debug.Log($"The Wind Direction is {windDirection}");
        windIndicator.rotation = Quaternion.Euler(0, 0, 90 - windDirection); 
    }

    public float GetWindDirection()
    {
        return windDirection;
    }

    public float GetWindSpeed()
    {
        return windSpeed;
    }
}
