using UnityEngine;

public class StormWall : MonoBehaviour
{
    [Header("StormWall Configurables")]
    [SerializeField] Vector2 startPosition;
    [SerializeField] float startRotation;
    [SerializeField] Vector2 startVelocity;
    [SerializeField] Vector2 startAcceleration;
    [SerializeField] bool stormWallFacesDirectionOfTravel = true;
    [SerializeField] float damagePerSecond = 25f; // Damage per second to the player
    private Rigidbody2D stormWallRigidBody;

    private GameObject stormWall;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stormWall = transform.gameObject;
        stormWallRigidBody = stormWall.GetComponent<Rigidbody2D>();
        stormWall.transform.position = startPosition;
        stormWall.transform.rotation = Quaternion.Euler(0, 0, startRotation);
        stormWallRigidBody.linearVelocity = startVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        stormWallRigidBody.linearVelocity += startAcceleration * Time.deltaTime;
        if (stormWallFacesDirectionOfTravel)
        {
            stormWall.transform.rotation = Quaternion.Euler(0, 0, VectorUtilities.VectorToPolar(stormWallRigidBody.linearVelocity)[0] + 90f);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<BoatHealth>().TakeDamage(Time.deltaTime * damagePerSecond);
        }
        else if (other.CompareTag("EndPort"))
        {
            LevelManager levelManager = FindFirstObjectByType<LevelManager>();
            levelManager.PlayerLost();
        }
    }
}
