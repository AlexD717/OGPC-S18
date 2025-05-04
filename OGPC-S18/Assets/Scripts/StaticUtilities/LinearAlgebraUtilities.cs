using UnityEngine;

public static class LinearAlgebraUtilities
{
    public static Vector2 RotateVector(Vector2 oldVector, float angle)
    {
        float[] polarVector;
        float newMagnitude;
        float newAngle;
        Vector2 newVector;


        polarVector = VectorUtilities.VectorToPolar(oldVector);

        newMagnitude = polarVector[1];
        newAngle = polarVector[0] + angle;
        newVector = VectorUtilities.PolarToVector(newAngle, newMagnitude);

        return newVector;
    }

    public static Vector2 RotateVectorAroundPoint(Vector2 oldVector, float angle, Vector2 pivotVector)
    {
        float[] polarVector;
        float newMagnitude;
        float newAngle;
        Vector2 relativeVector;
        Vector2 newVector;

        relativeVector = oldVector - pivotVector;

        polarVector = VectorUtilities.VectorToPolar(relativeVector);

        newMagnitude = polarVector[1];
        newAngle = polarVector[0] + angle;

        newVector = VectorUtilities.PolarToVector(newAngle, newMagnitude) + pivotVector;

        return newVector;
    }
}