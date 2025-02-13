using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuestActiveManager : MonoBehaviour
{
    [SerializeField] private int maxNumberOfQuests = 4;
    private int numActiveQuests = 0;
    private List<Quest> activeQuestsList = new List<Quest>();

    [SerializeField] private GameObject activeQuestPanel;
    [SerializeField] private InputActionAsset inputActions;
    private InputAction questMenuToggle;

    private void OnEnable()
    {
        var playerControls = inputActions.FindActionMap("Player");
        questMenuToggle = playerControls.FindAction("QuestMenuToggle");

        questMenuToggle.Enable();
    }

    private void OnDisable()
    {
        questMenuToggle.Disable();
    }

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

    private void Update()
    {
        if (questMenuToggle.triggered)
        {
            if (activeQuestPanel.activeSelf)
            {
                HideQuestPanel();
            }
            else
            {
                ShowQuestPanel();
            }
        }
    }

    private void ShowQuestPanel()
    {
        activeQuestPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void HideQuestPanel()
    {
        activeQuestPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}