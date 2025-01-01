using UnityEngine;
using TMPro;
public class WindManager : MonoBehaviour
{
    [Header("References")]

    [SerializeField] private TextMeshProUGUI windSpeedText;
    [SerializeField] private TextMeshProUGUI windBearingText;


    [SerializeField] private Transform windIndicator;

    [Header("Starting Wind Condition")]
    [SerializeField] private float windDirection; // starting wind direction, 0 is North, 90 is East, 180 is South, and 270 is West
    [SerializeField] private float windSpeed; // starting windspeed

    [Header("Wind Generation Variables")]
    [SerializeField] private int stabilityLevels; // increases amount of deltas, increasing stability in strength and direction, minval = 2
    [SerializeField] private int mainMaxMag; // Maximum windspeed
    [SerializeField] private float RandomMagRange; // Maximum variance from zero in random distribution for lowest delta
    [SerializeField] private float RandomAngRange; // Maximum variance from zero in random distribution for lowest delta
    [SerializeField] private float maxMagDeltaStep; //Maximum delta for magnitude, increases for each delta
    [SerializeField] private float maxAngDeltaStep; //Maximum delta for angle, increases for each delta
    [SerializeField] private int stepMultiple; //Multiplier for the step, controlling how much each step increases

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

    private float Limit(float n, float max)
    {
        if (n > max) {n = max;}
        if (n < -1*max) {n = -1*max;}
        return n;
    }


    private void UpdateWind() 
    {
        magDeltas[0] = Random.Range(-1 * RandomMagRange, RandomMagRange);
        angDeltas[0] = Random.Range(-1 * RandomAngRange, RandomAngRange);

        for (int i = 1; i < stabilityLevels; i++)
        {
            magDeltas[i] = Limit(magDeltas[i] + magDeltas[i-1], stepMultiple*i*maxMagDeltaStep); //adding the delta(+ or -) to the next delta to get the new ROC
            angDeltas[i] = Limit(angDeltas[i] + angDeltas[i-1], stepMultiple*i*maxAngDeltaStep); //adding the delta(+ or -) to the next delta to get the new ROC
        }

        windDirection = windDirection + angDeltas[^1];
        windSpeed = windSpeed + magDeltas[^1];

    }

    private void UpdateUI()
    {
        windSpeedText.text = $"Windspeed: {windSpeed.ToString("F1")}";
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
            Debug.Log($"[{string.Join(", ", magDeltas)}] : [{string.Join(", ", angDeltas)}]");       
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
