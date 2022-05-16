using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "RPGToolkit/Quests/Quest", order = 1)]
public class QuestData : ScriptableObject
{
    public string questName;
    public string description;

    public virtual Quest GetRuntimeQuest()
    {
        return new Quest();
    }
}
