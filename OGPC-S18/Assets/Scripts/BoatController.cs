using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

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
    [SerializeField] private int maxShipHealth;
    [SerializeField] private int damageFromCollisions = 0;

    [Header("References")]
    [SerializeField] private Transform sail;
    [SerializeField] private Transform player;
    [SerializeField] private Transform compass;
    [SerializeField] private Scrollbar speedometer;
    [SerializeField] private InputActionAsset inputActions;

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
    public float shipHealth;
    private LevelManager levelManager;

    private void OnEnable()
    {
        InputActionMap playerControls = inputActions.FindActionMap("Player");
        sailToggle = playerControls.FindAction("SailToggle");

        sailToggle.Enable();
    }

    private void OnDisable()
    {
        sailToggle.Disable();
    }

    private void Start()
    {
        shipHealth = (float)maxShipHealth;
        windManager = FindFirstObjectByType<WindManager>();
        currentManager = FindFirstObjectByType<CurrentManager>();
        rb = GetComponent<Rigidbody2D>();
        rudderController = GetComponent<RudderController>();
        rudderController.SetRudderMoveSpeed(rudderMoveSpeed);
        levelManager = FindFirstObjectByType<LevelManager>();
        
        rb.linearVelocity = rb.linearVelocity + UsefulStuff.Convert.PolarToVector(currentManager.GetCurrentDirection(), currentManager.GetCurrentSpeed() * currentMaxSpeedMod);

    
    }

    private void Update()
    {
        if (shipHealth <= 0)
        {
            levelManager.PlayerLost();
            return;
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Island") || collision.gameObject.transform.parent.gameObject.CompareTag("Port"))
        {
            // Boat is colliding with an island or obstacle
            shipHealth -= (float)damageFromCollisions;
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
        compass.rotation = Quaternion.Euler(0,0,90 - player.eulerAngles.z);
        if (sailEnabled)
        {
            speedometer.size = boatWaterSpeed / maxSpeed;
        }
        else
        {
            speedometer.size = boatWaterSpeed / maxSpeedUnderOars;
        }
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
        if (boatWaterSpeed < maxSpeed/100 * windManager.GetWindSpeed())
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
