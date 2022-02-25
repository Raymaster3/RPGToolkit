using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Abilitiy", menuName = "Abilities/Basic", order = 1)]
public class Ability : RPGObject
{
    [HideInInspector] public int index;

    [Header("Ability Properties")]
    //[SerializeField] private string abilityName;
    [SerializeField] List<Effect> effectsToApply;
    [Header("Damage")]
    [SerializeField] Damage damageType;
    [SerializeField] float damageQuantity;
    [Header("Lifetime")]
    [SerializeField] float lifeTime;
    [Header("Resources")]
    [SerializeField] private Pair[] resources;
    [Header("Times")]
    [SerializeField] private float coolDown;
    [SerializeField] private float channelTime;
    [Header("Control")]
    [SerializeField] private bool needsAim;
    [SerializeField] private int numberOfCasts = 1;
    //[SerializeField] List<EffectValue> effects;

    private Character caster;
    private Vector3 castPoint;
    private bool canCast = true;
    private float coolDownTimer = 0;
    private float casts = 0;
    protected AbilityState state = AbilityState.Idle;
    protected GameObject indicator;

    [Header("Objects")]
    [SerializeField] GameObject prefab;
    [SerializeField] GameObject aimIndicator;
    

    public GameObject AimIndicator { get => aimIndicator; }
    public GameObject Prefab { get => prefab; }
    public List<Effect> EffectsToApply { get => effectsToApply; }
    public Damage DamageType { get => damageType; }
    public float DamageQuantity { get => damageQuantity; }
    public float LifeTime { get => lifeTime; }
    public float CoolDown { get => coolDown; }

    public Character Caster { get => caster; }
    public Vector3 Point { get => castPoint; }
    //public List<EffectValue> Effects{ get => effects; }

    

    public virtual void Cast(Character cster, Vector3 position)
    {
        if (!canCast) return;
        caster = cster;
        castPoint = position;

        if (state == AbilityState.Idle)
            Activate();
        else if (state == AbilityState.Waiting)
        {
            state = AbilityState.MultiCasting;
        }
    }
    // Called once when the skill is casted 
    private void Activate()
    {
        // Do some starting set up. 
        OnActivate();
        // Then we need to check if we need to aim this ability
        if (needsAim) SetUpAiming();
        else SetUpChanneling();
    }
    protected virtual void OnActivate()
    {

    }
    // Callback that gets executed each frame while the ability is aiming
    private void AimingBehaviour()
    {
        Aiming();
        if (FromAimToChannelCond()) { SpellsManager.instance.UnSubscribeFunction(AimingBehaviour); OnEndAim(); SetUpChanneling();  } // BasicBehaviour
        if (AimCancelCondition()) { SpellsManager.instance.UnSubscribeFunction(AimingBehaviour); OnEndAim(); state = AbilityState.Idle; }
    }
    protected virtual void Aiming() 
    {
        indicator.transform.position = SpellsManager.getMouseWorldPos() + new Vector3(0, 0.5f, 0);
    }


    private void NextCastBeh()
    {
        state = AbilityState.Waiting;
        UIManager.instance.UnBlockAbility(index);
        NextCast();
        casts++;
        if (casts < numberOfCasts)
        {
            SpellsManager.instance.SubscribeFunction(WaitForActivation);
        }
        else
        {
            casts = 0;
            SetUpCooldown();
        }
    }
    private void WaitForActivation()
    {
        if (state == AbilityState.Waiting) return;
        state = AbilityState.MultiCasting;
        if (needsAim) SetUpAiming();
        else SetUpChanneling();
        SpellsManager.instance.UnSubscribeFunction(WaitForActivation);
    }
    protected virtual void NextCast()
    {

    }

