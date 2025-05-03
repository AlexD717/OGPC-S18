using UnityEngine;

public class TutorialTarget : MonoBehaviour
{
    public Vector2 target;
    private GameObject arrow;

    private void Start()
    {
        arrow = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (target == Vector2.zero)
        {
            arrow.SetActive(false);
        }
        else
        {
            arrow.SetActive(true);
            Vector2 direction = target - new Vector2(transform.position.x, transform.position.y);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, (angle - 90) % 360);
        }
    }
}
