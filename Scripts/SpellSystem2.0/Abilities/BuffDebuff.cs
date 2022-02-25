using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "RPGToolkit/Abilities/BuffDebuff", order = 1)]
public class BuffDebuff : Ability
{
    public override void OnFirstCast()
    {
        foreach(Effect e in EffectsToApply)
        {
            e.OnApply(SpellsManager.instance.target);
        }
    }
}
