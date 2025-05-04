using UnityEngine;

public class BoatParticles : MonoBehaviour
{
    [SerializeField] Rigidbody2D boatRigidbody;
    private ParticleSystem[] particleSystems;

    private void Start()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }


    private void Update()
    {
        float boatVelocity = boatRigidbody.linearVelocity.magnitude;

        foreach (ParticleSystem particleSystem in particleSystems)
        {
            var mainModule = particleSystem.main; // Store the main module in a variable
            mainModule.startSpeed = boatVelocity * 1; // Modify the startSpeed property

            var emissionModule = particleSystem.emission; // Store the emission module in a variable
            emissionModule.rateOverTime = boatVelocity * 25; // Modify the rateOverTime property
        }
    }
}
