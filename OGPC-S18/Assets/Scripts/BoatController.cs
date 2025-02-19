using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class BoatController : MonoBehaviour
{
    [Header("Ship Data")]
    [SerializeField] private float maxSpeed; //Make less than 100
    [SerializeField] private float maxSpeedUnderOars;
    [SerializeField] private float oarAcceleration;
    [SerializeField] private float speedAccelerationMod;
    [SerializeField] private float maxRudderAngle;
    [SerializeField] private float turningSpeed;
    [SerializeField] private float rudderMoveSpeed;
    [SerializeField] private float maxRotationResistance;
    [SerializeField] private float baseResistance;
    [SerializeField] private float stallAngle;
    [SerializeField] private float currentAccelerationMod;
    [SerializeField] private float currentMaxSpeedMod;

    [Header("References")]
    [SerializeField] private Transform sail;
    [SerializeField] private Transform player;
    [SerializeField] private Transform compass;
    [SerializeField] private TextMeshProUGUI boatHeadingText;
    [SerializeField] private TextMeshProUGUI boatSpeedText;
    [SerializeField] private InputActionAsset inputActions;

    [Header("Debug Settings")]
    [SerializeField] private int debugTicksInterval; //gives debug message only every n gameticks

    private enum BoatState
    {
        sailing,
        docked,
    }
    private BoatState boatState;

    private InputAction sailToggle;

    private bool sailEnabled = true;
    private Vector2 sailVelocity = Vector2.zero;

    private Rigidbody2D rb;
    private WindManager windManager;
    private CurrentManager currentManager;
    private RudderController rudderController;
    private float relativeWindDirection;
    private float boatSpeed;
    private float boatWaterSpeed;
    private float boatHeading;
    private Vector2 boatWaterVector;
    private float boatVelocityMagnitude;
    private int debugTimer = 0;
    private bool logDebug = false;

    private void OnEnable()
    {
        var playerControls = inputActions.FindActionMap("Player");
        sailToggle = playerControls.FindAction("SailToggle");

        sailToggle.Enable();
    }

    private void OnDisable()
    {
        sailToggle.Disable();
    }

    private void Start()
    {
        boatState = BoatState.sailing;
        windManager = FindFirstObjectByType<WindManager>();
        currentManager = FindFirstObjectByType<CurrentManager>();
        rb = GetComponent<Rigidbody2D>();
        rudderController = GetComponent<RudderController>();
        rudderController.SetRudderMoveSpeed(rudderMoveSpeed);
        
        rb.linearVelocity = rb.linearVelocity + UsefulStuff.Convert.Polar2Vector(currentManager.currentDirection, currentManager.currentSpeed * currentMaxSpeedMod);

        debugTimer = debugTicksInterval - 1;
    }

    private void Update()
    {
        debugTimer++;
        if (debugTimer == debugTicksInterval)
        {
            logDebug = true;
            debugTimer = 0;
        }

        UpdateUI();
        logDebug = false;

        if (boatState != BoatState.sailing)
        {
            return;
        }

        if (sailToggle.triggered)
        {
            sailEnabled = !sailEnabled;
            sail.gameObject.SetActive(sailEnabled);
        }
        if (sailEnabled)
        {
            RotateSailToMatchWind();
        }
    }

    private void FixedUpdate()
    {
        UpdateBoatData();
        if (boatState != BoatState.sailing) return;
        
        if (sailEnabled)
        {
            AddWind2Boat();
        }
        else
        {
            AddOar2Boat();
        }

        AddCurrent2Boat();
        BoatRotation();
    }

    private void UpdateBoatData()
    {
        boatHeading = (360-rb.transform.localEulerAngles.z)%360;
        boatSpeed = rb.linearVelocity.magnitude;
        boatVelocityMagnitude = rb.linearVelocity.magnitude;

        Vector2 currentVector = currentManager.GetCurrentVector() * currentMaxSpeedMod;
        boatWaterVector = rb.linearVelocity - currentVector;
        boatWaterSpeed = boatWaterVector.magnitude;
        if (logDebug)
        {
            Debug.Log($"Boats True Speed: {boatSpeed.ToString()}");
            Debug.Log("Boat Heading: " + boatHeading.ToString());
        }
    }

    private void RotateSailToMatchWind()
    {
        float windDirection = UsefulStuff.Convert.Angle2Bearing(UsefulStuff.Convert.Vector2Polar(windManager.GetWindVector())[0]);
        relativeWindDirection = (windDirection + transform.localRotation.eulerAngles.z) % 360;

        float windSailRotation;
        if (relativeWindDirection > 0 && relativeWindDirection < 180)
        {
            windSailRotation = (-relativeWindDirection / 2) % 90;
        }
        else
        {
            windSailRotation = ((360 - relativeWindDirection) / 2) % 90;
        }
        sail.transform.localRotation = Quaternion.Euler(0, 0, windSailRotation);
    }

    private void UpdateUI()
    {
        boatHeadingText.text = $"Boat Heading: {boatHeading.ToString("F1")}";
        boatSpeedText.text = $"Boat Speed: {boatSpeed.ToString("F1")}";
        compass.rotation = Quaternion.Euler(0,0,90 - player.eulerAngles.z);
    }

    private void AddWind2Boat()
    {
        float sailAngle = sail.transform.localEulerAngles.z;
        float sailAngleSpeedMod = Mathf.Cos(Mathf.Deg2Rad * sailAngle);
        
        if (relativeWindDirection < 180 + stallAngle && relativeWindDirection > 180 - stallAngle)
        {
            // Boat is stalling
            if (relativeWindDirection == 180)
            {
                sailAngleSpeedMod = 0;
            }
            else
            {
                sailAngleSpeedMod /= stallAngle + 1 - Mathf.Abs(180 - relativeWindDirection);
            }
        }

        float speedMagnitude = sailAngleSpeedMod * windManager.GetWindVector().magnitude * speedAccelerationMod;

        // Makes sure boat doesn't exceed maximum speed relative to current
        if (boatWaterSpeed < maxSpeed/100 * windManager.GetWindVector().magnitude)
            rb.AddRelativeForceY(speedMagnitude);
    }

    private void AddOar2Boat()
    {
        if (boatWaterSpeed < maxSpeedUnderOars)
            rb.AddRelativeForceY(oarAcceleration);
    }
    
    private void AddCurrent2Boat()
    {
        rb.AddForce(-boatWaterVector * currentAccelerationMod);

        if (logDebug) {Debug.Log("BoatWaterSpeed: " + boatWaterVector.magnitude.ToString());}
    }

    private void BoatRotation()
    {
        float rudderPosition = UsefulStuff.Miscellaneous.Round(rudderController.GetRudderPosition(),4);

        if (logDebug) 
        {
            Debug.Log("Rudder Position: " + rudderPosition);
        }
        float rudderAngle = rudderPosition * maxRudderAngle;

        float targetPosition = transform.eulerAngles.z + rudderAngle;
        float smoothRotation = Mathf.LerpAngle(transform.eulerAngles.z, targetPosition, turningSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, smoothRotation);

        if (rudderPosition != 0)
        {
            //Boat is turning
            //Increase linear resistance
            rb.linearDamping = Mathf.Abs(rudderPosition) * maxRotationResistance + baseResistance;
        }
    }

    public void Dock(Transform dockTransform)
    {
        boatState = BoatState.docked;
        sailEnabled = false;
        sail.gameObject.SetActive(sailEnabled);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0;

        transform.parent = dockTransform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void UnDock()
    {
        boatState = BoatState.sailing;
        sailEnabled = true;
        sail.gameObject.SetActive(sailEnabled);

        transform.parent = null;
    }
}
