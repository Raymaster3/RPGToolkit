using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Mock", menuName = "Abilities/MockAbility", order = 1)]
public class Ability : RPGObject
{
    [Header("Ability Properties")]
    //[SerializeField] private string abilityName;
    [SerializeField] List<Effect> effectsToApply;
    [SerializeField] private bool mockAbility;
    [SerializeField] protected bool blockMovement;
    [Header("Damage")]
    [SerializeField] Damage damageType;
    [SerializeField] float damageQuantity;
    [Header("Lifetime")]
    [SerializeField] float lifeTime;
    [Header("Resources")]
    [SerializeField] private Pair[] resources;
    [Header("Times")]
    [SerializeField] private float coolDown;
    [SerializeField] protected float channelTime;
    [SerializeField] protected float castAnimSpeed = 1;
    [Header("Control")]
    [SerializeField] protected bool needsAim;
    [SerializeField, ConditionalHide("needsAim", true)] protected bool needsTarget;
    [SerializeField] private int numberOfCasts = 1;
    [Header("Animation")]
    // Alternative
    [SerializeField] protected AnimationClip channelAnimation;
    [SerializeField] protected AnimationClip castAnimation;
    [SerializeField, ConditionalHide("channelAnimation")] protected bool loopChannelAnim = false;
    [SerializeField, ConditionalHide("castAnimation")] protected bool loopCastAnim = false;
    [Header("Sounds")]
    [SerializeField] protected AudioClip channelSound;
    [SerializeField] protected AudioClip castSound;
    [SerializeField] protected AudioClip activeSound;
    [Header("Scene Effects")]
    [SerializeField] protected SceneEffect channelEffects;
    [SerializeField] protected SceneEffect castEffects;

    private Character caster;
    private Vector3 castPoint;
    private bool canCast = true;
    private float coolDownTimer = 0;
    private float casts = 0;
    protected AbilityState state = AbilityState.Idle;
    protected GameObject indicator;
    private Action doBefore;

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
    public bool Mock { get => mockAbility; set => mockAbility = value; }

    public Character Caster { get => caster; set => caster = value;  }
    public Vector3 Point { get => castPoint; }

    public override string Description { get => getDescription(); }
    public override string getDescription()
    {
        if (damageType != null)
        {
            string colorInHex = ColorUtility.ToHtmlStringRGBA(damageType.DamageColor);
            return description.Replace("{d}", "<color=#" + colorInHex + ">" + damageType.calculateDamage(Player.instance, DamageQuantity) + "</color>");
        }
        return description;
    }
    public override void Swap(RPGObject with, Character owner = null, ushort ind = 0)
    {
        // We check if the activator is an item
        if (with.GetMyType() == "Item") // We drag the ability to the inventory
        {
            // For now we dont want to do anything here
            /*if (!((Item)with).hasAbility()) return;
            caster.Abilities[index] = with;*/ 
            return;
        }
        int temp = with.index;
        owner.Abilities[index] = with;
        owner.Abilities[temp] = this;
        with.index = index;
        index = temp;
    }
    public override string GetMyType()
    {
        return "Ability";
    }
    protected virtual void resizeVisual(GameObject go)
    {
        //SpellsManager.ResizeGameObject(go, radius);
    }
    public void setBeforeAction(Action action) { doBefore = action; }

    public override void Cast(Character cster, Vector3 position)
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
        if (!OnActivate()) return;

        #region Initial Checking
        if (needsTarget && SpellsManager.instance.target == null)
        {
            UIManager.instance.ShowMessage("No target", Color.red, ErrorDuration.Short);
            return;
        }
        if ((resources != null && resources.Length > 0))
        {
            if (!caster.hasResources(resources[0].First, resources[0].Second))
            {
                UIManager.instance.ShowMessage("Not enough resources", Color.red, ErrorDuration.Short);
                return;
            }
        }
        #endregion

