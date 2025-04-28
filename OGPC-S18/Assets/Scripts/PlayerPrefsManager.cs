using UnityEngine;

public static class PlayerPrefsManager
{
    public static void LevelCompleted(int level, float highscore)
    {
        Debug.Log("Level Completed: " + level + " with highscore: " + highscore);
        // Saves max level past
        int maxLevelPassed = GetMaxLevelPast();
        if (level > maxLevelPassed)
        {
            PlayerPrefs.SetInt("MaxLevelPassed", level);
        }

        // Saves highest score
        float maxHighscore = GetLevelHighscore(level);
        if (highscore > maxHighscore)
        {
            PlayerPrefs.SetFloat("Level" + level.ToString() + "Highscore", highscore);
        }
    }

    public static int GetMaxLevelPast()
    {
        return PlayerPrefs.GetInt("MaxLevelPassed", 0);
    }

    public static float GetLevelHighscore(int level)
    {
        return PlayerPrefs.GetFloat("Level" + level.ToString() + "Highscore", 0f);
    }
}