    protected virtual void OnStartCooldown()
    {

    }
    protected void SetUpCooldown()
    {
        state = AbilityState.CoolDown;
        OnStartCooldown();
        Action act = null;
        act = () =>
        {
            CoolingDown();
            coolDownTimer += Time.deltaTime;
            if (coolDownTimer >= coolDown)
            {
                coolDownTimer = 0;
                // End
                state = AbilityState.Idle;
                OnEndCooldown();
                SpellsManager.instance.UnSubscribeFunction(act);
            }
        };
        SpellsManager.instance.SubscribeFunction(act);
    }
    protected virtual void CoolingDown()
    {
        UIManager.instance.updateCoolDownUI(index, coolDownTimer);
    }
    protected virtual void OnEndCooldown()
    {

    }
    private void SetUpAiming()
    {
        state = AbilityState.Aiming;
        UIManager.instance.BlockAbility(index);
        //SpellsManager.instance.BlockAbilities(caster);
        OnStartAiming();
        SpellsManager.instance.SubscribeFunction(AimingBehaviour);
    }
    protected virtual void OnStartAiming()
    {
        indicator = Instantiate(AimIndicator, SpellsManager.getMouseWorldPos(), Quaternion.identity, null);
    }
    protected virtual void OnEndAim()
    {
        Destroy(indicator);
    }
    // Callback that gets called each frame while the ability is channeling
    protected virtual void Channeling(float progress)
    {
        UIManager.instance.UpdateChannelSlider(progress, channelTime);
    }
    private void SetUpChanneling()
    {
        // Set Up
        state = AbilityState.Channeling;
        //UIManager.instance.BlockAbility(index);
        SpellsManager.instance.BlockAbilities(caster);
        float localTimer = 0;
        Action act = null; 
        // Annonymous delegate that serves as a timer
        act = () => {
            localTimer += Time.deltaTime;
            if (AimCancelCondition()) { SpellsManager.instance.UnSubscribeFunction(act); UIManager.instance.HideChannelUI(); return; /* Aqui podriamos llamar a un callback de Cancel */ }
            if (localTimer >= channelTime) {
                localTimer = 0;
                SpellsManager.instance.UnSubscribeFunction(act);
                SpellsManager.instance.UnBlockAbilities(caster);
                UIManager.instance.HideChannelUI();
                Signal sig = new Signal(NextCastBeh, 1);
                NetworkManager.instance.SendSignal(sig);
                return;
            }
            Channeling(localTimer);
        }; 
        // Finally we subscribe the delegate to be executed each frame
        SpellsManager.instance.SubscribeFunction(act);
    }

    #region Control Conditions
    // We can use this to know at a certain time if we can cancel this ability
    public virtual bool AimCancelCondition()
    {
        return Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape);
    }
    public virtual bool FromAimToChannelCond() // Returns true if we can stop aiming and start channeling
    {
        return Input.GetMouseButtonDown(0);
    }
    public virtual bool InterruptCondition()
    {
        return false; // Can't be interrupted for now
    }
    #endregion

    // Previous version

    public virtual void OnFirstCast() {
    }
    /// <summary>
    /// Called every frame until the ability is casted again
    /// </summary>
    public virtual void WhileAim() { }
    public virtual void OnSecondCast() {
    }
    public void Cancel()
    {

    }
    public void resetAbility()
    {
    }
    private void CoolDownTimer()
    {
        coolDownTimer += Time.deltaTime;
        if (coolDownTimer >= coolDown)
        {
            coolDownTimer = 0;
            // Available again
            canCast = true;
            SpellsManager.instance.UnSubscribeFunction(CoolDownTimer);
            return;
        }
       UIManager.instance.updateCoolDownUI(index, coolDownTimer);
    }

    protected bool spendResource() // Returns true if it could spend the resources
    {
        foreach (Pair p in resources)
        {
            if (caster.hasResources(p.First, p.Second))
            {
                caster.useResource(p.First, p.Second);
                return true; 
            }
        }
        return false;
    }
    public AbilityState getState()
    {
        return state;
    }
    public void setState(AbilityState s)
    {
        state = s;
    }
}
[System.Serializable]
public class EffectValue {
    [SerializeField] private Effect effect;
    [SerializeField] private float value;
    [SerializeField] private Operation operation;

    public EffectValue ()
    {
        
    }

    public void Initialize()
    {
        effect.setValue(value);
        effect.Operation = operation;
    }

    public Effect Effect { get => effect; }
    public float Value { get => value; }
    public Operation Operation { get => operation; }
}
[System.Serializable]
public class Pair
{
    [SerializeField] Stat stat;
    [SerializeField] float value;

    public Stat First { get => stat; }
    public float Second { get => value; }
}
public enum AbilityState
{
    Idle,
    Aiming,
    Channeling,
    CoolDown, 
    MultiCasting,
    Waiting, 
    Blocked
}
