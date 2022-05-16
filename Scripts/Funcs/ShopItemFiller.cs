using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemFiller : MonoBehaviour
{
    public Text itemTitle;
    public Text itemPrice;
    public Image itemIcon;

    public Button buyButton;

    public Button closeButton;

    private void Start()
    {
        closeButton.onClick.AddListener(() => { UIManager.instance.CloseShop(); });
    }
}
