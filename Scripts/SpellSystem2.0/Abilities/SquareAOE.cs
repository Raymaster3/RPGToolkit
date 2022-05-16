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

    protected override void NextCast()
    {
        base.NextCast();
        if (debug)
        {
            SpellsManager.instance.resetGizmos();
            SpellsManager.instance.setUpGizmosCube(Point /*+ new Vector3(xSize/2, 0, zSize/2)*/, new Vector3(xSize, 0, zSize));
        }
    }
    protected override void resizeVisual(GameObject go)
    {
        float radius = Mathf.Sqrt(xSize*xSize + zSize*zSize);
        SpellsManager.ResizeGameObject(go, 0, false, xSize, zSize);
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
