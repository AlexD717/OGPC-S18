using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] private GameObject followTarget;
    public bool keepZPosition;
    public bool lateUpdate;

    private void Update()
    {
        if (!lateUpdate)
        {
            UpdatePosition();
        }
    }

    private void LateUpdate()
    {
        if (lateUpdate)
        {
            UpdatePosition();
        }   
    }

    private void UpdatePosition()
    {
        if (followTarget != null)
        {
            Vector3 targetPosition = followTarget.transform.position;
            if (keepZPosition)
            {
                targetPosition.z = transform.position.z; // Keep the z position of the current object
            }
            transform.position = targetPosition;
        }
    }
}
