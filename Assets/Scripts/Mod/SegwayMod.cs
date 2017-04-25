// Adam Kay

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
using System.Linq;
using MovementEffects;

public class SegwayMod : Mod {

    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(capsuleBoundsDirection.Start, capsuleBoundsDirection.radius);
        Gizmos.DrawWireSphere(capsuleBoundsDirection.End, capsuleBoundsDirection.radius);        

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(capsuleBoundsDirection.Start, aoeRadius);
    }
    
    [SerializeField] private VFXSegway segwayVFX;
    [SerializeField] private ForceSettings standardForceSettings;
    [SerializeField] private ForceSettings chargedForceSettings;

    [SerializeField] private CapsuleBoundsDirection capsuleBoundsDirection;
    [SerializeField] private float speedBoostMultiplier;
    [SerializeField] private float aoeRadius;

    // Use this for initialization
    void Start()
    {
        setModType(ModType.ForceSegway);        
    }

    protected override void Update(){
        if (wielderMovable != null){
            if (getModSpot() != ModSpot.Legs){
                transform.forward = wielderMovable.GetForward();
            }
        }
    }    

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        base.AttachAffect(ref wielderStats, wielderMovable);        
        segwayVFX.SetIdle(false);

        if (getModSpot() == ModSpot.Legs){
            segwayVFX.SetMoving(true);
            this.wielderStats.Multiply(StatType.MoveSpeed, speedBoostMultiplier);
            this.wielderMovable = wielderMovable;
            this.wielderMovable.SetMovementMode(MovementMode.ICE);
        }
        else{
            segwayVFX.SetMoving(false);
        }
    }

    public override void Activate(Action onCompleteCoolDown=null, Action onActivate=null){
        capsuleBoundsDirection.length = IsCharged() ? chargedForceSettings.capsuleLength : standardForceSettings.capsuleLength;
        base.Activate(onCompleteCoolDown, onActivate);   
    }

    protected override void BeginChargingArms(){ }
    protected override void RunChargingArms(){ }
    protected override void ActivateStandardArms(){ ForcePush(); }
    protected override void ActivateChargedArms(){ MegaForcePush(); }

    protected override void BeginChargingLegs(){ }
    protected override void RunChargingLegs(){ }
    protected override void ActivateChargedLegs(){
        MegaForcePush();
        if (wielderMovable.IsGrounded()) {
           Jump();
        }
    }
    protected override void ActivateStandardLegs(){
        ForcePush();
        if (wielderMovable.IsGrounded()) {
            Jump();
        }
    }        

    public override void DeActivate()
    {
        
    }

    public override void DetachAffect()
    {        
        segwayVFX.SetMoving(false);
        segwayVFX.SetIdle(true);
        if (getModSpot() == ModSpot.Legs)
        {
            wielderStats.Multiply(StatType.MoveSpeed, 1f / speedBoostMultiplier);
            this.wielderMovable.SetMovementMode(MovementMode.PRECISE);
        }
        base.DetachAffect();
    }

    void AoeAttack(){
        
    }

    void ForcePush(){
        //SFXManager.Instance.Play(SFXType.ForceSegwayPush);
        PoolObjectType poolObjType = IsCharged() ? PoolObjectType.VFXSegwayBlasterCharged : PoolObjectType.VFXSegwayBlaster;
        GameObject blasterFX = ObjectPool.Instance.GetObject(poolObjType);
        if (blasterFX) {
            blasterFX.DeActivate(1.1f);
            blasterFX.transform.position = capsuleBoundsDirection.Start;
            blasterFX.transform.forward = transform.forward;
        }        
        Timing.RunCoroutine(PushForTime());                        
    }

    void MegaForcePush() {        
        ForcePush();
    }

    IEnumerator<float> PushForTime() {
        float timeRemaining = energySettings.timeToAttack;
        while (timeRemaining>0) {
            GetOverlap().ForEach(other => {
                if (GetWielderInstanceID() != other.gameObject.GetInstanceID()){                    
                    IDamageable damageable = other.GetComponent<IDamageable>();
                    IMovable movable = other.GetComponent<IMovable>();

                    if (!recentlyHitObjects.Contains(other.gameObject)) {
                        if (damageable != null)
                        {
                            if (wielderStats.CompareTag(Strings.Tags.PLAYER))
                            {
                                AnalyticsManager.Instance.AddModDamage(this.getModType(), Attack);

                                if (damageable.GetHealth() - Attack < 0.1f)
                                {
                                    AnalyticsManager.Instance.AddModKill(this.getModType());
                                }
                            }
                            else
                            {
                                AnalyticsManager.Instance.AddEnemyModDamage(this.getModType(), Attack);
                            }

                            
                            damager.Set(Attack, getDamageType(), wielderMovable.GetForward());
                            damageable.TakeDamage(damager);
                        }                                 
                        if (movable != null){                            
                            Vector3 pushDirection = wielderMovable.GetForward();
                            movable.AddDecayingForce(pushDirection * pushForce);
                        }

                        if (damageable != null || movable != null)
                        {
                            recentlyHitObjects.Add(other.gameObject);
                        }
                    }
                }
            });
            timeRemaining -= Time.deltaTime;
            yield return 0f;
        }
    }

    List<Collider> GetOverlap(){ 
        if (getModSpot()!=ModSpot.Legs){ 
            return Physics.OverlapCapsule(capsuleBoundsDirection.Start, capsuleBoundsDirection.End, capsuleBoundsDirection.radius).ToList();
        }
        return Physics.OverlapSphere(capsuleBoundsDirection.Start, aoeRadius).ToList();
    }

    void Jump() {
        wielderMovable.AddDecayingForce(Vector3.up * jumpForce);
    }

    private float jumpForce {
        get {
            return IsCharged() ? chargedForceSettings.jumpForce : standardForceSettings.jumpForce;
        }
    }
    private float pushForce {
        get {
            if (getModSpot()!=ModSpot.Legs){ 
                return IsCharged() ? chargedForceSettings.armpushForce : standardForceSettings.armpushForce;
            }
            return IsCharged() ? chargedForceSettings.aoePushForce : standardForceSettings.aoePushForce;
        }
    }

    #region Private Structures
    [System.Serializable]
    private class ForceSettings {
        public float jumpForce;
        public float armpushForce;
        public float aoePushForce;
        public float capsuleLength;
    }
    #endregion
}
