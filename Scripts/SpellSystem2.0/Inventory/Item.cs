using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "MockItem", menuName = "RPGToolkit/Items/MockItem", order = 1)]
public class Item : RPGObject
{
    [SerializeField] protected Ability casts;
    [SerializeField] private ushort maxStack;
    [SerializeField] private ItemRarity rarity;
    public bool isMock;

    public override void Cast (Character caster, Vector3 origin)
    {
        casts?.Cast(caster, origin);
    }
    public override void Swap(RPGObject with, Character owner = null, ushort ind = 0)
    {
        //if (with != null && with.GetMyType() == "Ability") return;
        if (with.GetMyType() == "Item")
        {
            int temp = with.index;
            if (owner != null)
            {
                owner.getInventory().getSlots()[index].setItem((Item)with);
                owner.getInventory().getSlots()[ temp].setItem(this);
                //with.index = index;
            }
            index = temp;
        }else
        {
            // Dropping on ability
            if (casts != null)
            {
                casts.setBeforeAction(()=> { 
                    if (owner != null)
                    {
                        Debug.Log(this + " " + ind);
                        owner.getInventory().getSlots()[ind].removeItemsFromStack(1);
                    }
                });
                casts.index = with.index;
                ((Ability)with).Caster.Abilities[with.index] = casts;
            }
        }
    }
    public override string GetMyType()
    {
        return "Item";
    }
    public override string getDescription()
    {
        string text = objectName + "\n";
        text += " -- \n";
        text += description;
        return text;
    }
    public virtual void RightClickAction(Character caster, ushort position)
    {

    }
    public virtual void LeftClickAction(Character caster, ushort position)
    {
        // Show item in a window
    }
    public bool hasAbility() { return casts != null; }
    public ushort getMaxStack() { return maxStack; }
    public ItemRarity getRarity() { return rarity; }
}
