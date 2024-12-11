using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    private WindManager windManager;
    private float relativeWindDirection;

    [SerializeField] private Transform sail;

    private void Start()
    {
        windManager = FindFirstObjectByType<WindManager>();
    }

    private void Update()
    {
        RotateSailToMatchWind();
        SailSpeed();
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

    private void SailSpeed()
    {
        float sailAngle = sail.transform.eulerAngles.z;
        float sailAngleSpeedMod = Mathf.Cos(Mathf.Deg2Rad * sailAngle);
        float speedMagnitude = sailAngleSpeedMod * windManager.GetWindSpeed();
    }
}
