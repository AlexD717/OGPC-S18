using System;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public enum QuestType
    {
        story,
        random,
        faction
    }

    [SerializeField] private Quest[] quests;

    public void AddQuestsToMenu()
    {
        // Add quests to the Quests Menu
        // Up to 4 quests can be added, one must be reserved for story quests

    }
}
