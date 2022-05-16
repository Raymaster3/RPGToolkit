using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "RPGToolkit/Character", order = 1)]
public class CharacterData : ScriptableObject
{
    [SerializeField] private string characterName;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private StatValue[] stats;
    [SerializeField] private StatValue[] resources;
    [SerializeField] private Ability[] abilities;
    [SerializeField] private Faction faction;
    [SerializeField] private Effect[] permanentEffects;
    [SerializeField] private Inventory defaultInventory;
    [SerializeField] public AnimationClip hitAnim;

    public Faction Faction { get => faction; }
    public Ability[] Abilities { get => abilities; }
    public Effect[] PermanentEffects { get => permanentEffects; }
    public Inventory Inventory { get => defaultInventory; }

    public StatValue getStatByIndex(int pos) { return stats[pos]; }
    public StatValue getStatByName(string name) { 
        foreach (StatValue stat in stats)
        {
            if (stat.getStat().getName() == name) return stat;
        }
        return null;
    }
    public StatValue[] getStats()
    {
        return stats;
    }
    public StatValue[] getResources() { return resources; }
    public string getCharacterName()
    {
        return characterName;
    }
    public string getDescription() { return description; }
    public Sprite getIcon() { return icon; }
}

[System.Serializable]
public class StatValue
{
    [SerializeField]
    private Stat stat;
    [SerializeField]
    private float value;

    [SerializeField, Tooltip("Mark as true if you want this stat to be contained within 2 values")]
    private bool isRange;

    [SerializeField]
    private int min, max;

    public StatValue(Stat m_stat, float m_value, bool m_isRange = false, int m_min = 0, int m_max = 0) { 
        stat = m_stat; value = m_value; isRange = m_isRange; min = m_min; max = m_max; 
    }
    public void BoostByFactor(float factor, Operation op)
    {
        switch (op)
        {
            case Operation.Multiply:
                value = getValue() * factor;
                break;
            default:
                value = getValue() + factor;
                break;
        }
        
    }
    public void DebuffByFactor(float factor)
    {
        value /= factor;
    }

    public Stat getStat() { return stat; }
    public float getValue() {
        if (isRange && value > max) value = max;
        if (isRange && value < min) value = min;
        return value;
    }
    public bool isItRange() { return isRange; }
    public int getMin() { return min; }
    public int getMax() { return max; }
    
}
public enum Operation
{
    Multiply,
    Add
}