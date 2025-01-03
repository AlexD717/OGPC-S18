using System.Runtime.InteropServices.WindowsRuntime;
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
    [SerializeField] private float currentPushPower;

    [Header("One Time")]
    [SerializeField] private Transform sail;
    [SerializeField] private TextMeshProUGUI boatSpeedText;

    [Header("Debug Settings")]
    [SerializeField] private int debugTicksInterval; //gives debug message only every n gameticks

    private float previousSailMagnitude = 0;
    private Vector2 sailVelocity = Vector2.zero;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private WindManager windManager;
    private CurrentManager currentManager;
    private RudderController rudderController;
    private float relativeWindDirection;
    private float boatVelocityMagnitude;
    private int debugTimer = 0;

    private void Start()
    {
        windManager = FindFirstObjectByType<WindManager>();
        currentManager = FindFirstObjectByType<CurrentManager>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        rudderController = GetComponent<RudderController>();
        rudderController.SetRudderMoveSpeed(rudderMoveSpeed);

        debugTimer = debugTicksInterval - 1;
    }

    private void Update()
    {
        RotateSailToMatchWind();
        ApplyCurrent();

        UpdateUI();
    }

    private void FixedUpdate()
    {
        boatVelocityMagnitude = rb.linearVelocity.magnitude;
        BoatForwardVelocity();

        BoatRotation();
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
        boatSpeedText.text = $"Boat Speed: {boatVelocityMagnitude.ToString("F1")}";
    }

    private void BoatForwardVelocity()
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

        if (boatVelocityMagnitude < maxSpeed)
            rb.AddRelativeForceY(speedMagnitude);
    }

    private void BoatRotation()
    {
        float rudderPosition = rudderController.GetRudderPosition();
        if (Mathf.Abs(rudderPosition) < 0.0001f) 
        {
            rudderPosition = 0;
        }
        else if (rudderPosition > 0.9999f)
        {
            rudderPosition = 1;
        }

        debugTimer++;
        if (debugTimer == debugTicksInterval) 
        {
            debugTimer = 0;
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

    private void ApplyCurrent()
    {
        float currentDirection = currentManager.GetCurrentDirection();

        float relativeCurrentDirection = (transform.eulerAngles.z + currentDirection) % 360; //180 is facing into the wind, 0 is facing away
        float sideCurrentPowerMod = Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * relativeCurrentDirection));
        float frontCurrentPowerMod = Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * relativeCurrentDirection));

        float sideToFrontRatio = boxCollider.size.y / boxCollider.size.x;
        float currentPowerMod = (sideCurrentPowerMod * sideToFrontRatio + frontCurrentPowerMod) * currentPushPower;

        Vector2 currentVector = new Vector2(Mathf.Cos(currentDirection * Mathf.Deg2Rad), Mathf.Sin(currentDirection * Mathf.Deg2Rad));
        transform.Translate(-currentVector * currentPushPower * currentManager.GetCurrentSpeed() * Time.deltaTime, Space.World);
    }
}
