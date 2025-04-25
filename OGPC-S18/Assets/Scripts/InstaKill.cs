using UnityEngine;

public class InstaKill : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager levelManager = FindFirstObjectByType<LevelManager>();
            levelManager.PlayerLost();
        }
    }
}
