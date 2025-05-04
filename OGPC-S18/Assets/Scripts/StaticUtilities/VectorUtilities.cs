using UnityEngine;

public class VectorUtilities
{
    public static Vector2 PolarToVector(float angle, float magnitude)
    {
        float xValue = magnitude * Mathf.Cos(angle * Mathf.Deg2Rad);
        float yValue = magnitude * Mathf.Sin(angle * Mathf.Deg2Rad);
        return new Vector2(xValue, yValue);
    }

    public static float[] VectorToPolar(Vector2 vector)
    {
        float[] result = new float[2];
        result[0] = (360 + Vector2.SignedAngle(Vector2.right, vector)) % 360;
        result[1] = vector.magnitude;
        return result;
    }

    public static float BearingToAngle(float bearing)
    {
        return -bearing + 90f;
    }

    public static float AngleToBearing(float angle)
    {
        angle %= 360;
        if (angle <= 90) return -(angle - 90);
        else return 360 - (angle - 90);
    }
}
