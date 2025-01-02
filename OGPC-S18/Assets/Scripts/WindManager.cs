using UnityEngine;
using TMPro;

public class WindManager : MonoBehaviour
{
    [Header("References")]

    [SerializeField] private TextMeshProUGUI windBearingText;
    [SerializeField] private TextMeshProUGUI windSpeedText;


    [SerializeField] private Transform windIndicator;

    [Header("Starting Wind Condition")]
    [SerializeField] private float windDirection; // starting wind direction, 0 is North, 90 is East, 180 is South, and 270 is West
    [SerializeField] private float windSpeed; // starting windspeed

    [Header("Wind Generation Variables")]
    [SerializeField] private int stabilityLevels; // increases amount of deltas, increasing stability in strength and direction, minval = 2
    [SerializeField] private int mainMaxMag; // Maximum windspeed
    [SerializeField] private float randomMagRange; // Maximum variance from zero in random distribution for lowest delta
    [SerializeField] private float randomAngRange; // Maximum variance from zero in random distribution for lowest delta
    [SerializeField] private float maxMagDeltaStep; //Maximum delta for magnitude, increases for each delta
    [SerializeField] private float maxAngDeltaStep; //Maximum delta for angle, increases for each delta
    [SerializeField] private int stepMultiple; //Multiplier for the step, controlling how much each step increases
    [SerializeField] private float scalar; //Multiplier for the magnitude to help adjust strength
    [SerializeField] private int updateInterval; // Updates values every "this many frames"

    [Header("Debug Settings")]
    [SerializeField] private int debugTicksInterval; //gives debug message only every n gameticks
    private int debugTimer = 0;


    float[] magDeltas;
    float[] angDeltas;

    string magDebug;
    string angDebug;


    private void Start()
    {
        windIndicator.rotation = Quaternion.Euler(0, 0, 90 - windDirection);
        
        //Generation Variable/Array Stuff
        if (stabilityLevels < 2) {stabilityLevels = 2;}
    
        magDeltas = new float[stabilityLevels];
        angDeltas = new float[stabilityLevels];

        //starting deltas are all 0
        for (int i = 0; i < stabilityLevels; i++)
        {
            magDeltas[i] = 0f;
            angDeltas[i] = 0f;
        }
        debugTimer = debugTicksInterval - 1;
    }

    private void UpdateWind() 
    {
        magDeltas[0] = Random.Range(-1 * randomMagRange, randomMagRange);
        angDeltas[0] = Random.Range(-1 * randomAngRange, randomAngRange);

        for (int i = 1; i < stabilityLevels; i++)
        {
            magDeltas[i] = Mathf.Clamp(magDeltas[i] + magDeltas[i-1], -stepMultiple * i * maxMagDeltaStep, stepMultiple * i*maxMagDeltaStep); //adding the delta(+ or -) to the next delta to get the new ROC
            angDeltas[i] = Mathf.Clamp(angDeltas[i] + angDeltas[i-1], -stepMultiple * i * maxAngDeltaStep, stepMultiple * i*maxAngDeltaStep); //adding the delta(+ or -) to the next delta to get the new ROC
        }

        windDirection = windDirection + angDeltas[^1];
        windSpeed = windSpeed + scalar*magDeltas[^1];

        windIndicator.rotation = Quaternion.Euler(0, 0, 90 - windDirection);


    }

    private void UpdateUI()
    {
        windSpeedText.text = $"Wind Speed: {windSpeed.ToString("F1")}";
        windBearingText.text = $"Wind Bearing: {windDirection.ToString("F1")}";
        windIndicator.rotation = Quaternion.Euler(0, 0, 90 - windDirection);
    }

    int count = 0;
    private void Update()
    {
        count++;
        if (count == updateInterval)
        {
            count = 0;
            UpdateWind();
            UpdateUI();
        }
        debugTimer++;
        if (debugTimer == debugTicksInterval) 
        {
            debugTimer = 0;
            Debug.Log($"The wind direction is {windDirection}, and the wind speed is {windSpeed}");
            Debug.Log($"magDeltas: [{string.Join(", ", magDeltas)}] angDeltas: [{string.Join(", ", angDeltas)}]");       
        }
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
