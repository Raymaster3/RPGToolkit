using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillQuest : Quest
{
    public List<GenericObjectiveQuant> objectives = new List<GenericObjectiveQuant>();
    private GenericObjectiveQuant curObjective;

    public override void CheckObjectives()
    {
        Inventory inventory = Player.instance.getInventory();
        foreach (Slot slot in inventory.getSlots())
        {
            UpdateObjectives(slot.getItem(), slot.getQuantity());
        }
    }
    public override int[] getObjectiveCounter()
    {
        if (curObjective == null) curObjective = objectives[0];
        int[] res = new int[2];
        res[0] = curObjective.currentQuantity;
        res[1] = curObjective.totalQuantity;
        if (res[0] > res[1]) res[0] = res[1];
        return res;
    }
    public override void UpdateObjectives(object objective, int quant = 1)
    {
        Item obj = objective as Item;
        Character charObj = objective as Character;

        GenericObjectiveQuant target = null;

        if (obj != null)
            target = objectives.Find(x => (object) x.objective == objective);
        else if (charObj != null)
        {
            target = objectives.Find(x => {
                CharacterData data = x.objective as CharacterData;
                if (data == null) return false;
                return data.getCharacterName() == charObj.getCharName();
            });
        }

        if (target == null) return;
        curObjective = target;
        if (target.addToCurrent(quant))
        {
            // Finnish quest
            Complete();
        }



    }
}
