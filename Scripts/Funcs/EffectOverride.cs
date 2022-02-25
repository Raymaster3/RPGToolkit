using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectOverride
{
    [SerializeField] private Effect effect;
    [SerializeField] private bool Override; // Conditional to show the rest of the properties
    [SerializeField] private float durationOverride;
    [SerializeField] private StatValueEffect[] statsToModifyOverride;


    private void InitializeDfltEffect()
    {
        effect = ScriptableObject.Instantiate(effect);
    }

    public Effect getOverrittenEffect()
    {
        InitializeDfltEffect();
        if (!Override) return effect;
        effect.Duration = durationOverride;
        return effect;
    }
}
