using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopWindow : Window
{
    public GameObject offersPanel;
    [HideInInspector] public Shop targetShop;

    private GameObject prefab;

    public override void Open()
    {
        if (prefab == null) // First time opening the window
            prefab = Instantiate(offersPanel.transform.GetChild(0).gameObject); // Cache first element to use as prefab
        gameObject.SetActive(true);
        Populate();
    }
    public override void Close()
    {
        gameObject.SetActive(false);
    }
    public override void Populate()
    {
        Clear();
        foreach (Shop.ItemPrice offer in targetShop.offers)
        {
            GameObject go = Instantiate(prefab, offersPanel.transform);
            ShopItemFiller filler = go.GetComponent<ShopItemFiller>();
            filler.itemIcon.sprite = offer.item.Icon;
            filler.itemTitle.text = offer.item.Name;
            filler.itemPrice.text = "" + offer.price;

            filler.buyButton.onClick.AddListener(() => {
                Inventory inv = Player.instance.getInventory();
                if (inv.canAddItem(offer.item) && inv.spendGold(offer.price))
                {
                    inv.addItem(offer.item);
                }
            });
        }

    }
    private void Clear()
    {
        foreach (Transform child in offersPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
