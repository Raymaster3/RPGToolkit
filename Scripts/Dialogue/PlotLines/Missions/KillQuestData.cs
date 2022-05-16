using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "RPGToolkit/Quests/KillQuest", order = 1)]
public class KillQuestData : QuestData
{
    [SerializeReference] public List<GenericObjectiveQuant> objectives;

    public override Quest GetRuntimeQuest()
    {
        KillQuest newQuest = new KillQuest();
        newQuest.name = questName;
        newQuest.description = description;
        newQuest.objectives = new List<GenericObjectiveQuant>();
        foreach (GenericObjectiveQuant iq in objectives)
        {
            newQuest.objectives.Add(new GenericObjectiveQuant(iq.objective, iq.totalQuantity));
        }
        return newQuest;
    }
}
