using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [SerializeField] private Slot[] slots;
    [SerializeField] private int gold;
    Character owner;

    public Inventory (Inventory inv, Character owner)
    {
        this.owner = owner;
        gold = inv.gold;
        // Make a copy
        slots = new Slot[inv.slots.Length];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = new Slot(inv.slots[i].Id);
            //slots[i].setItem(inv.slots[i].getItem() == null ? InventoryManager.instance.mockItem : inv.slots[i].getItem());
            slots[i].setItem(inv.slots[i].getItem());
            slots[i].setQuantity(inv.slots[i].getQuantity());
        }
        gold = 0;
    }

    public Slot[] getSlots() { return slots; }
    public void swapItems(ushort firstPos, ushort secPos)
    {
        slots[firstPos].swapWith(slots[secPos]);
    }
    public void removeItem(Item i, ushort quantity = 1)
    {
        foreach (Slot s in slots)
        {
            if (s.getItem() == null) continue;
            if (s.getItem() != i) continue;
            s.removeItemsFromStack(quantity);
            return;
        }
    }
    public int addItem(Item i, out bool somePlaced, ushort quantity = 1)
    {
        ushort localQuantity = quantity;
        somePlaced = false;
        if (quantity == 0) return 0;
        
        foreach (Slot s in slots)
        {
            if (s.getItem() != null && s.getItem() == i)    // If we already have the item in the inventory
            {
                if (s.hasSpaceInStack(localQuantity))   // If we can fit the whole quantity in the stack
                {
                    s.addItemsToStack(localQuantity, this);
                    Player.instance.updateQuests(i, localQuantity);
                    localQuantity = 0;
                    somePlaced = true;
                    return 0;
                }
                else                                   // If we can't we add the diference to complete it and then substract from quantity 
                {
                    ushort space = s.getAvailableSpace();
                    s.addItemsToStack(space, this);
                    localQuantity -= space;
                    somePlaced = space > 0;
                }
            }
            else if (s.getItem() == null)
            {
                s.setItem(i);
                //s.setQuantity(localQuantity);
                s.addItemsToStack(localQuantity, this);
                Player.instance.updateQuests(i, localQuantity);
                somePlaced = true;
                return 0;
            }
        }
        UIManager.instance.ShowMessage("No space", Color.red, ErrorDuration.Short);

        return localQuantity;
        
    }
    public bool canAddItem(Item i, ushort quant = 1)
    {
        foreach(Slot s in slots)
        {
            if ((s.getItem() == i && s.hasSpaceInStack(quant)) || s.getItem() == null) return true;
        }
        return false;
    }
    public int addItem(Item i, ushort quantity = 1)
    {
        bool ind;
        return addItem(i, out ind, quantity);
    }
    public void addGold(int amount) { gold += amount; }
    public bool spendGold(int amount) {
        bool res = gold - amount > 0;
        if (res)
            gold -= amount; 
        return res;
    }
    public int getGold() { return gold; }
}
