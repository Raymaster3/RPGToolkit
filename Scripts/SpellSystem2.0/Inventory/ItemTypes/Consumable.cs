using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Consumable", menuName = "RPGToolkit/Items/Consumable", order = 1)]
public class Consumable : Item
{

    public override void RightClickAction(Character caster, ushort position)
    {
        caster.getInventory().getSlots()[position].removeItemsFromStack(1);
        casts.Cast(caster, SpellsManager.getMouseWorldPos());
    }
}
