using UnityEngine;
using TMPro;

public class CurrentManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private TextMeshProUGUI currentBearingText;
    [SerializeField] private TextMeshProUGUI currentSpeedText;
    [SerializeField] private Transform currentIndicator;
    [SerializeField] private Transform player;


    [Header("Starting Current Condition")]
    [SerializeField] public float currentHeading; // starting current direction, 0 is North, 90 is East, 180 is South, and 270 is West
    [SerializeField] public float currentSpeed; // starting current speed

    [Header("Current Generation Variables")]
    [SerializeField] private int stabilityLevels; // increases amount of deltas, increasing stability in strength and direction, minval = 2
    [SerializeField] private float RandomMagRange; // Maximum variance from zero in random distribution for lowest delta
    [SerializeField] private float RandomAngRange; // Maximum variance from zero in random distribution for lowest delta
    [SerializeField] private float maxMagDeltaStep; //Maximum delta for magnitude, increases for each delta
    [SerializeField] private float maxAngDeltaStep; //Maximum delta for angle, increases for each delta
    [SerializeField] private float stepMultiple; //Multiplier for the step, controlling how much each step increases
    [SerializeField] private int updateInterval; // Updates values every "this many frames"
    [SerializeField] private Vector2 currentSpeedRange; //Clamps on wind range
    [SerializeField] private Vector2 currentHeadingRange; //Clamps on wind direction



    float[] magDeltas;
    float[] angDeltas;

    Vector2 currentVector;

    private void Start()
    {   

        if (stabilityLevels < 2) {stabilityLevels = 2;}
    
        magDeltas = new float[stabilityLevels];
        angDeltas = new float[stabilityLevels];


        //starting deltas are all 0
        for (int i = 0; i < stabilityLevels; i++)
        {
            magDeltas[i] = 0f;
            angDeltas[i] = 0f;
        }

        currentVector = UsefulStuff.Convert.PolarToVector(currentHeading,currentSpeed);
    }

    private void UpdateCurrent() 
    {
        magDeltas[0] = Random.Range(-RandomMagRange, RandomMagRange);
        angDeltas[0] = Random.Range(-RandomAngRange, RandomAngRange);

        for (int i = 1; i < stabilityLevels; i++)
        {
            magDeltas[i] = Mathf.Clamp(magDeltas[i] + magDeltas[i-1], -stepMultiple * i * maxMagDeltaStep, stepMultiple * i*maxMagDeltaStep); //adding the delta(+ or -) to the next delta to get the new ROC
            angDeltas[i] = Mathf.Clamp(angDeltas[i] + angDeltas[i-1], -stepMultiple * i * maxAngDeltaStep, stepMultiple * i*maxAngDeltaStep); //adding the delta(+ or -) to the next delta to get the new ROC
        }

        currentHeading = (currentHeading + angDeltas[^1]+360)%360;
        
        currentHeading = UsefulStuff.Misc.ClampAngle(currentHeading, currentHeadingRange.x, currentHeadingRange.y);
        if (currentHeading == currentHeadingRange.x || currentHeading == currentHeadingRange.y) {for (int i=0; i < angDeltas.Length;i++) {angDeltas[i] = 0f;}}

        currentSpeed = Mathf.Clamp(currentSpeed + magDeltas[^1], currentSpeedRange.x, currentSpeedRange.y);
        if (currentSpeed == currentSpeedRange.x || currentSpeed == currentSpeedRange.y) {for (int i=0; i < magDeltas.Length;i++) {magDeltas[i] = 0f;}}



        currentVector = UsefulStuff.Convert.PolarToVector(currentHeading,currentSpeed);
    }

    private void UpdateUI()
    {
        currentSpeedText.text = $"Current speed: {currentSpeed.ToString("F1")}";
        currentBearingText.text = $"Current Bearing: {currentHeading.ToString("F1")}";
        currentIndicator.rotation = Quaternion.Euler(0, 0, 90 - (currentHeading+player.eulerAngles.z));
        currentIndicator.localScale = new Vector3(currentSpeed/currentSpeedRange.y, currentSpeed/currentSpeedRange.y, 1f);
    }


    int count = 0;
    private void Update()
    {
        count++;
        if (count == updateInterval)
        {
            count = 0;
            UpdateCurrent();
        }
        UpdateUI();
    }

    public float GetCurrentDirection()
    {
        return currentHeading;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}

