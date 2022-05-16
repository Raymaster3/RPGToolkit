using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slot
{
    [SerializeField] private ushort id;
    [SerializeField] private ushort quantity;
    [SerializeField] private Item item;
    private short linkIndex;   // What index is the ability linked, -1 if no link

    public Slot(ushort Id)
    {
        id = Id;
    }
    public ushort Id { get => id; set => id = value; } 

    public Item getItem() { return item; }
    public void setItem(Item itm) { item = itm; }
    public ushort getQuantity() { return quantity; }
    public void setQuantity(ushort q) { quantity = q; }
    public short getLinkIndex() { return linkIndex; }
    public void removeItemsFromStack(ushort n)
    {
        if (quantity - n <= 0)
        {
            quantity = 0;
            item = null; // Remove this item from inventory
        }
        else quantity -= n;
    }
    public void addItemsToStack(ushort n, Inventory inv)
    {
        ushort maxStack = item.getMaxStack();
        if (quantity + n <= maxStack)
        {
            // We can stack this
            quantity += n;
        }else
        {
            int leftToStack = maxStack - quantity;
            quantity += (ushort) leftToStack;
            int leftToAllocate = n - leftToStack;
            inv.addItem(item, (ushort) leftToAllocate);
        }
    }
    public void setSlotQuantity(ushort quant)
    {
        quantity = quant;
    }
    public bool hasSpaceInStack(ushort q) { return quantity + q <= item.getMaxStack(); }
    public ushort getAvailableSpace() { return (ushort)(item.getMaxStack() - quantity); }
    public void swapWith(Slot other)
    {
        if (other.getItem() != item)
        {
            Item tempItem = item;
            ushort tempQuant = quantity;

            item = other.item;
            quantity = other.quantity;
            other.item = tempItem;
            other.quantity = tempQuant;
        }else
        {
            ushort space = other.getAvailableSpace();
            int toFill = quantity - space;
            if (toFill >= 0)
            {
                other.setQuantity((ushort)(other.getQuantity() + space));
                quantity -= space;
            }
            else
            {
                
                other.setQuantity((ushort)(other.getQuantity() + quantity));
                item = null;
                quantity = 0;

            }
        }
    }
}
