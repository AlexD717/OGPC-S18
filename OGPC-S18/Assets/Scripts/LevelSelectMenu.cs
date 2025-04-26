using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class LevelSelectMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    private UIManager uiManager;

    private int numLevels;
    private int currentSelectedLevel = 0;

    private void Start()
    {
        uiManager = GetComponent<UIManager>();

        int maxLevelPassed = PlayerPrefsManager.GetMaxLevelPast();
        numLevels = menu.transform.childCount;
        for (int i = 0; i < numLevels; i++)
        {
            int levelNumber = i + 1;

            GameObject levelObject = menu.transform.GetChild(i).gameObject;

            // Add Listner to Load Level Button
            levelObject.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => LoadLevel(levelNumber.ToString()));
            // Add Listner to Level Image
            levelObject.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => LoadLevel(levelNumber.ToString()));

            // Change highscore text
            levelObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"Highscore: {PlayerPrefsManager.GetLevelHighscore(i + 1).ToString("F0")}";
        }

        currentSelectedLevel = maxLevelPassed;
        currentSelectedLevel = Mathf.Clamp(currentSelectedLevel, 0, numLevels - 1);
        SelectLevel(currentSelectedLevel);
        
    }

    private void LoadLevel(string levelNum)
    {
        uiManager.LoadLevelByName("Level" + levelNum);
    }

    public void NextLevel()
    {
        currentSelectedLevel++;
        currentSelectedLevel = currentSelectedLevel % numLevels;
        SelectLevel(currentSelectedLevel);
    }

    public void PreviousLevel()
    {
        if (currentSelectedLevel > 0)
        {
            currentSelectedLevel--;
        }
        else
        {
            currentSelectedLevel = numLevels - 1;
        }
        SelectLevel(currentSelectedLevel);
    }

    private void SelectLevel(int levelIndex)
    {
        for (int i = 0; i < numLevels; i++)
        {
            if (levelIndex == i)
            {
                menu.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                menu.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}