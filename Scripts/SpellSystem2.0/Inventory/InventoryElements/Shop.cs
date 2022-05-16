using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IEventsHandler
{
    public ItemPrice[] offers;

    [System.Serializable]
    public class ItemPrice
    {
        public Item item;
        public int price;
    }

    public void onMouseEnter()
    {
        UIManager.instance.setShopCursor();
    }

    public void onMouseExit()
    {
        UIManager.instance.setDefaultCursor();
    }

    public void onMouseClick(int button)
    {
        if (button == 1) return;
        UIManager.instance.OpenShop(this);
    }
}

