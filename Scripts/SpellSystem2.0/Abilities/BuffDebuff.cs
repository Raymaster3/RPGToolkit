using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "RPGToolkit/Abilities/BuffDebuff", order = 1)]
public class BuffDebuff : Ability
{
    [Header("Buff Properties")]
    [SerializeField] private bool useOnCaster;
    [SerializeField, ConditionalHide("useOnCaster", true)] private bool alliesOnly;
    [SerializeField, ConditionalHide("alliesOnly", true)] private bool enemiesOnly = false;

    private Character curTarget;
    public override void OnFirstCast()
    {
        foreach(Effect e in EffectsToApply)
        {
            e.OnApply(SpellsManager.instance.target);
        }
    }
    protected override void NextCast()
    {
        if (!spendResource()) return;

        curTarget = Caster;     // Default objective

        if (!useOnCaster)
        {
            if (alliesOnly && Caster.Faction.getRealtionStatus(SpellsManager.instance.target.Faction) != RelationType.Enemy) curTarget = SpellsManager.instance.target;
            else
            {
                if (!alliesOnly && enemiesOnly) curTarget = SpellsManager.instance.target;
                else return;
            }
        }

        foreach (Effect e in EffectsToApply)
        {
            if (!e.checkIfValid(Caster, curTarget)) continue;
            e.OnApply(curTarget);
        }
    }
}
