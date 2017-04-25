using MovementEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerMod : Mod {

    #region Public fields
    public IMovable WielderMovable { get { return wielderMovable; } }
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] private Hook hook;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpForceMultiplier;
    [SerializeField] private float maxHookLengthStandard;
    [SerializeField] private float maxHookLengthCharged;    
    [SerializeField]
    private float tornadoSpeed;
    [SerializeField]
    private float standardTornadoFallingForce;
    [SerializeField]
    private float chargedTornadoFallingForce;
    [SerializeField]
    private float tornadoDamageBoxWidth;
    #endregion

    #region Private Fields
    private bool hitTargetThisShot;
    private bool tornadoMode;
    private float angle;
    private bool isCharged;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        type = ModType.Grappler;
        category = ModCategory.Ranged;
        angle = 0f;
        isCharged = false;
    }

    // Update is called once per frame
    protected override void Update () {
        if (wielderMovable != null)
        {
            if (getModSpot() != ModSpot.Legs)
            {
                Vector3 forward = wielderMovable.GetForward().normalized;
                forward.y = 0f;
                transform.forward = forward;
            }
        }        
	}
    #endregion

    #region Public Methods
    public override void Activate(Action onCompleteCoolDown=null, Action onActivate=null)
    {
        if (!hook.IsThrown() && !energySettings.isCoolingDown)
        {
            hook.maxLength = IsCharged() ? maxHookLengthCharged : maxHookLengthStandard;
            if (getModSpot() != ModSpot.Legs)
            {
                onActivate = () => { SFXManager.Instance.Play(SFXType.GrapplingGun_Shoot, transform.position); };
            }
        }
        base.Activate(onCompleteCoolDown, onActivate);
    }    

    protected override void BeginChargingArms(){ }
    protected override void RunChargingArms(){ }
    
    protected override void ActivateStandardArms(){ hook.Throw(false); }
    protected override void ActivateChargedArms(){ hook.Throw(true); }

    protected override void BeginChargingLegs(){ }
    protected override void RunChargingLegs(){ }
    protected override void ActivateStandardLegs(){
        Jump();
        Tornado();
    }    

    protected override void ActivateChargedLegs(){        
        Jump();
        isCharged = true;
        Tornado();
    }

    private void Jump() {
        if (wielderMovable.IsGrounded()) {
            isCharged = false;
            float force = energySettings.IsCharged ? jumpForce * jumpForceMultiplier : jumpForce;
            wielderMovable.AddDecayingForce(Vector3.up * force);
        }
    }

    private void Tornado()
    {
        if (!wielderMovable.IsGrounded() && !tornadoMode)
        {            
            Timing.RunCoroutine(StartTornado(),Segment.Update);
        }
    }

    private IEnumerator<float> StartTornado()
    {
        tornadoMode = true;
        hook.ExtendHook();
        while (!wielderMovable.IsGrounded())
        {            
            Vector3 forward = wielderMovable.GetForward();
            forward.y = 0f;
            transform.forward = forward;
            angle += tornadoSpeed;            
            wielderStats.gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);            
            float force = isCharged ? chargedTornadoFallingForce : standardTornadoFallingForce;
            wielderMovable.AddDecayingForce(Vector3.up * force);
            DamageEnemies();
            yield return 0;
        }
        angle = 0f;
        hook.RetractHook();
        transform.forward = Vector3.down;
        isCharged = false;
        tornadoMode = false;
        yield return 0;
    }

    private void DamageEnemies()
    {
        RaycastHit hit;
        if(Physics.BoxCast(transform.position, new Vector3(tornadoDamageBoxWidth, 0f, hook.GetMaxLength()), transform.forward, out hit, Quaternion.identity, hook.GetMaxLength()*2, LayerMask.GetMask(Strings.Tags.ENEMY)))
        {
            IDamageable damageable = hit.transform.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damager.damage = isCharged ? energySettings.chargedLegAttackSettings.attack : energySettings.standardLegAttackSettings.attack;
                damager.damagerType = DamagerType.GrapplingHook;
                damager.impactDirection = transform.forward;
                damageable.TakeDamage(damager);
            }
        }
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable){
        base.AttachAffect(ref wielderStats, wielderMovable);
    }

    public override void DeActivate()
    {
        
    }

    public override void DetachAffect(){
        base.DetachAffect();
    }

    public bool GetHitTargetThisShot() { return hitTargetThisShot; }
    public void SetHitTargetThisShot(bool hitTarget) { hitTargetThisShot = hitTarget;}
    #endregion

    #region Private Methods
    #endregion

    #region Private Structures
    #endregion

}
