using UnityEngine;

public class GradientTexture : MonoBehaviour
{
    [SerializeField] private Gradient gradient;
    [SerializeField] private int textureWidth;

    void Start()
    {
        Texture2D gradientTexture = new Texture2D(textureWidth, 1);

        for (int i = 0; i < textureWidth; i++)
        {
            float time = (float)i / (textureWidth - 1);
            gradientTexture.SetPixel(i, 0, gradient.Evaluate(time));
        }

        gradientTexture.Apply();

        GetComponent<SpriteRenderer>().material.mainTexture = gradientTexture;
    }
}
