using UnityEngine;

public class WindowsOnly : MonoBehaviour
{
    void Start()
    {
#if UNITY_STANDALONE_WIN
        //Debug.Log("Running on Windows");
        gameObject.SetActive(true); // Enable the GameObject on Windows
#elif UNITY_WEBGL
        //Debug.Log("Running in WebGL");
        gameObject.SetActive(false); // Disable the GameObject in WebGL
#else
        //Debug.Log("Running on another platform");
#endif
    }
}
