using UnityEngine;

public static class MathUtilities
{
    public static float Round(float num, int places)
    {
        int tempNum;
        num = num * Mathf.Pow(10, places); //shifting preserved values to left side of decimal point
        tempNum = (int)num; //truncating unwanted values through int conversion
        num = (float)tempNum; //converting back into float
        num = num / Mathf.Pow(10, places); // undoing previous multiplication factor to get correctly scaled output
        return num;
    }

    public static float ClampAngle(float angle, float clamp1, float clamp2)
    {
        if (clamp1 > clamp2)
        {
            if (angle >= clamp1 || angle <= clamp2) return angle;
            else if (angle > (clamp2 + clamp1) / 2) return clamp1;
            else return clamp2;
        }
        else if (clamp1 < clamp2)
        {
            if (angle >= clamp1 && angle <= clamp2) return angle;
            else if (angle > (clamp1 + 360 - clamp2) / 2 + clamp2) return clamp1;
            else return clamp2;
        }
        else return clamp1;
    }
}
