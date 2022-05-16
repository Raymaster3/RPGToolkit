using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Item Quest", menuName = "RPGToolkit/Quests/Item Quest", order = 1)]
public class ItemQuestData : QuestData
{
    [SerializeField] public List<ItemObjectiveQuant> itemObjectives = new List<ItemObjectiveQuant>();

    public override Quest GetRuntimeQuest()
    {
        ItemQuest newQuest = new ItemQuest();
        newQuest.name = questName;
        newQuest.description = description;
        newQuest.itemObjectives = new List<ItemObjectiveQuant>();
        foreach (ItemObjectiveQuant iq in itemObjectives)
        {
            newQuest.itemObjectives.Add(new ItemObjectiveQuant(iq.objective, iq.totalQuantity));
        }
        return newQuest;
    }
}
