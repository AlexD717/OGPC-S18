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
    private List<Quest> randomQuests;
    [SerializeField] private GameObject questButtonPrefab;

    private void Start()
    {
        randomQuests = new List<Quest>();
    }

    public void AddQuestsToMenu(Transform questMenu, string portName)
    {
        // Add quests to the Quests Menu
        // Up to 4 quests can be added, one must be reserved for story quests
        Transform questLocation = questMenu.GetChild(2);

        // Add story quests

        // Add faction quests

        // Add random quests
        for (int i = 0; i < Mathf.Clamp(4 - questLocation.childCount, 0, 3); i++)
        {
            Quest randomViableQuest = GenerateRandomQuest();
            GameObject questButton = Instantiate(questButtonPrefab, questLocation);
            questButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = randomViableQuest.questName;
        }
    }

    private Quest[] GetViableQuests(string portName, QuestType questTypeWanted)
    {
        List<Quest> viableQuestList = new List<Quest>();
        foreach (Quest quest in quests)
        {
            bool viableQuest = false;
            if (quest.questType == questTypeWanted)
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

    private Quest GenerateRandomQuest()
    {
        // Creates a new quest
        Quest randomQuest = new Quest();

        // Assigns random values to the quest
        randomQuest.questName = "Quest Name";
        randomQuest.questType = QuestType.random;
        randomQuest.questStatus = QuestStatus.notStarted;

        return randomQuest;
    }
}
