using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDamage", menuName = "RPGToolkit/Damage", order = 1)]
public class Damage : ScriptableObject
{
    [SerializeField] private string damageName;
    [SerializeField] private string description;
    [SerializeField] private Color damageColor;         // For damage texts and descriptions

    [SerializeField] private Stat health;               // Which stat is being modified 
                                                        // With this we can make interesting variations like a damage that breaks armors    
                                                        // or one that consumes resources
    [SerializeField] private StatOperation[] weakneses;
    [SerializeField] private StatOperation[] strengths;

    public Color DamageColor { get => damageColor; }

    public bool canDamageTarget(Character target, Character caster)
    {
        RelationType rt = caster.Faction.getRealtionStatus(target.Faction);
        return (rt == RelationType.Neutral || rt == RelationType.Enemy);
    }
    public void damageTarget(Character target, Character caster, float amount)
    {
        if (!target.isInteractable()) return;
        float realDamage = amount;
        foreach(StatOperation statOp in strengths)
        {
            StatValue stat = statOp.FromCaster ? caster.getStatByName(statOp.Stat.getName()) : target.getStatByName(statOp.Stat.getName()); 
            if (stat != null)                                              
            {
                float strenghtValue = stat.getValue();
                switch (statOp.Operation)
                {
                    case Operation.Add:
                        realDamage += strenghtValue*statOp.Multiplier; 
                        break;
                    default:
                        realDamage *= (1 + strenghtValue);        // Suponemos valor entre 0 y 1, donde son porcentajes de mejora
                        break;
                }
            }
        }
        foreach (StatOperation statOp in weakneses)
        {
            StatValue stat = statOp.FromCaster ? caster.getStatByName(statOp.Stat.getName()) : target.getStatByName(statOp.Stat.getName());
            if (stat != null)
            {
                float weaknessValue = stat.getValue();
                switch (statOp.Operation)
                {
                    case Operation.Add:
                        realDamage -= weaknessValue;
                        break;
                    default:
                        realDamage *= (1 - weaknessValue);

                        break;
                }
            }
        }
        UIManager.instance.displayDamage(realDamage, damageColor, target.transform.position);
        StatValue hp = target.getStatByName(health.getName());
        target.modifyStatByFactor(hp.getStat(), -realDamage, Operation.Add);
        target.Hit();
        //hp.BoostByFactor(-realDamage, Operation.Add);
        if (hp.getValue() <= 0)
        {
            // Kill target
            target.Die();
        }
        //target.getStatByName(health.getName()).BoostByFactor(-realDamage, Operation.Add);       // Modify health 

    }
    public float calculateDamage(Character caster,float amount)
    {
        float realDamage = amount;
        foreach (StatOperation statOp in strengths)
        {
            StatValue stat;
            if (statOp.FromCaster)
                stat = caster.getStatByName(statOp.Stat.getName());
            else continue;
            if (stat != null)
            {
                float strenghtValue = stat.getValue();
                switch (statOp.Operation)
                {
                    case Operation.Add:
                        realDamage += strenghtValue * statOp.Multiplier;
                        break;
                    default:
                        realDamage *= (1 + strenghtValue);        // Suponemos valor entre 0 y 1, donde son porcentajes de mejora
                        break;
                }
            }
        }
        foreach (StatOperation statOp in weakneses)
        {
            StatValue stat;
            if (statOp.FromCaster)
                stat = caster.getStatByName(statOp.Stat.getName());
            else continue; 
            if (stat != null)
            {
                float weaknessValue = stat.getValue();
                switch (statOp.Operation)
                {
                    case Operation.Add:
                        realDamage -= weaknessValue;
                        break;
                    default:
                        realDamage *= (1 - weaknessValue);

                        break;
                }
            }
        }
        return realDamage;
    }

}
[System.Serializable]
public class StatOperation
{
    [SerializeField] private Stat stat;
    [SerializeField] private Operation operation;
    [SerializeField] private float multiplier = 1;          // To better control the stat modifying the damage
    [SerializeField, Tooltip("Use this stat from the target or caster")] 
    private bool fromCaster;                                // Should we consider this stat from the caster of the target

    public Stat Stat { get => stat; }
    public Operation Operation { get => operation; }

    public float Multiplier { get => multiplier; }
    public bool FromCaster { get => fromCaster; }
}
