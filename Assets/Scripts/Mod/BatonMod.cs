using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
using System.Linq;

public class BatonMod : Mod {

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(capsuleBounds.Start, capsuleBounds.radius);
        Gizmos.DrawWireSphere(capsuleBounds.End, capsuleBounds.radius);        
    }

    [SerializeField] private VFXStunBatonImpact impactEffect;
    [SerializeField] private CapsuleBounds capsuleBounds;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float slamForce;
    [SerializeField]
    Collider slamBox;
    [SerializeField]
    private float slamPush;
    [SerializeField]
    private float chargeMultiplier;
    [SerializeField]
    private float slamForceDecayRate;

    private ProjectileProperties projectileProperties = new ProjectileProperties();
    private bool wasCharged;
    

    //public Vector3 footPosition;

    protected override void Awake(){        
        type = ModType.StunBaton;
        category = ModCategory.Melee;
        //footPosition = Vector3.zero;
        base.Awake();
    }
    
    void Start()
    {
        slamBox.enabled = false;
        wasCharged = false;
    }   

    public override void Activate(Action onCompleteCoolDown=null, Action onActivate=null){
        if (getModSpot() != ModSpot.Legs)
        {
            onActivate = () => { SFXManager.Instance.Play(SFXType.StunBatonSwing, transform.position); };
        }
        base.Activate(onCompleteCoolDown, onActivate);
        SFXManager.Instance.Stop(SFXType.StunBatonCharge);
    }


    public override void DeActivate()
    {
        //Nothing to do here
    }

    protected override void BeginChargingArms(){SFXManager.Instance.Play(SFXType.StunBatonCharge, transform.position); }
    protected override void RunChargingArms(){ }
    protected override void ActivateStandardArms(){ Timing.RunCoroutine(Swing()); }
    protected override void ActivateChargedArms(){ Timing.RunCoroutine(Swing()); }

    protected override void BeginChargingLegs(){ }
    protected override void RunChargingLegs(){ }
    protected override void ActivateChargedLegs(){
        wasCharged = true;
        Jump();
        Slam();
    }
    protected override void ActivateStandardLegs(){
        Jump();
        Slam();
    }

    IEnumerator<float> Swing(){
        SFXManager.Instance.Play(SFXType.StunBatonSwing, transform.position);
        float timeRemaining = energySettings.timeToAttack;        
        while (timeRemaining>0 && isActiveAndEnabled) {            
            GetOverlap().ForEach(other => {
                if (GetWielderInstanceID() != other.gameObject.GetInstanceID() &&
                    (other.gameObject.CompareTag(Strings.Tags.PLAYER) ||
                    other.gameObject.CompareTag(Strings.Tags.ENEMY))){

                    IDamageable damageable = other.GetComponent<IDamageable>();
                    if (damageable != null && !recentlyHitObjects.Contains(other.gameObject)){
                        SFXManager.Instance.Play(SFXType.StunBatonImpact, transform.position);

                        if (wielderStats.gameObject.CompareTag(Strings.Tags.PLAYER)){
                            HitstopManager.Instance.StartHitstop(energySettings.hitStopTime);
                            AnalyticsManager.Instance.AddModDamage(this.getModType(), energySettings.attack);

                            if (damageable.GetHealth() - Attack < 0.1f)
                            {
                                AnalyticsManager.Instance.AddModKill(this.getModType());
                            }
                        }
                        else
                        {
                            AnalyticsManager.Instance.AddEnemyModDamage(this.getModType(), Attack);
                        }
                        
                        impactEffect.Emit();
                        damager.Set(Attack, getDamageType(), wielderMovable.GetForward());
                        damageable.TakeDamage(damager);
                        recentlyHitObjects.Add(other.gameObject);
                        IStunnable stunnable = other.GetComponent<IStunnable>();
                        if (stunnable != null){
                            stunnable.Stun();
                        }
                    }
                }
            });
            timeRemaining -= Time.deltaTime;
            yield return 0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(isAttached && other.gameObject.GetInstanceID() != projectileProperties.shooterInstanceID)
        {
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
            IMovable moveable = other.gameObject.GetComponent<IMovable>();
            float attack = wasCharged ? energySettings.chargedLegAttackSettings.attack : energySettings.standardLegAttackSettings.attack;
            if (damageable != null)
            {
                DamagerType dType = getModSpot()==ModSpot.Legs ? DamagerType.StunStomp : DamagerType.StunSwing;
                damager.Set(attack, dType, Vector3.down);
                damageable.TakeDamage(damager);
            }
            if(wasCharged && moveable != null)
            {
                moveable.AddDecayingForce(-moveable.GetForward() * slamPush);
            }
        }
    }

    private void Jump()
    {
        if (wielderMovable.IsGrounded())
        {
            wielderMovable.AddDecayingForce(Vector3.up * jumpForce);
        }
    }

    private void Slam(bool charged = false)
    {
        if (!wielderMovable.IsGrounded())
        {
            wielderMovable.StopVerticalMovement();
            wielderMovable.AddDecayingForce(Vector3.down * slamForce, slamForceDecayRate);
            StartCoroutine(TurnOnSlamBox());
        }
    }

    private IEnumerator TurnOnSlamBox()
    {
        slamBox.enabled = true;
        while (!wielderMovable.IsGrounded())
        {
            yield return null;
        }
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if(wasCharged && Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.NameToLayer(Strings.Layers.GROUND))){
            wasCharged = false;

            GameObject blasterShoot = ObjectPool.Instance.GetObject(PoolObjectType.VFXBlasterShoot);
            blasterShoot.transform.position = hit.transform.position;
            blasterShoot.transform.rotation = transform.rotation;            
        }
        InputManager.Instance.Vibrate(VibrationTargets.BOTH, 1f);
        slamBox.enabled = false;
        yield return null;
    }

    private List<Collider> GetOverlap(){ 
        int layerMask =LayerMasker.GetLayerMask(LayerMasker.Damageable);
        return Physics.OverlapCapsule(capsuleBounds.Start, capsuleBounds.End, capsuleBounds.radius, layerMask).ToList();
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable){
        projectileProperties.shooterInstanceID = wielderStats.gameObject.GetInstanceID();
        base.AttachAffect(ref wielderStats, wielderMovable);
    }

    public override void DetachAffect(){
        projectileProperties.shooterInstanceID = 0;
        //footPosition = Vector3.zero;
        base.DetachAffect();
        wasCharged = false;
    }      

}
