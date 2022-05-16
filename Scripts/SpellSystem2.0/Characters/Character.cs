using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour, IEventsHandler
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
    private Inventory inventory;
    [SerializeField] private Equipment equipment;

    private AnimationClip hitAnimation;

    [Header("Equipment Visuals")]
    [SerializeField] private SkinnedMeshRenderer[] bodyParts;
    [SerializeField] private SkinnedMeshRenderer[] defaultParts; 
    [SerializeField] private Transform leftTransform;
    [SerializeField] private Transform rightTransform;

    protected List<RPGObject> abilities;

    public Faction Faction { get => faction; }
    public List<RPGObject> Abilities { get => abilities; }

    [SerializeField] private List<Effect> effects;
    [SerializeField] private Effect[] passiveEffects;

    private AnimatorOverrideController overrideController; // Utils
    protected bool interactable = true;

    private void Awake()
    {
        abilities = new List<RPGObject>();
    }
    // Start is called before the first frame update
    void Start()
    {
        loadData();
       /* for (int i = 0; i < bodyParts.Length; i++)
        {
            defaultMeshes[i] = bodyParts[i].sharedMesh;
            defaultMaterials[i] = bodyParts[i].material;
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        /*healthText.text = "Health: " + stats[1].getValue();
        speedText.text = "Speed: " + stats[0].getValue();*/

       /* healthText.color = stats[1].getStat().getColor();
        speedText.color = stats[0].getStat().getColor();*/

        /*if (Input.GetKeyDown("b"))
        {
            stats[0].BoostByFactor(2, Operation.Multiply);
        }*/
    }

    public Sprite getIcon() { return icon; }
    public string getCharName() { return characterName; }
    public List<StatValue> getStats() { return stats; }
    private void loadData()
    {
        // Initialize runtime data
        characterName = data.getCharacterName();
        description = data.getDescription();
        icon = data.getIcon();
        faction = data.Faction;
        passiveEffects = data.PermanentEffects;
        inventory = new Inventory(data.Inventory, this); // For now we dont want to store state
        equipment = new Equipment(7);
        hitAnimation = data.hitAnim;

        foreach (Effect e in passiveEffects)
        {
            // Activate the effect
            e.Target = this;
            e.OnStartEffecting();
        }

        resources = new List<StatValue>();
        abilities = new List<RPGObject>();
        stats = new List<StatValue>();

        for (int i = 0; i < data.Abilities?.Length; i++)
        {
            Ability ab = data.Abilities[i] == null ? Instantiate(SpellsManager.instance.getMockAbility()) : Instantiate(data.Abilities[i]);
            ab.index = i;
            ab.Caster = this;
            abilities.Add(ab);
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
                UIManager.instance.updateStatsUI();
                UIManager.instance.UpdateSelectedChar(this);
            }
        }
    }

    public void addVisualEffect(GameObject prefab, float duration)
    {
        GameObject go = Instantiate(prefab, transform);     // Instantiate as child of character object
        go.transform.localPosition = Vector3.zero;
        go.AddComponent<Destroyer>().lifeTime = duration;
        go.GetComponent<Destroyer>().OnDestroy = () => { go.GetComponent<ParticleSystem>().Stop(); };
    }

    public void addEffect(Effect eff)
    {
        if (!interactable) return;
        if (effects == null) effects = new List<Effect>();
        effects.Add(eff);
        UIManager.instance.updateEffects();
    }
    public void addEffects(List<Effect> effs)
    {
        if (!interactable) return;
        if (effects == null) effects = new List<Effect>();
        effects.AddRange(effs);
    }
    public void removeEffect(Effect eff)
    {
        effects?.Remove(eff);
        UIManager.instance.updateEffects();
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
        if (!interactable) return;
        StatValue sv = getStatByName(stat.getName());

        if (sv != null) sv.BoostByFactor(factor, op); // Buff or debuff
        if (sv.getStat().getName() == "Health")
        {
            UIManager.instance.displayDamage(Mathf.Abs(factor), factor < 0 ? Color.red : Color.green, transform.position);
            if (factor < 0)
                Hit();
            if (sv.getValue() <= 0)
                Die();
        }
        UIManager.instance.updateStatsUI();
        UIManager.instance.UpdateSelectedChar(this);
    }
    /// <summary>
    /// Reverts the existing modifiers for this stat
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="value">The value of the modifier passed in the previous function</param>
    /// <param name="op">The operation that was performed</param>
    public void revertModifier(Stat stat, float value, Operation op)
    {
        if (!interactable) return;
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
        UIManager.instance.UpdateSelectedChar(this);
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
    public virtual void UpdateMovingAnimator()
    {
        
    }
    public virtual void StopMoving()
    {
        
    }
    public virtual void BlockMovement()
    {
        
    }
    public virtual void UnBlockMovement()
    {
        
    }
    public void SwapAbilityPos(Ability a, Ability b)
    {
        //Ability temp = abilities[a.index];
        abilities[a.index] = b;
        abilities[b.index] = a;
    }
    public Vector3 getCharacterForward()
    {
        return transform.GetChild(0).transform.forward;
    }
    public void LookAtTarget(Transform target)
    {
        transform.GetChild(0).LookAt(target);
    }
    public void LookAtTarget(Vector3 pos)
    {
        transform.GetChild(0).LookAt(pos);
    }
    public Inventory getInventory() { return inventory; }
    public Equipment getEquipment() { return equipment; }
    public void changeBodyVisual(EquipPosition part)        // Show default state
    {
        bodyParts[(int)part].enabled = false;
        if (defaultParts[(int)part] != null)
            defaultParts[(int)part].enabled = true;
        EquipmentPreview.instance.updateVisual();
    }
    public void changeBodyVisual(EquipPosition part, Mesh visual, Material material = null)
    {
        bodyParts[(int)part].enabled = true;
        if (defaultParts[(int)part] != null)
            defaultParts[(int)part].enabled = false;
        bodyParts[(int)part].sharedMesh = visual;
        if (material != null)
            bodyParts[(int)part].material = material;
        EquipmentPreview.instance.updateVisual();
    }
    public void changeBodyVisual(EquipPosition part, GameObject prefab, bool bothHands = false)
    {
        GameObject weapon = Instantiate(prefab);
        weapon.transform.localScale = transform.GetChild(0).localScale;

        if (part == EquipPosition.SecondHand)
        {
            // Left hand
            weapon.transform.parent = leftTransform;
        }
        else
        {
            // Right hand
            weapon.transform.parent = rightTransform;
            if (part == EquipPosition.TwoHanded && bothHands)
            {
                GameObject weapon2 = Instantiate(prefab, leftTransform);
                //weapon2.transform.localScale = transform.GetChild(0).localScale;
                weapon2.transform.localPosition = Vector3.zero;
                weapon2.transform.localRotation = Quaternion.Euler(180,0,0);
            }
        }
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
    }
    public void removeWeapon(EquipPosition part)
    {
        if (part == EquipPosition.SecondHand)
        {
            if (leftTransform.childCount > 0)
                Destroy(leftTransform.GetChild(0).gameObject);
            return;
        }else if (part == EquipPosition.TwoHanded)
        {
            if (leftTransform.childCount > 0)
                Destroy(leftTransform.GetChild(0).gameObject);
        }
        if (rightTransform.childCount > 0)
            Destroy(rightTransform.GetChild(0).gameObject);
    }
    public AnimatorOverrideController getController() { return overrideController; }
    public void setController(AnimatorOverrideController cont) { overrideController = cont; }
   // public virtual void updateQuests(object objective) { }
    public void Die()
    {
        Player.instance.updateQuests(this, 1);
        interactable = false;
    }
    public void Hit()
    {
        AnimationsManager.instance.PlayHitAnim(this, hitAnimation);
    }

    public void onMouseEnter()
    {
        Character player = Player.instance;
        RelationType rel = player.Faction.getRealtionStatus(faction);
        if (rel != RelationType.Friendly) UIManager.instance.setAttackCursor();
    }

    public void onMouseExit()
    {
        UIManager.instance.setDefaultCursor();
    }

    public void onMouseClick(int button)
    {
        if (button == 0)
        {
            SpellsManager.instance.target = this;
            UIManager.instance.ShowSelectedCharWindow(this);
        }
    }
    public void ToggleInteractable()
    {
        interactable = !interactable;
    }
    public bool isInteractable() { return interactable; }
}
