using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoatController : MonoBehaviour
{
    [Header("Ship Data")]
    [SerializeField] private float absoluteMaxSpeed; // Speed boat can never go above of
    [SerializeField] private float relativeToWindMaxSpeed; // Percentage of wind that can be captured
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
    [SerializeField] private InputActionAsset inputActions;

    [Header("Debug References")]
    [SerializeField] private TextMeshProUGUI boatHeadingText;
    [SerializeField] private TextMeshProUGUI boatSpeedText;
    float boatHeading;
    float boatSpeed;

    private bool boatSailing = true;
    private InputAction sailToggle;

    private bool sailEnabled = true;
    private Rigidbody2D rb;
    private WindManager windManager;
    private CurrentManager currentManager;
    private RudderController rudderController;
    private float relativeWindDirection;
    private float boatWaterSpeed;
    private Vector2 boatWaterVector;

    private void OnEnable()
    {
        InputActionMap playerControls = inputActions.FindActionMap("Player");
        sailToggle = playerControls.FindAction("SailToggle");

        sailToggle.Enable();
        Debug.Log("SailToggle enabled");
    }

    private void OnDisable()
    {
        sailToggle.Disable();
    }

    private void Start()
    {
        windManager = FindFirstObjectByType<WindManager>();
        currentManager = FindFirstObjectByType<CurrentManager>();
        rb = GetComponent<Rigidbody2D>();
        rudderController = GetComponent<RudderController>();
        rudderController.SetRudderMoveSpeed(rudderMoveSpeed);
        
        rb.linearVelocity = rb.linearVelocity + UsefulStuff.Convert.PolarToVector(currentManager.GetCurrentDirection(), currentManager.GetCurrentSpeed() * currentMaxSpeedMod);
    }

    private void Update()
    {
        UpdateUI();
        if (!boatSailing)
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
        if (!boatSailing) return;
        
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
        boatWaterVector = rb.linearVelocity - UsefulStuff.Convert.PolarToVector(currentManager.GetCurrentDirection(), currentManager.GetCurrentSpeed() * currentMaxSpeedMod);
        boatWaterSpeed = boatWaterVector.magnitude;

        boatHeading = (360 - rb.transform.localEulerAngles.z) % 360;
        boatSpeed = rb.linearVelocity.magnitude;
    }

    private void RotateSailToMatchWind()
    {
        relativeWindDirection = (windManager.GetWindDirection() + transform.localRotation.eulerAngles.z) % 360;

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

        float speedMagnitude = sailAngleSpeedMod * windManager.GetWindSpeed() * speedAccelerationMod;

        // Makes sure boat doesn't exceed maximum speed relative to current
        if (boatWaterSpeed < absoluteMaxSpeed && boatWaterSpeed < relativeToWindMaxSpeed * windManager.GetWindSpeed())
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
    }

    private void BoatRotation()
    {
        float rudderPosition = UsefulStuff.Misc.Round(rudderController.GetRudderPosition(),4);

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

    public float GetBoatSpeed()
    {
        return boatSpeed;
    }

    public void Dock(Transform dockTransform)
    {
        boatSailing = false;
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
        boatSailing = true;
        sailEnabled = true;
        sail.gameObject.SetActive(sailEnabled);

        transform.parent = null;
    }
}