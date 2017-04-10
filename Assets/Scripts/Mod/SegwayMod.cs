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
        Gizmos.DrawWireSphere(capsuleBounds.start.position, capsuleBounds.radius);
        Gizmos.DrawWireSphere(capsuleBounds.end.position, capsuleBounds.radius);        

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(capsuleBounds.start.position, aoeRadius);
    }
    
    [SerializeField] private VFXSegway segwayVFX;
    [SerializeField] private ForceSettings standardForceSettings;
    [SerializeField] private ForceSettings chargedForceSettings;

    [SerializeField] private CapsuleBounds capsuleBounds;
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
                //transform.up = -wielderMovable.GetForward();
            }
        }
    }    

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        base.AttachAffect(ref wielderStats, wielderMovable);        
        segwayVFX.SetIdle(false);

        //TODO ask art to set default rotation to this value
        transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
        transform.localPosition = new Vector3(0f, 0.015f, 0.09f);
        //

        if (getModSpot() == ModSpot.Legs){
            segwayVFX.SetMoving(true);
            this.wielderStats.Modify(StatType.MoveSpeed, speedBoostMultiplier);
            this.wielderMovable = wielderMovable;
            this.wielderMovable.SetMovementMode(MovementMode.ICE);
        }
        else{
            segwayVFX.SetMoving(false);
        }
    }

    public override void Activate(Action onComplete=null){
        base.Activate();   
    }

    protected override void ActivateChargedArms(){
        MegaForcePush();
    }
    protected override void ActivateStandardArms(){
        ForcePush();
    }
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
            wielderStats.Modify(StatType.MoveSpeed, 1f / speedBoostMultiplier);
            this.wielderMovable.SetMovementMode(MovementMode.PRECISE);
        }
        base.DetachAffect();
    }

    void AoeAttack(){
        
    }

    void ForcePush(){
        //AudioManager.Instance.PlaySFX(SFXType.ForceSegwayPush);
        GameObject blasterFX = ObjectPool.Instance.GetObject(PoolObjectType.VFXSegwayBlaster);
        if (blasterFX) {
            blasterFX.DeActivate(1.1f);
            blasterFX.transform.position = transform.position;
            blasterFX.transform.forward = -transform.up;
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

                    if (!recentlyHitEnemies.Contains(damageable)) {
                        if (damageable != null) {
                            recentlyHitEnemies.Add(damageable);
                            damageable.TakeDamage(attack);                            
                        }                                        
                        if (movable != null){                            
                            Vector3 pushDirection = -transform.up.NormalizedNoY();
                            movable.AddDecayingForce(pushDirection * pushForce);
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
            return Physics.OverlapCapsule(capsuleBounds.start.position, capsuleBounds.end.position, capsuleBounds.radius).ToList();
        }
        return Physics.OverlapSphere(capsuleBounds.start.position, aoeRadius).ToList();
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
    }
    #endregion
}
