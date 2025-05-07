using System;
using UnityEngine.SceneManagement;

public static class Loader
{
    private static Action onLoaderCallback;

    public static void LoadByName(string scene)
    {
        SceneManager.LoadScene("LoadingScreen");

        onLoaderCallback = () =>
        {
            SceneManager.LoadScene(scene);
        };
    }

    public static void LoadByIndex(int index)
    {
        SceneManager.LoadScene("LoadingScreen");

        onLoaderCallback = () =>
        {
            SceneManager.LoadScene(index);
        };
    }

    public static void RestartLevel()
    {
        // Reloads the current scene
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    public static void LoaderCallback()
    {
        if (onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }
}