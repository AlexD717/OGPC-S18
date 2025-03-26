using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
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
        Loader.LoadByIndex(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RestartCurrentScene()
    {
        Loader.LoadByName(SceneManager.GetActiveScene().name);
    }
}
