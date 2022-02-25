using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SquareAOE", menuName = "RPGToolkit/Abilities/SquareAOE", order = 1)]
public class SquareAOE : AOESpell
{
    [Header("BoxAOE")]
    [SerializeField] private float xSize;
    [SerializeField] private float zSize;
    [SerializeField] private bool pivotCentered;
    [SerializeField] private float xPos, zPos;

    public override void OnSecondCast()
    {
        base.OnSecondCast();

        if (debug)
        {
            //SpellsManager.instance.setUpGizmosCube(SpellsManager.getMouseWorldPos(), new Vector3(xSize, xSize, zSize));
            //SpellsManager.instance.resetGizmos();
        }

        /*foreach(EffectValue e in Effects)
        {
            e.Initialize();
            e.Effect.OnApply(SpellsManager.instance.target);
        }*/
    }
    public override void BehaviourAfterDelay(Vector3 m_point)
    {
        RaycastHit[] hits = Physics.BoxCastAll(m_point, new Vector3(xSize, xSize, zSize), new Vector3(0, 1, 0));
        foreach (RaycastHit hit in hits)
        {
            Character charac = hit.collider.GetComponent<Character>();
            if (charac != null)
            {
                foreach (Effect e in EffectsToApply)
                    e.OnApply(charac);
                if (DamageType.canDamageTarget(charac, Caster))
                {
                    DamageType.damageTarget(charac, Caster, DamageQuantity);
                }
            }
        }
    }

}
