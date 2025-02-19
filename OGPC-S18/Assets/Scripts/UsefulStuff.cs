using UnityEngine;
using System;
using System.Collections.Generic;

public class UsefulStuff
{
    public class Miscellaneous
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
    }
    public class Convert
    {
        public static Vector2 Vector32Vector2(Vector3 vector) //Truncates the z value
        {
            return new Vector2(vector.x, vector.y);
        }
        public static Vector3 Vector22Vector3(Vector2 vector) //Adds z value of 0
        {
            return new Vector3(vector.x, vector.y, 0f);
        }
        public static Vector2 Polar2Vector(float angle, float magnitude) //Turns polar coords into a vector
        {
            float xValue;
            float yValue;
            Vector2 result;

            xValue = magnitude*Mathf.Cos(angle*Mathf.Deg2Rad);
            yValue = magnitude*Mathf.Sin(angle*Mathf.Deg2Rad);
            result = new Vector2(xValue, yValue);

            return result;
        }
        public static float[] Vector2Polar(Vector2 vector)
        {
            float magnitude;
            float angle;
            float[] result = new float[2];

            magnitude = vector.magnitude;
            angle = Vector2.SignedAngle(Vector2.right, vector);
            angle = (360 + angle)%360; //Makes sure angle is positive
            result[0] = angle;
            result[1] = magnitude;

            return result;
        }
        public static float Bearing2Angle(float bearing) //Converts ship bearing to absolute angle used by unity
        {
            return -bearing + 90f;
        }
        public static float Angle2Bearing(float angle) //Converts absolute angle into ship bearing
        {
            angle %= 360;
            if (angle <= 90) {return -(angle-90);}
            else {return 360 -(angle-90);}
        }
    }
    public class Debug
    {
        public class Vectors
        {
            public static void LogVector2(string header, Vector2 vector)
            {
                UnityEngine.Debug.Log(header + " " + vector.x.ToString() + " " + vector.y.ToString());
            }
            public static void LogVector2(Vector2 vector)//Overloading
            {
                UnityEngine.Debug.Log(vector.x.ToString() + " " + vector.y.ToString());
            }
            public static void LogVector3(string header, Vector3 vector)
            {
                UnityEngine.Debug.Log(header + " " + vector.x.ToString() + " " + vector.y.ToString() + " " + vector.z.ToString());
            }
            public static void LogVector3(Vector3 vector)
            {
                UnityEngine.Debug.Log(vector.x.ToString() + " " + vector.y.ToString() + " " + vector.z.ToString());
            }
        }
        public class LogArrays
        {
            public static void Vector2(Vector2[] vectors, string header = "")
            {
                string result = header + " ";
                foreach (Vector2 vector in vectors)
                {
                    result = result + "(" + vector.x.ToString() + ", " + vector.y.ToString() + ") ";
                }
                UnityEngine.Debug.Log(result);
            }
            public static void Bool(bool[] booleans, string header = null)
            {
                string result = (header == null) ? "" : header + " ";
                foreach (bool boolean in booleans)
                {
                    result = result + boolean.ToString() + " ";
                }
                UnityEngine.Debug.Log(result);
            }
        }
    }
    public class LinearAlgebra
    {
        public static Vector2 RotateVector(Vector2 oldVector, float angle)
        {
            float[] polarVector;
            float newMagnitude;
            float newAngle;
            Vector2 newVector;


            polarVector = UsefulStuff.Convert.Vector2Polar(oldVector);

            newMagnitude = polarVector[1];
            newAngle = polarVector[0] + angle;
            newVector = UsefulStuff.Convert.Polar2Vector(newAngle, newMagnitude);

            return newVector;
        }        public static Vector2 RotateVectorAroundPoint(Vector2 oldVector, float angle, Vector2 pivotVector)
        {
            float[] polarVector;
            float newMagnitude;
            float newAngle;
            Vector2 relativeVector;
            Vector2 newVector;

            relativeVector = oldVector - pivotVector;

            polarVector = UsefulStuff.Convert.Vector2Polar(relativeVector);

            newMagnitude = polarVector[1];
            newAngle = polarVector[0] + angle;

            newVector = UsefulStuff.Convert.Polar2Vector(newAngle, newMagnitude) + pivotVector;

            return newVector;
        }
    }
    public class Game
    {
        public static void GamePaused(bool pause)
        {
            if (pause) { Time.timeScale = 0f;}
            else { Time.timeScale = 1f;}
        }
    }
    public class Timer
    {
        int period;
        int currentStep;

        Timer(int period)
        {
            this.period = period;
        }
        public void Update()
        {
            currentStep++;
            currentStep %= period;
        }
        public bool Execute()
        {
            return currentStep == 0;
        }

    }
}
