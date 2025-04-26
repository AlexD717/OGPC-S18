using System.Collections.Generic;
using UnityEngine;

public class BoatHealth : MonoBehaviour
{
    [SerializeField] private float maxShipHealth;
    private float shipHealth;
    [SerializeField] private float baseDamageFromCollisions;
    [SerializeField] private float speedDamageMult;
    private BoatController boatController;
    private LevelManager levelManager;

    [SerializeField] private List<string> collisionTagsToTakeDamage;

    private void Start()
    {
        boatController = GetComponent<BoatController>();
        levelManager = FindFirstObjectByType<LevelManager>();
        shipHealth = maxShipHealth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collisionTagsToTakeDamage.Contains(collision.gameObject.tag) || collisionTagsToTakeDamage.Contains(collision.transform.parent.gameObject.tag))
        {
            // Boat is colliding with an object that should deal damage
            Debug.Log($"Boat collided with {collision.gameObject.name}");
            TakeDamage(baseDamageFromCollisions + boatController.GetBoatSpeed() * speedDamageMult);
        }
    }

    public void TakeDamage(float damage)
    {
        shipHealth -= damage;
        Debug.Log($"Ship took {damage} damage. Current health: {shipHealth}");

        if (shipHealth <= 0)
        {
            levelManager.PlayerLost();
        }
    }
}
