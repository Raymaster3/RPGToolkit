using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [Header("Player UI")]
    public Image[] abilitySlots;
    public Sprite defaultAbilitySlot;
    public Image health;
    public Image resource;
    public GameObject effectsSlotHolder;
    public GameObject effectSlotPrefab;
    public Image[] coolDownIndicators;
    [Header("GameObjects")]
    [SerializeField] private GameObject damageText;
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject channelBar;
    [SerializeField] private GameObject popUpWindow;
    [SerializeField] private GameObject tooltipPanel;
    [Header("Error Message")]
    [SerializeField] private GameObject errorText;
    private bool showToolTip = true;

    [Header("Windows")]
    public Window characterWindow;
    public Window dialogueWindow;
    public Window questsTabWindow;
    public Window shopWindow;
    public SelectedCharWindow selectedCharWindow;

    [Header("Cursors")]
    public Texture2D defaultCursor;
    public Texture2D shopCursor;
    public Texture2D attackCursor;
    public Vector2 shopxoffSet = new Vector2(45, 0);
    public Vector2 defaultxoffSet = new Vector2 (30, 0);
    public Vector2 attackoffSet = new Vector2 (30, 0);

    private bool showChannelUI = false;

    [Header("Test")]
    public bool test;
    [ConditionalHide("test", true)]
    public string tryyyying;
    [ConditionalHide("test", true)]
    public string testt2;
    [ConditionalHide("test", true)]
    public string teeest3;


    public static UIManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) instance = this;
        //text.color = color;
        setDefaultCursor();
    }
    private void Start()
    {
        int i = 0;
        foreach (Image slot in abilitySlots)
        {
            slot.gameObject.AddComponent<CanvasGroup>();
            UIMover uiM = slot.gameObject.AddComponent<UIMover>();
            uiM.index = i;
            uiM.Type = ObjectType.Ability;
            i++;
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        if (Raycaster.instance.openCharWindowAction)
        {
            ToggleCharWindow();
        }
        if (Input.GetKeyDown("s"))
        {
            ShowMessage("Hola funco", Color.green, ErrorDuration.Short);
        }
        if (Input.GetKeyDown("l"))
        {
            ShowMessage("Hola funco", Color.green, ErrorDuration.Long);
        }
    }

    public void ToggleCharWindow()
    {
        if (characterWindow.isOpened()) CloseCharWindow();
        else OpenCharWindow();
    }
    public void OpenCharWindow()
    {
        characterWindow.Open();
        characterWindow.Populate();
    }
    public void CloseCharWindow()
    {
        characterWindow.Close();
    }

    public void updateUI()
    {
        updateEffects();
        updateAbilities();
    }
    public void updateEffects()
    {
        foreach (Transform t in effectsSlotHolder.transform)
        {
            Destroy(t.gameObject);
        }
        for (int i = 0; i < Player.instance.getEffects().Count; i++)
        {
            GameObject effSlot = Instantiate(effectSlotPrefab, effectsSlotHolder.transform);
            effSlot.GetComponent<Image>().sprite = Player.instance.getEffects()[i].Icon;
        }
    }
    public void updateAbilities()
    {
        if (Player.instance.Abilities == null) return;
        for (int i = 0; i < abilitySlots.Length && i < Player.instance.Abilities.Count; i++)
        {
            abilitySlots[i].sprite = Player.instance.Abilities[i].Icon != null ? Player.instance.Abilities[i].Icon : defaultAbilitySlot;

            if (Player.instance.Abilities[i].GetMyType() == "Ability" && ((Ability)Player.instance.Abilities[i]).Mock)
                abilitySlots[i].GetComponent<UIMover>().unMakeInteractable();
            else abilitySlots[i].GetComponent<UIMover>().makeInteractable();
        }
    }
    public void updateStatsUI()
    {
        StatValue hp = Player.instance.getStatByName("Health");
        if (hp != null)
        {
            health.material.SetColor("_MainColor", hp.getStat().getColor());
            health.fillAmount = hp.getValue() / hp.getMax();
        }
        StatValue[] resources = Player.instance.getResources().ToArray();
        if (resources.Length != 0)
        {
            resource.material.SetColor("_MainColor", resources[0].getStat().getColor());
            resource.fillAmount = resources[0].getValue() / resources[0].getMax();
        }
        if (characterWindow.isOpened()) characterWindow.Populate();
    }
    public void updateInventoryUI()
    {
        if (characterWindow.isOpened()) characterWindow.Populate();
    }
    public void updateCoolDownUI(int pos, float progress)
    {
        Ability ab = ((Ability) Player.instance.Abilities[pos]);
        coolDownIndicators[pos].fillAmount = 1 - (progress / ab.CoolDown);
    }
    public void ToggleChannelingUI()
    {
        showChannelUI = !showChannelUI;
        channelBar.SetActive(showChannelUI);
    }
    private void ShowChannelUI()
    {
        channelBar.SetActive(true);
        showChannelUI = true;
    }
    public void HideChannelUI()
    {
        showChannelUI = false;
        channelBar.SetActive(false);
    }
    public void UpdateChannelSlider(float progress, float max)
    {
        ShowChannelUI();
        Slider slr = channelBar.GetComponent<Slider>();
        slr.maxValue = max;
        slr.value = progress;
    }

    public void BlockAbility(int pos)
    {
        coolDownIndicators[pos].fillAmount = 1;
    }
    public void UnBlockAbility(int pos)
    {
        coolDownIndicators[pos].fillAmount = 0;
    }
    public void displayDamage(float amount, Color color, Vector3 position)
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(position);
        GameObject go = Instantiate(damageText, screenPos, Quaternion.identity, canvas);
        go.GetComponent<DamageTextAnim>().targetPos = position;
        go.GetComponent<DamageTextAnim>().textColor = color;
        go.GetComponent<TMPro.TextMeshProUGUI>().text = "" + amount;
    }
    
    public void sentElementToTop(Transform element)
    {
        element.SetParent(canvas);
    }
    public void ReturnElementToParent(Transform element,Transform parent)
    {
        element.SetParent(parent);
    }
    #region PopUp Window
    public void ShowPopUpWindow(string message, UnityEngine.Events.UnityAction acceptBehaviour, UnityEngine.Events.UnityAction declineBehavior)
    {
        //popUpWindow.transform.position = new Vector2(Screen.height / 2, Screen.width / 2);
        Text messageText = popUpWindow.transform.GetChild(0).GetComponent<Text>();
        messageText.text = message;

        Button agree = popUpWindow.transform.GetChild(1).GetComponent<Button>();
        agree.onClick = new Button.ButtonClickedEvent();   // Reset actions
        agree.onClick.AddListener(acceptBehaviour);
        agree.onClick.AddListener(ClosePopUp);

        Button decline = popUpWindow.transform.GetChild(2).GetComponent<Button>();
        decline.onClick = new Button.ButtonClickedEvent();
        decline.onClick.AddListener(declineBehavior);
        decline.onClick.AddListener(ClosePopUp);
        popUpWindow.SetActive(true);
    }
    public void ClosePopUp()
    {
        popUpWindow.SetActive(false);
    }
    #endregion
    #region Tooltip
    public void setShowToolTip(bool st)
    {
        showToolTip = st;
    }
    public void showTooltip(string message, Vector3 position = default(Vector3))
    {
        if (!showToolTip) return;
        if (position == default(Vector3)) position = Input.mousePosition;
        if (tooltipPanel.activeSelf == false) { tooltipPanel.SetActive(true); }

        tooltipPanel.transform.position = position;
        TextMeshProUGUI txt = tooltipPanel.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        txt.SetText(message);
        txt.ForceMeshUpdate();  
    }
    public void hideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
    #endregion
    #region Dialogue Window
    public void ShowDialogueWindow(DialogNode node, RuntimeDialogueContainer context)
    {
        DialogueWIndow window = dialogueWindow as DialogueWIndow;
        window.currentNode = node;
        window.context = context;
        window.Open();
    }
    public void HideDialogueWindow()
    {
        dialogueWindow.Close();
    }
    #endregion
    #region Quests Tab
    public void UpdateQuestsTab()
    {
        questsTabWindow.Populate();
    }
    #endregion
    #region Shop Window
    public void OpenShop(Shop s)
    {
        ((ShopWindow)shopWindow).targetShop = s;
        shopWindow.Open();
    }
    public void CloseShop()
    {
        shopWindow.Close();
    }
    #endregion
    #region Selected Character Window
    public void ShowSelectedCharWindow(Character c)
    {       
        selectedCharWindow.selectedChar = c;
        selectedCharWindow.Open();
    }

    public void closeSelctedCharWindow()
    {
        selectedCharWindow.Close();
    }
    public void UpdateSelectedChar(Character c)
    {
        if (c == selectedCharWindow.selectedChar) // Only update ui if the affected character is selected
        {
            selectedCharWindow.Populate();
        }
    }
    #endregion
    #region Cursors
    public void setDefaultCursor()
    {
        if (defaultCursor != null)
            Cursor.SetCursor(defaultCursor, defaultxoffSet, CursorMode.Auto);
    }
    public void setShopCursor()
    {
        if (shopCursor != null)
            Cursor.SetCursor(shopCursor, shopxoffSet, CursorMode.Auto);
    }
    public void setAttackCursor()
    {
        if (attackCursor != null)
            Cursor.SetCursor(attackCursor, attackoffSet, CursorMode.Auto);
    }
    #endregion
    #region Error Message
    public void ShowMessage(string message, Color color, ErrorDuration duration)
    {
        GameObject go = Instantiate(errorText, canvas);
        ErrorMessage errMess = go.GetComponent<ErrorMessage>();
        errMess.color = color;
        errMess.message = message;
        //errMess.duration = 3f;
        errMess.errDur = duration;
    }
    #endregion
}
