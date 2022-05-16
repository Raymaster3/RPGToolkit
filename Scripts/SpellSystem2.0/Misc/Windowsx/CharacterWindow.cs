using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterWindow : Window
{
    [SerializeField] private Image[] slotImages;
    [SerializeField] private Image charImage;
    [SerializeField] private Text characterNameText;
    [SerializeField] private Text GoldCounter;
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private GameObject statTextPrefab;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject equippmentPanel;
    [SerializeField] private AudioClip openSound, closeSound;


    public override void Close()
    {
        gameObject.SetActive(false);
        SoundManager.instance.Play2DSound(closeSound);
        UIManager.instance.hideTooltip();
    }

    public override bool isOpened()
    {
        return gameObject.activeSelf;
    }

    public override void Open()
    {
        gameObject.SetActive(true);
        SoundManager.instance.Play2DSound(openSound);
        EquipmentPreview.instance.ResetTransform();
        // Fill the inventory slots
        ushort maxSlots = InventoryManager.instance.maxSlots;
        GameObject og = inventoryPanel.transform.GetChild(0).gameObject;
        GoldCounter.text = "" + Player.instance.getInventory().getGold();
        int j = 0;
        foreach (Transform t in inventoryPanel.transform)
        {
            if (j != 0) Destroy(t.gameObject);
            j++;
        }
        ItemInfoFiller filler = GetComponentInChildren<ItemInfoFiller>();

        if (filler.transform.GetComponent<UIMover>() == null)
        {
            filler.transform.gameObject.AddComponent<UIMover>().Type = ObjectType.Item;
        }
        filler.transform.GetComponent<UIMover>().index = 0;

        for (ushort i = 1; i < maxSlots; i++)
        {
            GameObject go = Instantiate(og, inventoryPanel.transform);
            filler = go.GetComponentInChildren<ItemInfoFiller>();
            go.transform.SetSiblingIndex(i);
            filler.transform.GetComponent<UIMover>().Type = ObjectType.Item;
            filler.transform.GetComponent<UIMover>().index = i;
        }
    }

    public override void Populate()
    {
        //charImage.sprite = Player.instance.getIcon();
        characterNameText.text = Player.instance.getCharName();
        GoldCounter.text = "" + Player.instance.getInventory().getGold();

        foreach (Transform trans in statsPanel.transform)
        {
            Destroy(trans.gameObject); // Clear
        }
        List<StatValue> stats = Player.instance.getStats();
        foreach (StatValue stat in stats)
        {
            GameObject go = Instantiate(statTextPrefab, statsPanel.transform);
            Text txt = go.GetComponent<Text>();
            txt.color = stat.getStat().getColor();
            txt.text = stat.getStat().getName() + ": " + stat.getValue();
        }
        int i = 0;
        foreach (Transform t in inventoryPanel.transform)
        {
            // Iterate through all children
            if (t.childCount == 0) { i++; continue; } // Currently dragging
            if (Player.instance.getInventory().getSlots().Length <= i) return;
            Item item = Player.instance.getInventory().getSlots()[i].getItem();
            ItemInfoFiller itf = t.GetComponentInChildren<ItemInfoFiller>();
            if (itf == null)
            {
                i++;
                continue;
            }
            if (item != null)
            {
                itf.transform.GetComponent<UIMover>().makeInteractable();
                itf.ShowQuantity(Player.instance.getInventory().getSlots()[i].getQuantity());
                itf.setIcon(item.Icon);
                Color color = item.getRarity() == null ? Color.white : item.getRarity().getColor();
                itf.setSlotColor(color);
            }
            else
            {
                itf.HideQuantity();
                itf.setIcon(null);
                itf.setSlotColor(Color.white);
                itf.transform.GetComponent<UIMover>().unMakeInteractable();
                itf.transform.GetComponent<CanvasGroup>().interactable = false;
            }
            i++;
        }  // Fill inventory info
        foreach (Transform t in equippmentPanel.transform)
        {
            Transform childSlot = t.GetChild(0);
            EquipPosition pos = t.GetComponent<UIEquippmentSlot>().getPosition();
            if (Player.instance.getEquipment().getSlots()[InventoryManager.instance.getPosition(pos)].getItem() == null)
            {
                Color c = childSlot.GetComponent<Image>().color;
                childSlot.GetComponent<Image>().color = new Color(c.r, c.b, c.g, 0);
            }else
            {
                Color c = childSlot.GetComponent<Image>().color;
                childSlot.GetComponent<Image>().color = new Color(c.r, c.b, c.g, 1);
                childSlot.GetComponent<Image>().sprite = Player.instance.getEquipment().getSlots()[InventoryManager.instance.getPosition(pos)].getItem().Icon;
            }
        } // Fill equippment info
    }
    public override void UpdateWindow()
    {

    }
}
