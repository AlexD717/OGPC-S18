using UnityEngine;

public class WindManager : MonoBehaviour
{
    [Header("Starting Wind Condition")]
    [SerializeField] private float windHeading; // starting wind direction, 0 is North, 90 is East, 180 is South, and 270 is West
    [SerializeField] private float windSpeed; // starting windspeed

    [Header("Wind Generation Variables")]
    [SerializeField] private int stabilityLevels; // increases amount of deltas, increasing stability in strength and direction, minval = 2
    [SerializeField] private float randomMagRange; // Maximum variance from zero in random distribution for lowest delta
    [SerializeField] private float randomAngRange; // Maximum variance from zero in random distribution for lowest delta
    [SerializeField] private float maxMagDeltaStep; //Maximum delta for magnitude, increases for each delta
    [SerializeField] private float maxAngDeltaStep; //Maximum delta for angle, increases for each delta
    [SerializeField] private float stepMultiple; //Multiplier for the step, controlling how much each step increases
    [SerializeField] private int updateInterval; // Updates values every "this many frames"
    int frameCount = 0; // Frame counter
    [SerializeField] private Vector2 windSpeedRange; //Clamps on wind range
    [SerializeField] private Vector2 windHeadingRange; //Clamps on wind direction
    private Transform player;
    float[] magDeltas;
    float[] angDeltas;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
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

        windHeading = (windHeading + angDeltas[^1] + 360) % 360;
        windHeading = MathUtilities.ClampAngle(windHeading, windHeadingRange.x, windHeadingRange.y);
        if (windHeading == windHeadingRange.x || windHeading == windHeadingRange.y) {for (int i=0; i < angDeltas.Length;i++) {angDeltas[i] = 0f;}}
        
        windSpeed = Mathf.Clamp(windSpeed + magDeltas[^1], windSpeedRange.x, windSpeedRange.y); 
        if (windSpeed == windSpeedRange.x || windSpeed == windSpeedRange.y) {for (int i=0; i < magDeltas.Length;i++) {magDeltas[i] = 0f;}}

    }
    private void Update()
    {
        frameCount++;
        if (frameCount == updateInterval)
        {
            frameCount = 0;

            Hurricane[] hurricanes = FindObjectsByType<Hurricane>(FindObjectsSortMode.None);
            Hurricane closestHurricane = null;
            float closestDistance = Mathf.Infinity;
            foreach (Hurricane hurricane in hurricanes)
            {
                float distance = Vector2.Distance(hurricane.transform.position, player.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestHurricane = hurricane;
                }
            }
            if (closestHurricane != null)
            {
                if (closestHurricane.IsPlayerInHurricane())
                {
                    windHeading = closestHurricane.GetWindDirection();
                    windSpeed = closestHurricane.GetWindSpeed();
                    return;
                }
            }
            UpdateWind();
        }
    }

    public float GetWindDirection()
    {
        return windHeading;
    }

    // Returns the wind direction in radians with 0 being east and normal math stuff
    public float GetWindRadAngle()
    {
        return (90 - windHeading) % 360 * Mathf.Deg2Rad;
    }

    public float GetWindSpeed()
    {
        return windSpeed;
    }
}
