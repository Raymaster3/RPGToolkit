using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CloseToEffect", menuName = "RPGToolkit/Effects/CloseTo", order = 1)]
public class CloseTo : Effect
{
    [SerializeField] private float radius;
    [SerializeField] private Vector3 originPoint;

    public void setPointAndRadius(Vector3 point, float rad)
    {
        radius = rad;
        originPoint = point;
    }
    public override void OnApply(Character trgt)
    {
        Effect eff = Instantiate(this);             // Copy the effect for the player
        eff.Target = trgt;
        ((CloseTo)eff).setPointAndRadius(originPoint, radius);
        trgt.addEffect(eff);
        eff.OnStartEffecting();
    }
    public override void OnStartEffecting()
    {
        base.OnStartEffecting();
    }
    public override void WhileEffecting()
    {
        // Cogeremos y aplicaremos un efecto para todos los personajes que entren en la zona de efecto
        // Una vez entran el efecto se ocupara de su target individualmente
        // Por tanto tenemos dos partes, la primera debera mantener el area de efecto y aplicar el efecto a cada uno que entre
        // Y la segunda parte se encargará de controlar a su target propio que ya tendra asignado el efecto

        if ((Target.transform.position - originPoint).magnitude <= radius)
        {
            // Inside area of effect
            if (!Target.hasEffect(this))
            {
                foreach (StatValueEffect stat in StatsToModify)
                    Target.modifyStatByFactor(stat.getStat(), stat.getValue(), stat.getOperation());
                Target.addEffect(this);
            }
        } else {
            RevertChanges();
        }
        base.WhileEffecting();  
    }
    public override void OnEndEffecting()
    {
        base.OnEndEffecting();
        RevertChanges();
    }
    private void RevertChanges()
    {
        if (Target.hasEffect(this))
        {
            foreach (StatValueEffect stat in StatsToModify)  // We revert the changes 
                Target.revertModifier(stat.getStat(), stat.getValue(), stat.getOperation());
            Target.removeEffect(this);  // And then remove it from the target
        }
    }
}