using UnityEngine;

public static class AngleUtilities
{
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
