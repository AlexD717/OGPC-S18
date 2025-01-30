using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]
public class Quest : ScriptableObject
{
    public string questName;

    public QuestManager.QuestType questType;
    [HideInInspector] public QuestManager.QuestStatus questStatus = QuestManager.QuestStatus.notStarted;

    public string[] viablePorts; // Which ports can this quest be found in

    public float reward;
    public int difficulty;
    public Port goalPort;
}