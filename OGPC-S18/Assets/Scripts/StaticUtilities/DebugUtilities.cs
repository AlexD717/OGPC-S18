using UnityEngine;

public class DebugUtilities
{
    public static class DebugUtils
    {
        public static class Vectors
        {
            public static void LogVector2(string header, Vector2 vector)
            {
                Debug.Log($"{header} {vector.x} {vector.y}");
            }

            public static void LogVector2(Vector2 vector)
            {
                Debug.Log($"{vector.x} {vector.y}");
            }

            public static void LogVector3(string header, Vector3 vector)
            {
                Debug.Log($"{header} {vector.x} {vector.y} {vector.z}");
            }

            public static void LogVector3(Vector3 vector)
            {
                Debug.Log($"{vector.x} {vector.y} {vector.z}");
            }
        }

        public static class LogArrays
        {
            public static void Vector2(Vector2[] vectors, string header = "")
            {
                string result = header + " ";
                foreach (Vector2 vector in vectors)
                {
                    result += $"({vector.x}, {vector.y}) ";
                }
                Debug.Log(result);
            }

            public static void Bool(bool[] booleans, string header = null)
            {
                string result = (header == null) ? "" : header + " ";
                foreach (bool boolean in booleans)
                {
                    result += boolean + " ";
                }
                Debug.Log(result);
            }
        }
    }
}