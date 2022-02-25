using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PermanentEffect", menuName = "RPGToolkit/Effects/Permanent", order = 1), System.Serializable]
public class PermanentEffect : Effect
{
    public override void OnEndEffecting()
    {
        base.OnEndEffecting();
        Target.removeEffect(this);
    }
}
