using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect : ScriptableObject
{
    // Private varaibles accessible from inspector
    [SerializeField] private string effectName;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private float duration;
    [SerializeField] private StatValueEffect[] statsToModify;
    //[SerializeField] private Stat statToModify;
    [SerializeField] private bool permanent;
    [SerializeField] protected EffectType natureOfEffect;
    [SerializeField] protected GameObject effectPrefab;

    // Private variables
    private Operation operation;                     // Filled by the ability
    private float value;                             

    private float timer = 0;
    private Character target;

    // Public accessors
    public StatValueEffect[] StatsToModify { get => statsToModify; }
    //public Stat StatToModify { get => statToModify; set => statToModify = value; }
    public Character Target { get => target; set => target = value; }
    public Operation Operation { get => operation; set => operation = value; }
    public Sprite Icon { get => icon; }
    public float Duration { get => duration; set => duration = value; }
    public float Value { get => value; }


    public bool checkIfValid(Character caster, Character target)
    {
        if (natureOfEffect == EffectType.Positive)
            return caster.Faction.getRealtionStatus(target.Faction) == RelationType.Friendly;
        else
            return caster.Faction.getRealtionStatus(target.Faction) != RelationType.Friendly;
    }

    public virtual void OnApply(Character trgt) {   // Called once when the effect is applied to a character
        target = trgt;
        Effect eff = Instantiate(this);             // Copy the effect for the player
        eff.Target = target;
        target.addEffect(eff);
        if (effectPrefab != null) target.addVisualEffect(effectPrefab, duration);
        eff.OnStartEffecting();                     // We call the functions on the copy we created 
    }
    public virtual void OnStartEffecting() {        // Called once when the effect starts effecting
        SpellsManager.instance.SubscribeFunction(WhileEffecting);
        foreach (StatValueEffect stat in StatsToModify)
        {
            Target.modifyStatByFactor(stat.getStat(), stat.getValue(), stat.getOperation()); 
        }
    }
    public virtual void WhileEffecting() {          // Called every frame while the effect is applied
        if (permanent) return;                      // If permanent we dont want to end the timer

        // Timer
        timer += Time.deltaTime;
        if (timer < duration) return;
        timer = 0;
        OnEndEffecting();
    }
    public virtual void OnEndEffecting() {          // Called once the effect stops
        SpellsManager.instance.UnSubscribeFunction(WhileEffecting);
    }
    public void setValue(float v) { value = v; }

}
[System.Serializable]
public class StatValueEffect
{
    [SerializeField] private Stat stat;             // Stat to be modified 
    [SerializeField] private float value;
    [SerializeField] Operation operation;           // Operation to be performed

    public void setOperation(Operation op) { operation = op; }
    public Operation getOperation() { return operation; }
    public Stat getStat() { return stat; }
    public float getValue()
    {
        return value;
    }
}
public enum EffectType
{
    Positive,
    Negative
}
