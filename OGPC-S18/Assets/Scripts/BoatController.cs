using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    [Header("Ship Data")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedAccelerationMod;
    [SerializeField] private float windAccelerationMod;
    [SerializeField] private float stallAngle;


    [Header("One Time")]
    [SerializeField] private Transform sail;
    [SerializeField] private TextMeshProUGUI boatSpeedText;

    private Rigidbody2D rb;
    private WindManager windManager;
    private float relativeWindDirection;
    private float boatSpeed;

    private void Start()
    {
        windManager = FindFirstObjectByType<WindManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        RotateSailToMatchWind();
        boatSpeed = rb.linearVelocity.magnitude;
        BoatForwardVelocity();

        UpdateUI();
    }

    private void RotateSailToMatchWind()
    {
        relativeWindDirection = (windManager.GetWindDirection() + transform.localRotation.eulerAngles.z) % 360;
        Debug.Log(relativeWindDirection);

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
            Debug.Log(sailAngleSpeedMod);
        }

        float speedMagnitude = sailAngleSpeedMod * windManager.GetWindSpeed() * speedAccelerationMod;

        if (boatSpeed < maxSpeed)
            rb.AddRelativeForceY(speedMagnitude);
    }
}
