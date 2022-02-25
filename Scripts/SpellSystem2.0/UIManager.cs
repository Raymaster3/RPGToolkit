using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Image[] abilitySlots;
    public Image health;
    public Image resource;
    public GameObject effectsSlotHolder;
    public GameObject effectSlotPrefab;
    public Image[] coolDownIndicators;
    [SerializeField] private GameObject damageText;
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject channelBar;
    private bool showChannelUI = false;

    public TextMeshProUGUI text;
    public Color color;

    public static UIManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) instance = this;
        //text.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void updateUI()
    {
        foreach (Transform t in effectsSlotHolder.transform)
        {
            Destroy(t.gameObject);
        }
        for (int i = 0; i < abilitySlots.Length && i < Player.instance.Abilities.Count; i++)
        {
            abilitySlots[i].sprite = Player.instance.Abilities[i].Icon;
        }
        for (int i = 0; i < Player.instance.getEffects().Count; i++)
        {
            GameObject effSlot = Instantiate(effectSlotPrefab, effectsSlotHolder.transform);
            effSlot.GetComponent<Image>().sprite = Player.instance.getEffects()[i].Icon;
        }
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
    }
    public void updateCoolDownUI(int pos, float progress)
    {
        Ability ab = Player.instance.Abilities[pos];
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
        go.GetComponent<TMPro.TextMeshProUGUI>().color = color;
        go.GetComponent<TMPro.TextMeshProUGUI>().text = "" + amount;
    }
}
