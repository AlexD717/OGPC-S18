using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private int numberOfLevels;

    private void OnEnable()
    {
        numberOfLevels = SceneManager.sceneCountInBuildSettings - 4; // Exclude the main menu, tutorial, level select, and loading screen scenes
    }
    public void LoadLevelByName(string name)
    {
        Loader.LoadByName(name);
    }

    public void LoadLevelByIndex(int index)
    {
        Loader.LoadByIndex(index);
    }

    public void NextSene()
    {
        if (SceneManager.GetActiveScene().buildIndex + 1 >= numberOfLevels)
        {
            Loader.LoadByName("LevelSelect");
            return;
        }
        else
        {
            Loader.LoadByIndex(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void RestartCurrentScene()
    {
        Loader.LoadByName(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}