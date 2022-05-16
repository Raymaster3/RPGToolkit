using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoFiller : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] Image slotImage;
    [SerializeField] Image backGroundSelectedImage;
    [SerializeField] Text itemQuantityText;
    [SerializeField] GameObject itemQuantitySlot;

    private Color defaultBackground;
    private bool firstSelected = false;
    
    public void HideQuantity()
    {
        itemQuantityText.gameObject.SetActive(false);
        itemQuantitySlot.SetActive(false);
    }
    public void ShowQuantity(ushort quantity)
    {
        itemQuantitySlot.SetActive(true);
        itemQuantityText.gameObject.SetActive(true);
        itemQuantityText.text = "" + quantity;
    }
    public void setIcon(Sprite icon)
    {
        Color color = itemImage.color;

        if (icon == null) itemImage.color = new Color(color.r, color.g, color.b, 0);
        else itemImage.color = new Color(color.r, color.g, color.b, 1);

        itemImage.sprite = icon;
    }
    public void SetSelected()
    {
        if (!firstSelected) defaultBackground = InventoryManager.instance.highlightColor;
        firstSelected = true;
        backGroundSelectedImage.color = new Color(defaultBackground.r, defaultBackground.g, defaultBackground.b, 1);
    }
    public void UnSelect()
    {
        backGroundSelectedImage.color = defaultBackground;
    }
    public void ToggleSelection()
    {

    }
    public void setSlotColor(Color color)
    {
        slotImage.color = color;
    }
}
