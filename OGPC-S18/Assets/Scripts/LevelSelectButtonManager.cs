using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButtonManager : MonoBehaviour
{
    private void Start()
    {
        int level = 0;
        foreach (Transform button in transform)
        {
            level++;
            int levelNum = level; // So that when the level variable updates the levelNum varible will be the same
            button.GetComponent<Button>().onClick.AddListener(() => LoadLevel(levelNum));
            button.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Level " + level.ToString();
        }
    }

    private void LoadLevel(int levelNum)
    {
        Loader.LoadByName("Level"+levelNum.ToString());
    }
}
