// Adam Kay

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
using System.Linq;

public class TankTreadsMod : Mod
{
    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackSphereRadius);
    }

    #region Serialized
    [SerializeField] private float attackSphereRadius;    
    [SerializeField] private float legsMoveSpeedMod;
    [SerializeField] private ForceSettings standardForceSettings;
    [SerializeField] private ForceSettings chargedForceSettings;
    #endregion

    #region Privates
    #endregion Privates

    #region Unity Lifetime
    protected override void Awake(){
        base.Awake();
        setModType(ModType.TankTreads);
    }
    // Use this for initialization
    void Start(){        
        setModSpot(ModSpot.Default);
    }
    #endregion

    #region Public Methods

    public override void Activate(Action onCompleteCoolDown=null, Action onActivate=null){
        onActivate = ()=> { SFXManager.Instance.Play(SFXType.TankTreads_Swing, transform.position);};
        onCompleteCoolDown = ()=> { SFXManager.Instance.Stop(SFXType.TankTreads_Swing);};
        base.Activate(onCompleteCoolDown, onActivate);
    }


    protected override void BeginChargingArms(){ }
    protected override void RunChargingArms(){ }
    protected override void ActivateStandardArms(){ Hit(); }

    protected override void ActivateChargedArms(){ Hit(); }

    protected override void BeginChargingLegs(){ }
    protected override void RunChargingLegs(){ }
    protected override void ActivateStandardLegs(){ Jump(); }

    protected override void ActivateChargedLegs(){ Jump(); }


    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable){
        base.AttachAffect(ref wielderStats, wielderMovable);
        if (getModSpot() == ModSpot.Legs){
            this.wielderStats.Multiply(StatType.MoveSpeed, legsMoveSpeedMod);
        }        
    }

    public override void DeActivate(){

    }

    public override void DetachAffect(){    
        if (getModSpot() == ModSpot.Legs){
            wielderStats.Multiply(StatType.MoveSpeed, 1f / legsMoveSpeedMod);
        }
        base.DetachAffect();
    }   

    public void Hit(){
        Timing.RunCoroutine(DoHit());
    }

    IEnumerator<float> DoHit() {
        float timeRemaining = energySettings.coolDownTime;
        while (timeRemaining>0) {
            Physics.OverlapSphere(transform.position, attackSphereRadius).ToList().ForEach(other=> {
                if (GetWielderInstanceID()!=other.gameObject.GetInstanceID() ) {
                    IDamageable damageable = other.GetComponent<IDamageable>();
                    IMovable movable = other.GetComponent<IMovable>();

                    if (recentlyHitEnemies.Contains(damageable)) return;

                    if (damageable != null){
                        SFXManager.Instance.Play(SFXType.TankTreads_Attack, transform.position);
                        recentlyHitEnemies.Add(damageable);
                        
                
                        if (wielderStats.CompareTag(Strings.Tags.PLAYER)) {
                            AnalyticsManager.Instance.AddModDamage(this.getModType(), Attack);
                            HitstopManager.Instance.StartHitstop(energySettings.hitStopTime);

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
                        Vector3 direction = (other.transform.position - transform.position).normalized;
                        movable.AddDecayingForce(direction * pushForce);                    
                    }
                }
            });
            timeRemaining -= Time.deltaTime;
            yield return 0f;
        }
    }

    #endregion

    #region Private Methods

    private void Jump() {
        if (wielderMovable.IsGrounded()){
            wielderMovable.AddDecayingForce(Vector3.up * jumpForce);
        }
    }

    private float jumpForce {
        get {
            return IsCharged() ? chargedForceSettings.jumpForce : standardForceSettings.jumpForce;
        }
    }
    private float pushForce {
        get {
            return IsCharged() ? chargedForceSettings.armpushForce : standardForceSettings.armpushForce;
        }
    }

    #endregion
    #region PRIVATE Structures
    [System.Serializable]
    private class ForceSettings {
        public float jumpForce;
        public float armpushForce;
    }
    #endregion
}
