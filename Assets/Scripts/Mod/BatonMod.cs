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

    private ProjectileProperties projectileProperties = new ProjectileProperties();

    protected override void Awake(){        
        type = ModType.StunBaton;
        category = ModCategory.Melee;
        base.Awake();
    }

    public override void Activate(Action onComplete=null){
        base.Activate();        
    }


    public override void DeActivate()
    {
        //Nothing to do here
    }

    protected override void BeginChargingArms(){ }
    protected override void RunChargingArms(){ }
    protected override void ActivateStandardArms(){ Timing.RunCoroutine(Swing()); }
    protected override void ActivateChargedArms(){ Timing.RunCoroutine(Swing()); }

    protected override void BeginChargingLegs(){ }
    protected override void RunChargingLegs(){ }
    protected override void ActivateChargedLegs(){ LayMine(); }
    protected override void ActivateStandardLegs(){ LayMine(); }

    IEnumerator<float> Swing(){
        //AudioManager.Instance.PlaySFX(SFXType.StunBatonSwing);
        float timeRemaining = energySettings.timeToAttack;        
        while (timeRemaining>0 && isActiveAndEnabled) {            
            GetOverlap().ForEach(other => {
                if (GetWielderInstanceID() != other.gameObject.GetInstanceID() &&
                    (other.gameObject.CompareTag(Strings.Tags.PLAYER) ||
                    other.gameObject.CompareTag(Strings.Tags.ENEMY))){

                    IDamageable damageable = other.GetComponent<IDamageable>();
                    if (damageable != null && !recentlyHitEnemies.Contains(damageable)){
                        AudioManager.Instance.PlaySFX(SFXType.StunBatonImpact);

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

    private List<Collider> GetOverlap(){ 
        int layerMask =LayerMasker.GetLayerMask(LayerMasker.Damageable);
        return Physics.OverlapCapsule(capsuleBounds.Start, capsuleBounds.End, capsuleBounds.radius, layerMask).ToList();
    }

    void LayMine(){
        GameObject stunMine = ObjectPool.Instance.GetObject(PoolObjectType.Mine);
        if (stunMine != null){
            //AudioManager.Instance.PlaySFX(SFXType.StunBatonLayMine);
            projectileProperties.Initialize(GetWielderInstanceID(), Attack);
            stunMine.GetComponent<StunMine>().SetProjectileProperties(projectileProperties);
            stunMine.transform.position = transform.position;
        }
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable){
        base.AttachAffect(ref wielderStats, wielderMovable);
    }

    public override void DetachAffect(){
        base.DetachAffect();
    }      

}
