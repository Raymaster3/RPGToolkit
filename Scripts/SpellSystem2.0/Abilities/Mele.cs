using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mele", menuName = "RPGToolkit/Abilities/Mele", order = 1)]
public class Mele : Ability
{
    [Header("Melee Properties")]
    [SerializeField] private float minAngle, maxAngle;
    [SerializeField] private float minRange, maxRange;
    [SerializeField, ConditionalHide("needsAim", true)] private bool faceTarget = false;

    private bool InKillzone(Vector3 position, out bool outOfReach)
    {
        outOfReach = false;
        Vector3 direction = position - Caster.transform.position;
        float distance = direction.magnitude;
        if (distance <= minRange || distance >= maxRange) { outOfReach = true; return false; }
        float angle = Vector3.Angle(direction, Caster.getCharacterForward());
        if (angle <= minAngle || angle >= maxAngle) return false;
        return true;
    }

    protected override void NextCast()
    {
        bool outOfReach = false;
        if (!needsTarget)
        {
            Vector3 targetPos = Caster.transform.position + Caster.getCharacterForward() * 3;
            if (needsAim)
            {
                targetPos = SpellsManager.getMouseWorldPos();
                Caster.LookAtTarget(targetPos);
            }

            GameObject go = Instantiate(Prefab, Caster.transform.position, Quaternion.identity);
            go.transform.LookAt(targetPos);
            go.AddComponent<Destroyer>().lifeTime = LifeTime;

            RaycastHit[] targets = Physics.SphereCastAll(Caster.transform.position, maxRange, Caster.transform.up);
            foreach (RaycastHit hit in targets)
            {
                Character charac = hit.collider.GetComponent<Character>();
                if (charac == null) continue;
                if (InKillzone(hit.collider.transform.position, out outOfReach)) Damage(charac);
            }
        }else
        {
            if (SpellsManager.instance.target == null)
            {
                UIManager.instance.ShowMessage("No target", Color.red, ErrorDuration.Short);
                return;
            }
            if (!InKillzone(SpellsManager.instance.target.transform.position, out outOfReach)) 
            {
                if (!faceTarget || outOfReach) 
                {
                    UIManager.instance.ShowMessage("Enemy out of sight", Color.yellow, ErrorDuration.Short);
                    return;
                }
                Caster.LookAtTarget(SpellsManager.instance.target.transform);
            }
            Damage(SpellsManager.instance.target);
        }
    }

    private void Damage(Character c)
    {
        // Idea! We do it here because we need to check for each one if its meant for allies or enemies
        // For that we would need to make a distinction between positive and negative effects

        // Apply effects as well
        foreach (Effect effect in EffectsToApply)
        {
            if (effect.checkIfValid(Caster, c))
                effect.OnApply(c);
        }

        if (DamageType.canDamageTarget(c, Caster)) {
            DamageType.damageTarget(c, Caster, DamageQuantity);     
       }
    }
    protected override void Aiming()
    {
    }
    protected override void OnEndAim()
    {
    }
    protected override void OnStartAiming()
    {
    }
    protected override bool OnActivate()
    {
        bool outofReach;
        if (needsTarget && SpellsManager.instance.target != null && !InKillzone(SpellsManager.instance.target.transform.position, out outofReach))
        {
            UIManager.instance.ShowMessage("Out of sight!", Color.yellow, ErrorDuration.Short);
            return false;
        }
        return true;
    }
    public override bool FromAimToChannelCond()
    {
        return true;
    }
}