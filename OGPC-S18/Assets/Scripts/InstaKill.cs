using UnityEngine;

public class InstaKill : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<BoatHealth>().TakeDamage(Mathf.Infinity);
        }
    }
}
