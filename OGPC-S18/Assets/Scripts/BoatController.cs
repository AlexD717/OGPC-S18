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

    [Header("One Time")]
    [SerializeField] private Transform sail;
    [SerializeField] private TextMeshProUGUI boatSpeedText;

    [Header("Debug Settings")]
    [SerializeField] private int debugTicksInterval; //gives debug message only every n gameticks

    private Rigidbody2D rb;
    private WindManager windManager;
    private RudderController rudderController;
    private float relativeWindDirection;
    private float boatSpeed;
    private int debugTimer = 0;

    private void Start()
    {
        windManager = FindFirstObjectByType<WindManager>();
        rb = GetComponent<Rigidbody2D>();
        rudderController = GetComponent<RudderController>();
        rudderController.SetRudderMoveSpeed(rudderMoveSpeed);

        debugTimer = debugTicksInterval - 1;
    }

    private void Update()
    {
        RotateSailToMatchWind();
        boatSpeed = rb.linearVelocity.magnitude;
        BoatForwardVelocity();
        BoatRotation();

        UpdateUI();
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
        boatSpeedText.text = $"Boat Speed: {boatSpeed.ToString("F1")}";
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

        // Makes sure boat doesn't exceed maximum speed
        if (boatSpeed < maxSpeed)
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
