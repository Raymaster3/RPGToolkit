using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Abilitiy", menuName = "RPGToolkit/Abilities/AOE", order = 1)]
public class AOESpell : Ability
{
    [Header("AOE")]
    [SerializeField] private float radius;
    [SerializeField] List<EffectOverride> effectsBeforeDelay;
    [Header("Delay Effect")]
    [SerializeField] private bool delayedActivation;
    [SerializeField] private float delayTime;
    [SerializeField] private float yOffset;

    private float timer = 0;
    private Vector3 point;
    private List<Character> afectedChars;

    public bool DelayedActivation { get => delayedActivation; }
    public float DelayTime { get => delayTime; }
    

    public bool debug;

    protected override void OnEndAim()
    {
        base.OnEndAim();
        point = SpellsManager.getMouseWorldPos();
    }
    protected override void NextCast()
    {
        if (spendResource())            // Only execute if it was posible to spend the resources
        {
            GameObject go = Instantiate(Prefab, point + new Vector3(0, yOffset, 0), Quaternion.identity);
            go.AddComponent<Destroyer>().lifeTime = LifeTime;

            SpellsManager.instance.resetGizmos();
            if (!delayedActivation) delayTime = 0;
            SpellsManager.instance.SubscribeFunction(SubscribeEffectForDelay);
        }
    }

    public override void OnSecondCast()
    {
        point = SpellsManager.getMouseWorldPos();

        base.OnSecondCast();

        if (spendResource())            // Only execute if it was posible to spend the resources
        {
            if (Caster.GetType() == typeof(Player))
            {
                Destroy(indicator);
                GameObject go = Instantiate(Prefab, point + new Vector3(0, yOffset, 0), Quaternion.identity);
                go.AddComponent<Destroyer>().lifeTime = LifeTime;
            }
            else Instantiate(Prefab, Point + new Vector3(0, yOffset, 0), Quaternion.identity);

            SpellsManager.instance.resetGizmos();
            if (!delayedActivation) delayTime = 0;

            SpellsManager.instance.SubscribeFunction(SubscribeEffectForDelay);
        }else {
            if (Caster.GetType() == typeof(Player)) Destroy(indicator);
        }

        /*foreach(EffectValue e in Effects)
        {
            e.Initialize();
            e.Effect.OnApply(SpellsManager.instance.target);
        }*/
    }
    public virtual void BehaviourAfterDelay(Vector3 m_point)
    {
        RaycastHit[] hits = Physics.SphereCastAll(m_point, radius, new Vector3(0, 1, 0));
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
    public void SubscribeEffectForDelay()
    {
        timer += Time.deltaTime;
        if (timer >= delayTime)
        {
            timer = 0;
            afectedChars = new List<Character>();
            BehaviourAfterDelay(point);
            SpellsManager.instance.UnSubscribeFunction(SubscribeEffectForDelay);
            return;
        }
        WhileWaiting();
    }
    private void WhileWaiting()
    {
        if (afectedChars == null) afectedChars = new List<Character>();
        RaycastHit[] hits = Physics.SphereCastAll(point, radius, new Vector3(0, 1, 0));
        foreach (RaycastHit hit in hits)
        {
            Character c = hit.collider.GetComponent<Character>();
            if (c == null || afectedChars.Contains(c)) continue;
            afectedChars.Add(c);
            foreach (EffectOverride eo in effectsBeforeDelay)
            {
                Effect e = eo.getOverrittenEffect();
                if (e.GetType() == typeof(CloseTo))
                {
                    ((CloseTo)e).setPointAndRadius(point, radius);
                }
                e.OnApply(c);
            }
        }
    }
}
