using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    [Header("Ship Data")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedAccelerationMod;

    [Header("One Time")]
    [SerializeField] private Transform sail;
    private Rigidbody2D rb;
    private WindManager windManager;
    private float relativeWindDirection;
    [SerializeField] private TextMeshProUGUI boatSpeedText;

    private void Start()
    {
        windManager = FindFirstObjectByType<WindManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        RotateSailToMatchWind();
        SailSpeed();

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
        float speed = rb.linearVelocity.magnitude;
        boatSpeedText.text = $"Boat Speed: {speed.ToString("F1")}";
    }

    private void SailSpeed()
    {
        float sailAngle = sail.transform.eulerAngles.z;
        float sailAngleSpeedMod = Mathf.Cos(Mathf.Deg2Rad * sailAngle);
        float speedMagnitude = sailAngleSpeedMod * windManager.GetWindSpeed() * speedAccelerationMod;
        rb.AddRelativeForceY(speedMagnitude);
    }
}