        // Then we need to check if we need to aim this ability
        if (needsAim) SetUpAiming();
        else SetUpChanneling();
    }
    protected virtual bool OnActivate()
    {
        return true;
    }
    // Callback that gets executed each frame while the ability is aiming



    private void NextCastBeh()
    {
        state = AbilityState.Waiting;
        UIManager.instance.UnBlockAbility(index);
        doBefore?.Invoke();

        if (castAnimation != null)
            AnimationsManager.instance.PlaySkillAnim(Caster, "SkillsAnimBaseLayer", castAnimation, castAnimSpeed);
        if (castSound != null)
            SoundManager.instance.PlaySoundOnPlayer(castSound);

        ApplySceneEffects();    // Camera shake and such
        NextCast();

        casts++;
        if (casts < numberOfCasts)
            SpellsManager.instance.SubscribeFunction(WaitForActivation);
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
    protected virtual void OnStartCooldown()
    {

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
    private void AimingBehaviour()
    {
        Aiming();
        if (FromAimToChannelCond()) { SpellsManager.instance.UnSubscribeFunction(AimingBehaviour); OnEndAim(); SetUpChanneling(); } // BasicBehaviour
        if (AimCancelCondition()) { SpellsManager.instance.UnSubscribeFunction(AimingBehaviour); OnEndAim(); state = AbilityState.Idle; UIManager.instance.UnBlockAbility(index); }
    }
    protected virtual void Aiming()
    {
        indicator.transform.position = SpellsManager.getMouseWorldPos() + new Vector3(0, 0.5f, 0);
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
    protected virtual void OnStartChanneling()
    {
        if (channelAnimation != null)
            AnimationsManager.instance.PlaySkillAnim(Caster, "SkillsAnimBaseLayer", channelAnimation, channelAnimation.length / channelTime, loopChannelAnim);
        if (channelSound != null)
        {
            SoundManager.instance.PlaySoundOnPlayer(channelSound, channelTime);
        }
    }
    private void SetUpChanneling()
    {
        // Set Up
        state = AbilityState.Channeling;
        SpellsManager.instance.BlockAbilities(caster);
        float localTimer = 0;

        OnStartChanneling();
        channelEffects?.StartEffect();

        if (blockMovement)
        {
            caster.StopMoving();
            caster.BlockMovement();
        }

        // Annonymous delegate that serves as a timer
        Action act = null;
        act = () => {
            localTimer += Time.deltaTime;
            if (AimCancelCondition()) { SpellsManager.instance.UnSubscribeFunction(act); CancelChannel(); return; /* Aqui podriamos llamar a un callback de Cancel */ }
            if (localTimer >= channelTime) {
                localTimer = 0;
                SpellsManager.instance.UnSubscribeFunction(act);
                SpellsManager.instance.UnBlockAbilities(caster);
                UIManager.instance.HideChannelUI();
                NextCastBeh();
                OnEndChanneling();
                channelEffects?.Stop();
                if (blockMovement) caster.UnBlockMovement();
                return;
            }
            Channeling(localTimer);
        }; 
        // Finally we subscribe the delegate to be executed each frame
        SpellsManager.instance.SubscribeFunction(act);
    }
    protected virtual void OnEndChanneling()
    {
        AnimationsManager.instance.StopLoopAnim(Caster);
    }
    private void CancelChannel()
    {
        UIManager.instance.HideChannelUI();
        SoundManager.instance.StopWorldSound(Player.instance.gameObject, 0.2f);
        SpellsManager.instance.UnBlockAbilities(caster);
        state = AbilityState.Idle;
        OnEndChanneling();
    }


    #region Control Conditions
    // We can use this to know at a certain time if we can cancel this ability
    public virtual bool AimCancelCondition()
    {
        return /*Input.GetMouseButtonDown(1) || */ Raycaster.instance.escape;
    }
    public virtual bool FromAimToChannelCond() // Returns true if we can stop aiming and start channeling
    {
        return Raycaster.instance.leftClick;
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
        if (resources.Length == 0) return true;
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
    protected void ApplySceneEffects()
    {
        castEffects?.StartEffect();
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
