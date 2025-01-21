using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public enum QuestType
    {
        story,
        random,
        faction
    }
    public enum QuestStatus
    {
        notStarted,
        active,
        completed,
    }

    [SerializeField] private Quest[] quests;
    private Quest[] randomQuests;
    [SerializeField] private GameObject questButtonPrefab;

    public void AddQuestsToMenu(Transform questMenu, string portName)
    {
        // Add quests to the Quests Menu
        // Up to 4 quests can be added, one must be reserved for story quests
        Transform questLocation = questMenu.GetChild(2);


        // Add random quests
        for (int i = 0; i < Mathf.Clamp(4 - questLocation.childCount, 0, 3); i++)
        {
            Quest[] randomViableQuest = GetViableQuests(portName);
            foreach (Quest viableQuest in randomViableQuest)
            {
                Debug.Log("Viable Quest Found: " + viableQuest.questName.ToString());
            }
        }
    }

    private Quest[] GetViableQuests(string portName)
    {
        List<Quest> viableQuestList = new List<Quest>();
        foreach (Quest quest in quests)
        {
            bool viableQuest = false;
            if (quest.questType == QuestType.random)
            {
                foreach (string validLocation in quest.viablePorts)
                {
                    if (validLocation == portName)
                    {
                        viableQuest = true;
                    }
                }
            }

            if (viableQuest)
            {
                viableQuestList.Add(quest);
            }
        }

        return viableQuestList.ToArray();
    }
}
