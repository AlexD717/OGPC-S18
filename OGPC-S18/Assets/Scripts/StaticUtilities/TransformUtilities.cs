using UnityEngine;

public static class TransformUtilities
{
    public static Transform GetClosestPosition(Transform[] positions, GameObject startPoint)
    {
        float smallestDistance = Mathf.Infinity;
        Transform closestPos = null;

        foreach (Transform position in positions)
        {
            float distanceBetween = Vector2.Distance(position.position, startPoint.transform.position);
            if (distanceBetween < smallestDistance)
            {
                smallestDistance = distanceBetween;
                closestPos = position;
            }
        }

        return closestPos;
    }
}
