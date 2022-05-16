using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemQuest : Quest
{
    [SerializeField] public List<ItemObjectiveQuant> itemObjectives = new List<ItemObjectiveQuant>();
    private ObjectiveQuant<Item> currentObjective;

    public ItemQuest() { }
    public ItemQuest(Item objective, int objquantity)
    {
        itemObjectives.Add(new ItemObjectiveQuant(objective, objquantity));
        currentObjective = itemObjectives[0];
    }

    public override void UpdateObjectives(object objective, int quant = 1)
    {
        Item obj = objective as Item;
        if (obj == null) return;
        ObjectiveQuant<Item> target = itemObjectives.Find(x => x.objective == obj);
        if (target == null) return;
        currentObjective = target;
        if (target.addToCurrent(quant))
        {
            // Finnish quest
            Complete();
        }
    }
    public override int[] getObjectiveCounter()
    {
        if (currentObjective == null) currentObjective = itemObjectives[0];
        int[] res = new int[2];
        res[0] = currentObjective.currentQuantity;
        res[1] = currentObjective.totalQuantity;
        if (res[0] > res[1]) res[0] = res[1];
        return res;
    }
    public override void CheckObjectives()
    {
        Inventory inventory = Player.instance.getInventory();
        foreach(Slot slot in inventory.getSlots())
        {
            if (slot.getItem() == itemObjectives[0].objective)
            {
                UpdateObjectives(slot.getItem(), slot.getQuantity());
            }
        }
    }
    public override object[] GetObjectives()
    {
        return base.GetObjectives();
    }
}
