using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]
public class Quest : ScriptableObject
{
    public QuestManager.QuestType questType;

    public string[] viablePorts; // Which ports can this quest be found in
}
