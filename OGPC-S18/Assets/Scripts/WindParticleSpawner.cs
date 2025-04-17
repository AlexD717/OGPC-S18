using UnityEngine;

public class WindParticleSpawner : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private GameObject windParticlePrefab;
    [SerializeField] private float spawnInterval;
    private float nextSpawnTime = 0f;

    [SerializeField] private Vector2 xSpawn;
    [SerializeField] private Vector2 ySpawn;

    [Header("References")]
    [SerializeField] private WindManager windManager;

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnWindParticle();
            nextSpawnTime = Time.time + (spawnInterval / windManager.GetWindSpeed());
        }
    }

    private void SpawnWindParticle()
    {
        Vector3 spawnPosition = RandomSpawnPosition();
        GameObject windParticleObject = Instantiate(windParticlePrefab, spawnPosition, Quaternion.identity);
        //windParticleObject.transform.SetParent(transform);
        WindParticle windParticle = windParticleObject.GetComponent<WindParticle>();
        windParticle.windAngle = windManager.GetWindRadAngle();
        windParticle.windSpeed = windManager.GetWindSpeed();
    }

    private Vector3 RandomSpawnPosition()
    {
        float x = Random.Range(xSpawn.x, xSpawn.y);
        float y = Random.Range(ySpawn.x, ySpawn.y);
        return new Vector3(x, y, 0) + transform.position;
    }
}
