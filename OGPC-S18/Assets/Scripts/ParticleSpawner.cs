using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    [Header("Wind Particles")]
    [SerializeField] private GameObject windParticlePrefab;
    [SerializeField] private float windSpawnInterval;
    private float windNextSpawnTime = 0f;

    [Header("Water Particles")]
    [SerializeField] private GameObject waterParticlePrefab;
    [SerializeField] private float waterSpawnInterval;
    private float waterNextSpawnTime = 0f;

    [Header("Spawn Area")]
    [SerializeField] private Vector2 xSpawn;
    [SerializeField] private Vector2 ySpawn;

    [Header("References")]
    [SerializeField] private WindManager windManager;
    [SerializeField] private CurrentManager currentManager;

    private GameObject particlesParent;
    private Transform windParticlesParent;
    private Transform waterParticlesParent;

    private void Start()
    {
        particlesParent = new GameObject("ParticlesParent");
        windParticlesParent = new GameObject("WindParticlesParent").transform;
        waterParticlesParent = new GameObject("WaterParticlesParent").transform;

        windParticlesParent.parent = particlesParent.transform;
        waterParticlesParent.parent = particlesParent.transform;
    }

    private void Update()
    {
        if (Time.time >= windNextSpawnTime)
        {
            SpawnWindParticle();
            windNextSpawnTime = Time.time + (windSpawnInterval / windManager.GetWindSpeed());
        }
        if (Time.time >= waterNextSpawnTime)
        {
            SpawnWaterParticle();
            float currentSpeed = currentManager.GetCurrentSpeed();
            if (currentSpeed < 0.2f)
            {
                currentSpeed = 0.2f;
            }
            waterNextSpawnTime = Time.time + (waterSpawnInterval / currentSpeed);
        }
    }

    private void SpawnWindParticle()
    {
        Vector3 spawnPosition = RandomSpawnPosition();
        GameObject windParticleObject = Instantiate(windParticlePrefab, spawnPosition, Quaternion.identity, windParticlesParent);
        WindParticle windParticle = windParticleObject.GetComponent<WindParticle>();
        windParticle.windAngle = windManager.GetWindRadAngle();
        windParticle.windSpeed = windManager.GetWindSpeed();
    }

    private void SpawnWaterParticle()
    {
        Vector3 spawnPosition = RandomSpawnPosition();
        GameObject waterParticleObject = Instantiate(waterParticlePrefab, spawnPosition, Quaternion.identity, waterParticlesParent);
        WaterParticle waterParticle = waterParticleObject.GetComponent<WaterParticle>();
        waterParticle.currentAngle = currentManager.GetCurrentRadAngle();
        waterParticle.currentSpeed = currentManager.GetCurrentSpeed();
    }

    private Vector3 RandomSpawnPosition()
    {
        float x = Random.Range(xSpawn.x, xSpawn.y);
        float y = Random.Range(ySpawn.x, ySpawn.y);
        return new Vector3(x, y, 0) + transform.position;
    }
}
