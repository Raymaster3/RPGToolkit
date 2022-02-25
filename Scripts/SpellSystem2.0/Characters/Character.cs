using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public CharacterData data;

    public Text healthText;
    public Text speedText;


    private string characterName;
    private string description;
    private Sprite icon;
    private List<StatValue> stats;
    private List<StatValue> resources;
    private Faction faction;

    protected List<Ability> abilities;

    public Faction Faction { get => faction; }
    public List<Ability> Abilities { get => abilities; }

    [SerializeField] private List<Effect> effects;
    [SerializeField] private Effect[] passiveEffects;

    private void Awake()
    {
        abilities = new List<Ability>();
    }
    // Start is called before the first frame update
    void Start()
    {
        loadData();
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = "Health: " + stats[1].getValue();
        speedText.text = "Speed: " + stats[0].getValue();

        healthText.color = stats[1].getStat().getColor();
        speedText.color = stats[0].getStat().getColor();

        if (Input.GetKeyDown("b"))
        {
            stats[0].BoostByFactor(2, Operation.Multiply);
        }
    }

    private void loadData()
    {
        // Initialize runtime data
        characterName = data.getCharacterName();
        description = data.getDescription();
        icon = data.getIcon();
        faction = data.Faction;
        passiveEffects = data.PermanentEffects;

        foreach (Effect e in passiveEffects)
        {
            // Activate the effect
            e.Target = this;
            e.OnStartEffecting();
        }

        resources = new List<StatValue>();
        abilities = new List<Ability>();
        stats = new List<StatValue>();

        for (int i = 0; i < data.Abilities.Length; i++)
        {
            //Instantiate(data.Abilities[i]);
            if (data.Abilities[i] != null)
            {
                Ability ab = Instantiate(data.Abilities[i]);
                ab.index = i;
                abilities.Add(ab);
            }
        }

        for (int i = 0; i < data.getResources().Length; i++)
        {
            StatValue stat = data.getResources()[i];
            if (stat != null) resources.Add(new StatValue(stat.getStat(), stat.getValue(), stat.isItRange(), stat.getMin(), stat.getMax()));
        }

        int index = 0;
        foreach (StatValue stat in data.getStats())
        {
            stats.Add(new StatValue(stat.getStat(), stat.getValue(), stat.isItRange(), stat.getMin(), stat.getMax()));
            //stats[index].setOperation(stat.getOperation());
            index++;
        }
        UIManager.instance.updateUI();
    }

    public List<Effect> getEffects()
    {
        return effects;
    }

    public bool hasResources(Stat resource, float amountNeeded)
    {
        foreach (StatValue sv in resources)
        {
            if (sv.getStat().getName() == resource.getName())
            {
                return sv.getValue() >= amountNeeded;
            }
        }
        return true;
    }

    public void useResource(Stat resource, float amount)
    {
        foreach (StatValue sv in resources)
        {
            if (sv.getStat().getName() == resource.getName())
            {
                sv.BoostByFactor(-amount, Operation.Add);
                UIManager.instance.updateUI();
            }
        }
    }

    public void addEffect(Effect eff)
    {
        if (effects == null) effects = new List<Effect>();
        effects.Add(eff);
        UIManager.instance.updateUI();
    }
    public void addEffects(List<Effect> effs)
    {
        if (effects == null) effects = new List<Effect>();
        effects.AddRange(effs);
    }
    public void removeEffect(Effect eff)
    {
        effects?.Remove(eff);
        UIManager.instance.updateUI();
    }
    public void removeEffects(List<Effect> eff)
    {
        foreach(Effect e in eff)
        {
            effects?.Remove(e);
        }
    }
    public bool hasEffect(Effect eff)
    {
        return effects.Contains(eff);
    }
    /// <summary>
    /// Multiply a stat by a factor
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="factor"></param>
    public void modifyStatByFactor(Stat stat, float factor, Operation op)
    {
        StatValue sv = getStatByName(stat.getName());
        if (sv != null) sv.BoostByFactor(factor, op); // Buff or debuff
        UIManager.instance.updateUI();
    }
    /// <summary>
    /// Reverts the existing modifiers for this stat
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="value">The value of the modifier passed in the previous function</param>
    /// <param name="op">The operation that was performed</param>
    public void revertModifier(Stat stat, float value, Operation op)
    {
        switch(op)
        {
            case Operation.Multiply:
                getStatByName(stat.getName()).BoostByFactor(1/value, op);
                break;
            default:
                getStatByName(stat.getName()).BoostByFactor(-value, op);
                break;
        }
        UIManager.instance.updateUI();
    }
    public List<StatValue> getResources() { return resources; }
    public StatValue getStatByName(string name)
    {
        if (stats == null) stats = new List<StatValue>();
        foreach (StatValue stat in stats)
        {
            if (stat.getStat().getName() == name) return stat;
        }
        if (resources == null) resources = new List<StatValue>();
        foreach (StatValue stat in resources)
        {
            if (stat.getStat().getName() == name) return stat;
        }
        return null;
    }
    public StatValue getStatByIndex(int pos) { return stats[pos]; }
}
