using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PerSecond", menuName = "RPGToolkit/Effects/PerSecond", order = 1)]
public class PerSecond : Effect
{
    [SerializeField] private float timeInterval;
    private float m_timer = 0;

    public override void OnStartEffecting()
    {
        SpellsManager.instance.SubscribeFunction(WhileEffecting);
    }

    public override void WhileEffecting()
    {
        m_timer += Time.deltaTime;
        if (m_timer >= timeInterval)
        {
            m_timer = 0;
            // Effect
            foreach (StatValueEffect stat in StatsToModify)
            {
                Target.modifyStatByFactor(stat.getStat(), stat.getValue(), stat.getOperation());
                // Target.modifyStatByFactor(StatToModify, Value, Operation); // Testing yet
            }
        }
        base.WhileEffecting();
    }
    public override void OnEndEffecting()
    {
        base.OnEndEffecting();
        Target.removeEffect(this);
    }
}
