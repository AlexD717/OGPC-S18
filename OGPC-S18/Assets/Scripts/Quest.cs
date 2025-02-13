using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]
public class Quest : ScriptableObject
{
    public string questName;

    public QuestGetttingManager.QuestType questType;
    [HideInInspector] public QuestGetttingManager.QuestStatus questStatus = QuestGetttingManager.QuestStatus.notStarted;

    public string[] viablePorts; // Which ports can this quest be found in

    public float reward;
    public int difficulty;
    public Port goalPort;
}