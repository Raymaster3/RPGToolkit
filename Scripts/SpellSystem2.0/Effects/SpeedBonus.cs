using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatBonus", menuName = "RPGToolkit/Effects/StatBuff", order = 1)]
public class SpeedBonus : Effect
{
    public override void OnEndEffecting()
    {
        base.OnEndEffecting();
        foreach (StatValueEffect stat in StatsToModify)
        {
            Target.revertModifier(stat.getStat(), stat.getValue(), stat.getOperation());
            // Target.revertModifier(StatToModify, Value, Operation); // Testing
        }
        Target.removeEffect(this);
    }
}
