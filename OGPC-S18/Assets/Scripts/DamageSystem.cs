using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    private int currentHealth;
    [SerializeField] private int collisionDamage;
    [SerializeField] private float collisionCooldown;
    private float collisionCooldownTimer = 0;

    private LevelManager levelManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        levelManager = FindFirstObjectByType<LevelManager>();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (collisionCooldownTimer <= 0)
        {
            if (other.gameObject.CompareTag("Island"))
            {
                Debug.Log("Collided with island!");
                DamageShip(collisionDamage);
                collisionCooldownTimer = collisionCooldown;
            }
        }
    }

    private void DamageShip(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            levelManager.PlayerLost();
        }
    }

    // Update is called once per frame
    void Update()
    {
        collisionCooldownTimer -= Time.deltaTime;
        if (collisionCooldownTimer < 0)
        {
            collisionCooldownTimer = 0;
        }
    }
}
