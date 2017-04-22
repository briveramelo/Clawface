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

    private ProjectileProperties projectileProperties = new ProjectileProperties();

    protected override void Awake(){        
        type = ModType.StunBaton;
        category = ModCategory.Melee;
        base.Awake();
    }

    public override void Activate(Action onCompleteCoolDown=null, Action onActivate=null){
        onActivate = ()=> { SFXManager.Instance.Play(SFXType.StunBatonSwing, transform.position);};
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
    protected override void ActivateChargedLegs(){ Jump(); }
    protected override void ActivateStandardLegs(){ Jump(); }

    IEnumerator<float> Swing(){
        SFXManager.Instance.Play(SFXType.StunBatonSwing, transform.position);
        float timeRemaining = energySettings.timeToAttack;        
        while (timeRemaining>0 && isActiveAndEnabled) {            
            GetOverlap().ForEach(other => {
                if (GetWielderInstanceID() != other.gameObject.GetInstanceID() &&
                    (other.gameObject.CompareTag(Strings.Tags.PLAYER) ||
                    other.gameObject.CompareTag(Strings.Tags.ENEMY))){

                    IDamageable damageable = other.GetComponent<IDamageable>();
                    if (damageable != null && !recentlyHitEnemies.Contains(damageable)){
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
                        recentlyHitEnemies.Add(damageable);
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

    private void Jump()
    {
        wielderMovable.AddDecayingForce(Vector3.up * jumpForce);
    }

    private List<Collider> GetOverlap(){ 
        int layerMask =LayerMasker.GetLayerMask(LayerMasker.Damageable);
        return Physics.OverlapCapsule(capsuleBounds.Start, capsuleBounds.End, capsuleBounds.radius, layerMask).ToList();
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable){
        base.AttachAffect(ref wielderStats, wielderMovable);
    }

    public override void DetachAffect(){
        base.DetachAffect();
    }      

}
