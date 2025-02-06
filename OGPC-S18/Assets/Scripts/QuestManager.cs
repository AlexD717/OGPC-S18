using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEditor.Experimental.GraphView;

public class QuestManager : MonoBehaviour
{
    public enum QuestType
    {
        Story,
        Random,
        Faction
    }
    public enum QuestStatus
    {
        notStarted,
        active,
        completed,
    }

    [SerializeField] private Quest[] quests;
    [SerializeField] private GameObject questButtonPrefab;
    private GameObject dockedPort;
    private GameObject dockCanvas;

    [Header("Random Quest Generation")]
    public int[] difficultyIdealDistance = new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
    [SerializeField] private float goodEnoughThershold;
    [SerializeField] private float rewardConst;
    private List<GameObject> validQuestPortList = new List<GameObject>();

    public void PlayerDocked(Port port, GameObject _dockCanvas)
    {
        dockedPort = port.gameObject;
        dockCanvas = _dockCanvas;
    }

    public void AddQuestsToMenu(Transform questMenu)
    {
        // Add quests to the Quests Menu
        // Up to 4 quests can be added, one must be reserved for story quests
        Transform questLocation = questMenu.GetChild(2);
        
        bool questsAlreadyAdded = false;
        if (questLocation.childCount > 0) { questsAlreadyAdded = true; }

        // Add story quests

        if (questsAlreadyAdded)
        {
            return;
        }

        // Add faction quests

        // Add random quests
        validQuestPortList = GameObject.FindGameObjectsWithTag("Port").ToList();
        validQuestPortList.Remove(dockedPort);
        for (int i = 0; i < Mathf.Clamp(4 - questLocation.childCount, 0, 3); i++)
        {
            Quest randomQuest = GenerateRandomQuest();

            GameObject questButton = Instantiate(questButtonPrefab, questLocation);
            questButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = randomQuest.questName;
            questButton.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "Reward: " + randomQuest.reward.ToString("F0");
            questButton.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = "Difficulty: " + randomQuest.difficulty.ToString("F0");

            questButton.GetComponent<Button>().onClick.AddListener(() => RandomQuestSelected(randomQuest)); // When button clicked run this function
        }
    }

    private void RandomQuestSelected(Quest selectedQuest)
    {
        // Show menu to confirm quest selection
        // Displays more detailed information about the quest

        Transform selectedQuestPanel = dockCanvas.transform.GetChild(2);
        selectedQuestPanel.gameObject.SetActive(true);

        FillChildText(selectedQuestPanel, 0, selectedQuest.questName);
        FillChildText(selectedQuestPanel, 1, "Quest Type: " + selectedQuest.questType.ToString());
        FillChildText(selectedQuestPanel, 2, "Reward: " + selectedQuest.reward.ToString("F0"));
        FillChildText(selectedQuestPanel, 3, "Difficulty: " + selectedQuest.difficulty.ToString("F0"));
    }

    private void FillChildText(Transform menu, int childIndex, string text)
    {
        menu.GetChild(childIndex).GetComponent<TMPro.TextMeshProUGUI>().text = text;
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
        Quest randomQuest = ScriptableObject.CreateInstance<Quest>();

        // Assigns random values to the quest
        randomQuest.questType = QuestType.Random;
        randomQuest.questStatus = QuestStatus.notStarted;
        
        int difficulty = Random.Range(1, difficultyIdealDistance.Length);
        randomQuest.difficulty = difficulty;
        randomQuest.reward = Mathf.Round(Random.Range(0.8f, 1.2f) * difficulty * rewardConst);

        GameObject goalPort = GetGoalPort(difficultyIdealDistance[difficulty - 1]);
        validQuestPortList.Remove(goalPort);
        randomQuest.goalPort = goalPort.GetComponent<Port>();

        randomQuest.questName = "Diliver to " + randomQuest.goalPort.nameText.text;

        return randomQuest;
    }

    private GameObject GetGoalPort(float idealDistance)
    {
        float closestDistance = Mathf.Infinity;
        GameObject idealPort = null;
        List<GameObject> validPorts= new List<GameObject>();

        foreach (GameObject port in validQuestPortList)
        {
            if (port != dockedPort)
            {
                float distanceToPort = Vector2.Distance(dockedPort.transform.position, port.transform.position);
                float distanceToIdeal = Mathf.Abs(distanceToPort -  idealDistance);
                if (distanceToIdeal <= goodEnoughThershold)
                {
                    validPorts.Add(port);
                }
                if (distanceToIdeal < closestDistance)
                {
                    closestDistance = distanceToIdeal;
                    idealPort = port;
                }
            }
        }

        if (validPorts.Count == 0)
        {
            validPorts.Add(idealPort);
        }

        return validPorts[Random.Range(0, validPorts.Count - 1)];
    }
}