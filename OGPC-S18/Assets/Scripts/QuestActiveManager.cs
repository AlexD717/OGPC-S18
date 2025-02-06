using System.Collections.Generic;
using UnityEngine;

public class QuestActiveManager : MonoBehaviour
{
    [SerializeField] private int maxNumberOfQuests = 4;
    private int numActiveQuests = 0;
    private List<Quest> activeQuestsList = new List<Quest>();

    public bool canAcceptQuest()
    {
        if (numActiveQuests >= maxNumberOfQuests)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void questAccepted(Quest questSelected)
    {
        // Fail safe
        if (!canAcceptQuest())
            Debug.LogError("Can't accept quest, max number reached");

        numActiveQuests++;
        activeQuestsList.Add(questSelected);
    }
}
