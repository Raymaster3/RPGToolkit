using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public string name;
    public string description;

    [HideInInspector] public PlotStep questGiver;

    public virtual void UpdateObjectives(object objective, int quant = 1) { }
    public virtual object[] GetObjectives() { return null; }
    public virtual int[] getObjectiveCounter() { return new int[0]; }
    public virtual void CheckObjectives()
    {

    }

    protected void Complete()
    {
        questGiver.questFinnished = true;
        QuestsManager.instance.QuestFinnished(this);
    }
}
public class ObjectiveQuant<TObj>
{
    [SerializeField] public int currentQuantity = 0;
    [SerializeField] public int totalQuantity;
    [SerializeField] public TObj objective;

    public ObjectiveQuant() { }
    public ObjectiveQuant(TObj obj, int maxQuant)
    {
        objective = obj;
        totalQuantity = maxQuant;
    }

    public bool addToCurrent(int quantity)
    {
        currentQuantity += quantity;
        if (currentQuantity >= totalQuantity)
        {
            // Finnish mission
            return true;
        }
        return false;
    }
}
[System.Serializable]
public class ItemObjectiveQuant : ObjectiveQuant<Item>
{
    public ItemObjectiveQuant(Item obj, int maxQuant) : base(obj, maxQuant)
    {
        objective = obj;
        totalQuantity = maxQuant;
    }
}
[System.Serializable]
public class GenericObjectiveQuant : ObjectiveQuant<Object>
{
    public GenericObjectiveQuant(object obj, int maxQuant) : base(obj as Object, maxQuant)
    {
        objective = obj as Object;
        totalQuantity = maxQuant;
    }
}
