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

    public static Vector2 Polar2Vector(float ang, float mag) //Turns polar coords into a vector
    {
        return new Vector2(mag*Mathf.Sin(ang*Mathf.Deg2Rad),mag*Mathf.Cos(ang*Mathf.Deg2Rad));
    }
}
