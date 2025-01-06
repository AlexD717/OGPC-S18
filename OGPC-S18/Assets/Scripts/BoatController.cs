using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    [Header("Ship Data")]
    [SerializeField] private float maxSpeed;
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
    [SerializeField] private TextMeshProUGUI boatHeadingText;
    [SerializeField] private TextMeshProUGUI boatSpeedText;

    [Header("Debug Settings")]
    [SerializeField] private int debugTicksInterval; //gives debug message only every n gameticks

    private Rigidbody2D rb;
    private WindManager windManager;
    private CurrentManager currentManager;
    private RudderController rudderController;
    private float relativeWindDirection;
    private float boatSpeed;
    private float boatWaterSpeed;
    private float boatHeading;
    private Vector2 boatWaterVector;

    private int debugTimer = 0;
    private bool logDebug = false;

    private void Start()
    {
        windManager = FindFirstObjectByType<WindManager>();
        currentManager = FindFirstObjectByType<CurrentManager>();
        rb = GetComponent<Rigidbody2D>();
        rudderController = GetComponent<RudderController>();
        rudderController.SetRudderMoveSpeed(rudderMoveSpeed);
        
        rb.linearVelocity = rb.linearVelocity + UsefulStuff.Polar2Vector(currentManager.currentDirection, currentManager.currentSpeed * currentMaxSpeedMod);

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

        RotateSailToMatchWind();

        UpdateUI();
        logDebug = false;
    }

    private void FixedUpdate()
    {
        UpdateBoatData();

        AddWind2Boat();
        AddCurrent2Boat();
        BoatRotation();
    }

    private void UpdateBoatData()
    {
        boatHeading = (360-rb.transform.localEulerAngles.z)%360;
        boatSpeed = rb.linearVelocity.magnitude;

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
        if (boatWaterSpeed < maxSpeed)
            rb.AddRelativeForceY(speedMagnitude);
    }
    
    private void AddCurrent2Boat()
    {
        rb.AddForce(-boatWaterVector * currentAccelerationMod);

        if (logDebug) {Debug.Log("BoatWaterSpeed: " + boatWaterVector.magnitude.ToString());}
    }

    private void BoatRotation()
    {
        float rudderPosition = UsefulStuff.Round(rudderController.GetRudderPosition(),4);

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
    
}
