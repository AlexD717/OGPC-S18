using UnityEngine;

public class UsefulStuff
{
    public static float Round(float num, int places) //Rounds to n places, eg 0 truncates decimal, 1 is to 1 decimal place, -1 is to tens place
    {
        int tempNum;
        num = num * Mathf.Pow(10, places); //shifting preserved values to left side of decimal point
        tempNum = (int) num; //truncating unwanted values through int conversion
        num = (float) tempNum; //converting back into float
        num = num / Mathf.Pow(10, places); // undoing previous multiplication factor to get correctly scaled output
        return num;
    }

    public static Vector2 Polar2Vector(float angle, float magnitude) //Turns polar coords into a vector
    {
        return new Vector2(magnitude*Mathf.Sin(angle*Mathf.Deg2Rad),magnitude*Mathf.Cos(angle*Mathf.Deg2Rad));
    }

    public static Vector2 RotateVector(Vector2 vector, float angle)
    {
        float newMagnitude;
        float newAngle;

        newMagnitude = vector.magnitude;
        newAngle = Vector2.SignedAngle(vector, Vector2.right) + angle;

        return Polar2Vector(newAngle, newMagnitude);
    }

    public static void GamePaused(bool pause)
    {
        if (pause) { Time.timeScale = 0f;}
        else { Time.timeScale = 1f;}
    }
}
