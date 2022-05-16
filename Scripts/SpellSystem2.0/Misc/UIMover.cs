using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIMover : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int index;
    private bool interactable = true, initiated = false;
    private Color originalColor;
    [SerializeField] ObjectType type = ObjectType.Ability;
    Vector2 originalPos;
    Transform parent;
    CanvasGroup cg;
    bool draggerOnTop;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        parent = transform.parent;
    }

    public Vector2 getOriginalPos() { return originalPos; }
    public void setOriginalPos(Vector2 pos) { originalPos = pos;}
    public ObjectType Type { get=>type; set => type = value; }

    public void makeInteractable()
    {
        interactable = true;
    }
    public void unMakeInteractable()
    {
        interactable = false;
    }

    public void DraggerOnTop()
    {
        draggerOnTop = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!interactable) return;
        UIManager.instance.setShowToolTip(false);
        UIManager.instance.hideTooltip();
        originalPos = transform.position;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        draggerOnTop = false;
        initiated = true;
        UIManager.instance.sentElementToTop(transform);

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!interactable) return;
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!initiated) return;
        UIManager.instance.setShowToolTip(true);
        Vector2 distance = new Vector2(transform.position.x, transform.position.y) - originalPos;
        transform.position = originalPos;
        cg.interactable = true;
        cg.blocksRaycasts = true;
        UIManager.instance.ReturnElementToParent(transform, parent);
        if (draggerOnTop) return;
        if (distance.magnitude >= 50)
        {
            if (type == ObjectType.Ability)
                UIManager.instance.ShowPopUpWindow("Are you sure you want to remove this ability?", RemoveAbility, () => { UIManager.instance.ClosePopUp(); });
            else
                UIManager.instance.ShowPopUpWindow("Are you sure you want to remove this item?", RemoveItem, () => { UIManager.instance.ClosePopUp(); });
        }
        else SoundManager.instance.Play2DSound(InventoryManager.instance.dragCancelSound);
    }
    private void RemoveAbility()
    {
        // Remove ability
        Ability mock = Instantiate(SpellsManager.instance.getMockAbility());
        mock.index = index;
        mock.Caster = Player.instance;
        Player.instance.Abilities[index] = mock;
        UIManager.instance.updateAbilities();
        SoundManager.instance.PlayRemoveAbilitySound();
    }
    private void RemoveItem()
    {
        Player.instance.getInventory().getSlots()[index].setItem(null);
        Player.instance.getInventory().getSlots()[index].setQuantity(0);
        UIManager.instance.updateInventoryUI();
        SoundManager.instance.PlayRemoveItemSound();
        // Play a sound
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<UIMover>().interactable)
        {
            UIMover uiM = eventData.pointerDrag.GetComponent<UIMover>();
            uiM.DraggerOnTop();

            //Sprite draggedSprite;
            //Image DraggerImg = uiM.GetComponent<Image>();
            //Image droppedImg = GetComponent<Image>();

            //draggedSprite = DraggerImg.sprite;
            //DraggerImg.sprite = droppedImg.sprite;
            //droppedImg.sprite = draggedSprite;

            
            SwapObjects(uiM);
        }
    }
    private void SwapObjects(UIMover dragger)
    {
        // De momento solo habilidades
        RPGObject draggerObject = null;

        if (type == ObjectType.Ability)
        {
            if (dragger.type == ObjectType.Ability)
            {
                // We are dropping an ability into an ability slot
                draggerObject = Player.instance.Abilities[dragger.index];
                draggerObject.Swap(Player.instance.Abilities[index], Player.instance);
                SoundManager.instance.PlayAbilitySwapSound();
            } else {
                // In this case an item is being dropped in an ability slot
                draggerObject = Player.instance.getInventory().getSlots()[dragger.index].getItem();
                draggerObject.Swap(Player.instance.Abilities[index], Player.instance, (ushort) dragger.index);
                SoundManager.instance.PlayItemSwapSound();
                UIManager.instance.updateInventoryUI();
            }
            UIManager.instance.updateAbilities();
        }
        else
        {
            if (dragger.type != ObjectType.Item) return;
            // Both are items
            Player.instance.getInventory().swapItems((ushort) dragger.index, (ushort) index);
            UIManager.instance.updateInventoryUI();
            SoundManager.instance.PlayItemSwapSound();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (type == ObjectType.Item)
        {
            if (eventData.button == PointerEventData.InputButton.Right) ItemRightClick();
        }
    }
    private void ItemRightClick()
    {
        Item item = Player.instance.getInventory().getSlots()[index].getItem();
        if (item != null) 
            item.RightClickAction(Player.instance, (ushort) index);
    }
    #region Tooltip
    public void OnPointerEnter(PointerEventData eventData)
    {
        RPGObject obj = type == ObjectType.Item ? Player.instance.getInventory().getSlots()[index].getItem() : Player.instance.Abilities[index];
        if (obj != null) 
            UIManager.instance.showTooltip(obj.getDescription(), transform.position + new Vector3(0, 100, 0)); // Place it 50 pixels above

        ItemInfoFiller filler = GetComponent<ItemInfoFiller>();
        if (filler != null) filler.SetSelected();

        /*originalColor = GetComponent<Image>().color;
        if (originalColor.a != 0)
            GetComponent<Image>().color = InventoryManager.instance.highlightColor;*/
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.hideTooltip();
        ItemInfoFiller filler = GetComponent<ItemInfoFiller>();
        if (filler != null) filler.UnSelect();
        //GetComponent<Image>().color = originalColor;
    }
    #endregion
}
public enum ObjectType
{
    Item,
    Ability
}
