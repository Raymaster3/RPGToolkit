using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEquippmentSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] EquipPosition slotPosition;

    public EquipPosition getPosition() { return slotPosition; }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Left click behaviour
        }
        else
        {
            // Right click behaviour, unequip 
            Player.instance.getEquipment().unEquipItem(slotPosition, Player.instance);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        RPGObject obj = Player.instance.getEquipment().getSlots()[InventoryManager.instance.getPosition(slotPosition)].getItem();
        if (obj != null)
            UIManager.instance.showTooltip(obj.getDescription(), transform.position + new Vector3(0, 100, 0)); // Place it 50 pixels above
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.hideTooltip();
    }
}
