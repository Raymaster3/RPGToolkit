using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Abilitiy", menuName = "RPGToolkit/Abilities/AOE", order = 1)]
public class AOESpell : Ability
{
    [Header("AOE")]
    [SerializeField] protected bool castOnPlayer;
    [SerializeField, ConditionalHide("castOnPlayer")] protected bool followPlayer;
    [SerializeField, ConditionalHide("castOnPlayer", true)] protected bool followTarget;
    [SerializeField] private float radius;
    [SerializeField] List<EffectOverride> effectsBeforeDelay;
    [Header("Delay Effect")]
    [SerializeField] private bool delayedActivation;
    [SerializeField, ConditionalHide("delayedActivation")] private float delayTime;
    [SerializeField, ConditionalHide("delayedActivation")] private float yOffset;

    private float timer = 0;
    private Vector3 point;
    private List<Character> afectedChars;

    public bool DelayedActivation { get => delayedActivation; }
    public float DelayTime { get => delayTime; }
    

    public bool debug;

    protected override void OnStartAiming()
    {
        base.OnStartAiming();
        //SpellsManager.ResizeGameObject(indicator, radius);
        //resizeVisual(indicator);
        SpellsManager.ResizeGameObject(indicator, radius);
    }

    protected override void OnEndAim()
    {
        base.OnEndAim();
        point = SpellsManager.getMouseWorldPos();
    }
    protected override void NextCast()
    {
        if (spendResource())            // Only execute if it was posible to spend the resources
        {
            if (debug)
            {
                SpellsManager.instance.setUpGizmosSphere(point, radius);
            }
            Vector3 targetPos = castOnPlayer ? Player.instance.transform.position : point + new Vector3(0, yOffset, 0);
            GameObject go = Instantiate(Prefab, targetPos, Quaternion.identity);

            if (followPlayer) go.AddComponent<SimpleFollow>().target = Player.instance.gameObject;
            if (followTarget) go.AddComponent<SimpleFollow>().target = SpellsManager.instance.target.gameObject;

            go.AddComponent<Destroyer>().lifeTime = LifeTime;
            go.GetComponent<Destroyer>().OnDestroy = () => {
                SoundManager.instance.StopWorldSound(go, 1);
            };
            SoundManager.instance.PlayAbilitySound(go, activeSound, true);
            // We have to resize the indicator 
            resizeVisual(go);
            if (!delayedActivation) delayTime = 0;
            SpellsManager.instance.SubscribeFunction(SubscribeEffectForDelay);
        }
    }
    protected override void resizeVisual(GameObject go)
    {
        SpellsManager.ResizeGameObject(go, radius);
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
        if (followPlayer) m_point = Player.instance.transform.position; // Update position accordingly
        if (followTarget) m_point = SpellsManager.instance.target.transform.position;

        RaycastHit[] hits = Physics.SphereCastAll(m_point, radius, new Vector3(0, 1, 0));
        foreach (RaycastHit hit in hits)
        {
            Character charac = hit.collider.GetComponent<Character>();
            if (charac == null) continue;
            foreach (Effect e in EffectsToApply)
            {
                if (e.checkIfValid(Caster, charac))
                    e.OnApply(charac);
            }
            if (charac.Faction.getRealtionStatus(Caster.Faction) == RelationType.Friendly) continue;
            if (DamageType.canDamageTarget(charac, Caster))
            {
                DamageType.damageTarget(charac, Caster, DamageQuantity);
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
    protected virtual void WhileWaiting()
    {
        if (followPlayer) point = Player.instance.transform.position; // Update position accordingly
        if (followTarget) point = SpellsManager.instance.target.transform.position;

        if (afectedChars == null) afectedChars = new List<Character>();
        RaycastHit[] hits = Physics.SphereCastAll(point, radius, new Vector3(0, 1, 0));
        foreach (RaycastHit hit in hits)
        {
            Character c = hit.collider.GetComponent<Character>();
            if (c == null || afectedChars.Contains(c)) continue;
            if (c.Faction.getRealtionStatus(Caster.Faction) == RelationType.Friendly) continue;
            afectedChars.Add(c);
            foreach (EffectOverride eo in effectsBeforeDelay)
            {
                Effect e = eo.getOverrittenEffect();
                if (!e.checkIfValid(Caster, c)) continue;
                if (e.GetType() == typeof(CloseTo))
                {
                    ((CloseTo)e).setPointAndRadius(point, radius);
                }
                e.OnApply(c);
            }
        }
    }
}
