using UnityEngine;
using TMPro;

public class CurrentManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private TextMeshProUGUI currentBearingText;
    [SerializeField] private TextMeshProUGUI currentSpeedText;
    [HideInInspector] public Vector2 deltaCurrentTick; //Change in current from previous tick to the next


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

    [Header("Debug Settings")]
    [SerializeField] private int debugTicksInterval; //gives debug message only every n gameticks
    private int debugTimer = 0;


    float[] magDeltas;
    float[] angDeltas;

    string magDebug;
    string angDebug;



    private void Start()
    {
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
        currentSpeed = currentSpeed + magDeltas[^1];
        currentSpeed = Mathf.Clamp(currentSpeed,0,maxCurrentSpeed);

        currentIndicator.rotation = Quaternion.Euler(0, 0, 90 - currentDirection);


        deltaCurrentTick = new Vector2(magDeltas[^1]*Mathf.Sin(angDeltas[^1]*Mathf.Deg2Rad),magDeltas[^1]*Mathf.Cos(angDeltas[^1]*Mathf.Deg2Rad))/updateInterval;
    }

    private void UpdateUI()
    {
        currentSpeedText.text = $"Current speed: {currentSpeed.ToString("F1")}";
        currentBearingText.text = $"Current Bearing: {currentDirection.ToString("F1")}";

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
        debugTimer++;
        if (debugTimer == debugTicksInterval) 
        {
            debugTimer = 0;
            Debug.Log($"The current's direction is {currentDirection},  its speed is {currentSpeed}. Last Delta: Ang: {angDeltas[^1].ToString()}, Mag: {magDeltas[^1].ToString()}");
            //Debug.Log($"magDeltas: [{string.Join(", ", magDeltas)}] angDeltas: [{string.Join(", ", angDeltas)}]");       
        }
    }
}
