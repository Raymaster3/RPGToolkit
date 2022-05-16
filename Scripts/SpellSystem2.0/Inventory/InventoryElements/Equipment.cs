using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Equipment
{
    [SerializeField] private Slot[] slots;

    public Equipment(int size)
    {
        slots = new Slot[size];
        for (ushort i = 0; i < size; i++)
        {
            slots[i] = new Slot(i);
        }
    }

    public Slot[] getSlots() { return slots; }

    public void placeItem(Equippable item, Character target, ushort originPosition)
    {
        ushort position = GetItemPos(item.Position);
        if (position == 42069) return;

        if (IteminPosition(item.Position) == null)      // No item equipped in this position
        {
            target.getInventory().getSlots()[originPosition].setItem(null);
            target.getInventory().getSlots()[originPosition].setQuantity(0);
        }
        else
        {
            foreach (StatValueEffect stat in ((Equippable)slots[position].getItem()).getStats())
                target.revertModifier(stat.getStat(), stat.getValue(), stat.getOperation());

            // In here we need to check if the new item is two-handed to replace both hands
            // Also we need to check if the equipped item is two handed and we want to equip a one-handed item
            Equippable equippedItem = (Equippable) slots[position].getItem();

            target.getInventory().getSlots()[originPosition].setItem(slots[position].getItem());
            
            if (equippedItem.Position == EquipPosition.TwoHanded)
            {
                // Always remove both items
                slots[InventoryManager.instance.getPosition(EquipPosition.MainHand)].setItem(null);
                slots[InventoryManager.instance.getPosition(EquipPosition.SecondHand)].setItem(null);
                target.removeWeapon(EquipPosition.SecondHand);
            }
            else slots[position].setItem(null);

            if (item.Position == EquipPosition.TwoHanded)
                if (slots[InventoryManager.instance.getPosition(EquipPosition.SecondHand)].getItem() != null)
                {
                    target.getInventory().addItem(slots[InventoryManager.instance.getPosition(EquipPosition.SecondHand)].getItem());
                    slots[InventoryManager.instance.getPosition(EquipPosition.SecondHand)].setItem(null);
                    target.removeWeapon(EquipPosition.SecondHand);
                }

            if (isWeapon(equippedItem.Position)) target.removeWeapon(equippedItem.Position);
            AnimationsManager.instance.RevertAnimations(target);
        }
        // Apply changes to stats
        foreach (StatValueEffect stat in item.getStats())
            target.modifyStatByFactor(stat.getStat(), stat.getValue(), stat.getOperation());

        // Show changes on character
        if (item.getPrefab() != null) target.changeBodyVisual(item.Position, item.getPrefab(), item.getShowOnBothHands());
        else target.changeBodyVisual(item.Position, item.getVisual(), item.getMat());

        if (item.Position == EquipPosition.TwoHanded)
        {
            slots[InventoryManager.instance.getPosition(EquipPosition.SecondHand)].setItem(item);
        }

        slots[InventoryManager.instance.getPosition(item.Position)].setItem(item);
        AnimationsManager.instance.ChangeAnimations(target, item.getAnimationsOverrides());
        UIManager.instance.updateInventoryUI();
    }
    public void unEquipItem(EquipPosition position, Character target)
    {
        if (slots[(int)position].getItem() == null) return;   // No item

        target.getInventory().addItem(slots[(int)position].getItem(), 1);

        /*if (position != EquipPosition.MainHand && position != EquipPosition.SecondHand && position != EquipPosition.TwoHanded)
            target.changeBodyVisual(position);          // Default
        else target.removeWeapon(position);*/

        if (position == EquipPosition.MainHand || position == EquipPosition.SecondHand)
        {
            // Weapon
            EquipPosition itemPos = ((Equippable)slots[InventoryManager.instance.getPosition(position)].getItem()).Position;
            target.removeWeapon(itemPos);
            if (itemPos == EquipPosition.TwoHanded)
            {
                // Item is two-handed
                slots[InventoryManager.instance.getPosition(EquipPosition.MainHand)].setItem(null);
                slots[InventoryManager.instance.getPosition(EquipPosition.SecondHand)].setItem(null);
            }
            else slots[InventoryManager.instance.getPosition(position)].setItem(null);
        } else target.changeBodyVisual(position);

        slots[InventoryManager.instance.getPosition(position)].setItem(null);

        if (position == EquipPosition.TwoHanded) slots[InventoryManager.instance.getPosition(EquipPosition.SecondHand)].setItem(null);

        AnimationsManager.instance.RevertAnimations(target);
    }

    private Item IteminPosition(EquipPosition pos)
    {
        if (pos == EquipPosition.TwoHanded)
        {
            ushort position = InventoryManager.instance.getPosition(EquipPosition.MainHand);
            Item it = slots[position].getItem();
            if (it != null) return it;
            position = InventoryManager.instance.getPosition(EquipPosition.SecondHand);
            it = slots[position].getItem();
            if (it != null) return it;
        }
        return slots[InventoryManager.instance.getPosition(pos)].getItem();
    }
    private ushort GetItemPos(EquipPosition pos)
    {
        if (pos == EquipPosition.TwoHanded)
        {
            ushort position = InventoryManager.instance.getPosition(EquipPosition.MainHand);
            Item it = slots[position].getItem();
            if (it != null) return position;
            position = InventoryManager.instance.getPosition(EquipPosition.SecondHand);
            it = slots[position].getItem();
            if (it != null) return position;
        }
        return InventoryManager.instance.getPosition(pos);
    }
    private bool isWeapon(EquipPosition pos) {
        return pos == EquipPosition.MainHand || pos == EquipPosition.SecondHand || pos == EquipPosition.TwoHanded;
    }
    public void ReloadAnimations(Character target)
    {
        foreach (Slot slot in slots)
        {
            if (slot.getItem() == null) continue;
            AnimationsManager.instance.ChangeAnimations(target, ((Equippable)slot.getItem()).getAnimationsOverrides());
        }
    }
}
