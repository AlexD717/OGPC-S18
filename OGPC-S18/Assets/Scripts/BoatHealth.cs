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
    private Animator animator;

    [SerializeField] private List<string> collisionTagsToTakeDamage;

    public bool tutorialLevel = false;

    private void Start()
    {
        boatController = GetComponent<BoatController>();
        animator = GetComponent<Animator>();
        levelManager = FindFirstObjectByType<LevelManager>();
        shipHealth = maxShipHealth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collisionTagsToTakeDamage.Contains(collision.gameObject.tag) || collisionTagsToTakeDamage.Contains(collision.transform.parent.gameObject.tag))
        {
            if (tutorialLevel)
            {
                Debug.Log($"Boat collided with {collision.gameObject.name} in tutorial level. Ignoring collision for tutorial purposes.");
                TutorialManager tutorialManager = FindFirstObjectByType<TutorialManager>();
                if (tutorialManager != null)
                {
                    tutorialManager.PlayerDied();
                }
                // Ignore collisions with tutorial objects
                return;
            }

            // Boat is colliding with an object that should deal damage
            Debug.Log($"Boat collided with {collision.gameObject.name}");
            TakeDamage(baseDamageFromCollisions + boatController.GetBoatSpeed() * speedDamageMult);
        }
    }

    public void TakeDamage(float damage)
    {
        shipHealth -= damage;
        Debug.Log($"Ship took {damage} damage. Current health: {shipHealth}");
        
        animator.SetTrigger("TookDamage");

        if (shipHealth <= 0)
        {
            if (tutorialLevel)
            {
                TutorialManager tutorialManager = FindFirstObjectByType<TutorialManager>();
                tutorialManager.PlayerDied();
            }
            else
            {
                levelManager.PlayerLost();
            }
        }
    }

    public void ResetHealth()
    {
        shipHealth = maxShipHealth;
        Debug.Log("Ship health reset to max.");
    }
}
