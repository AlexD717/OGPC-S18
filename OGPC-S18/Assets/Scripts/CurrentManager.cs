using UnityEngine;
using TMPro;

public class CurrentManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private TextMeshProUGUI currentBearingText;
    [SerializeField] private TextMeshProUGUI currentSpeedText;

    [SerializeField] private Transform currentIndicator;

    [Header("Starting Current Condition")]
    [SerializeField] public float currentDirection; // starting current direction, 0 is North, 90 is East, 180 is South, and 270 is West
    [SerializeField] public float currentSpeed; // starting current speed

    [Header("Current Generation Variables")]
    [SerializeField] private int maxCurrentSpeed; // Maximum speed of current
    [SerializeField] private int stabilityLevels; // increases amount of deltas, increasing stability in strength and direction, minval = 2
    [SerializeField] private float RandomMagRange; // Maximum variance from zero in random distribution for lowest delta
    [SerializeField] private float RandomAngRange; // Maximum variance from zero in random distribution for lowest delta
    [SerializeField] private float maxMagDeltaStep; //Maximum delta for magnitude, increases for each delta
    [SerializeField] private float maxAngDeltaStep; //Maximum delta for angle, increases for each delta
    [SerializeField] private float stepMultiple; //Multiplier for the step, controlling how much each step increases
    [SerializeField] private int updateInterval; // Updates values every "this many frames"



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

        currentVector = UsefulStuff.Polar2Vector(currentDirection,currentSpeed);
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

        currentDirection = currentDirection + angDeltas[^1];
        currentSpeed = Mathf.Clamp(currentSpeed + magDeltas[^1],0,maxCurrentSpeed);

        currentVector = UsefulStuff.Polar2Vector(currentDirection,currentSpeed);
    }

    private void UpdateUI()
    {
        currentSpeedText.text = $"Current speed: {currentSpeed.ToString("F1")}";
        currentBearingText.text = $"Current Bearing: {currentDirection.ToString("F1")}";
        currentIndicator.rotation = Quaternion.Euler(0, 0, 90 - currentDirection);
    }


    int count = 0;
    private void Update()
    {
        count++;
        if (count == updateInterval)
        {
            count = 0;
            UpdateCurrent();
            UpdateUI();
        }
    }

    public Vector2 GetCurrentVector()
    {
        return currentVector;
    }
}
