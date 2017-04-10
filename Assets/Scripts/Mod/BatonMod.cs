using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
using System.Linq;

public class BatonMod : Mod {

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(capsuleBounds.start.position, capsuleBounds.radius);
        Gizmos.DrawWireSphere(capsuleBounds.end.position, capsuleBounds.radius);
        Gizmos.DrawLine(capsuleBounds.start.position, capsuleBounds.end.position);
    }

    [SerializeField] private VFXStunBatonImpact impactEffect;
    [SerializeField] private CapsuleBounds capsuleBounds;

    private VFXHandler vfxHandler;
    private ProjectileProperties projectileProperties = new ProjectileProperties();

    protected override void Awake()
    {        
        type = ModType.StunBaton;
        category = ModCategory.Melee;
        vfxHandler = new VFXHandler(transform);
        base.Awake();
    }

    public override void Activate(Action onComplete=null){
        base.Activate(onComplete);      
    }


    public override void DeActivate()
    {
        //Nothing to do here
    }

    protected override void ActivateStandardArms() {
        Timing.RunCoroutine(Swing());
    }
    protected override void ActivateChargedArms(){
        Timing.RunCoroutine(Swing());
    }

    protected override void ActivateStandardLegs(){
        LayMine();        
    }

    protected override void ActivateChargedLegs(){
        LayMine();
    }

    IEnumerator<float> Swing(){
        //AudioManager.Instance.PlaySFX(SFXType.StunBatonSwing);
        float timeRemaining = energySettings.coolDownTime;
        int layerMask = ~(1 << (int)Layers.PlayerDetector);
        while (timeRemaining>0) {
            Physics.OverlapCapsule(capsuleBounds.start.position, capsuleBounds.end.position, capsuleBounds.radius, layerMask).ToList().ForEach(other => {
                //TODO
                //Figure out why player isn't detected when mallcop swings
                Debug.Log(other);
                if (GetWielderInstanceID() != other.gameObject.GetInstanceID() &&
                    (other.gameObject.CompareTag(Strings.Tags.PLAYER) ||
                    other.gameObject.CompareTag(Strings.Tags.ENEMY))){

                    IDamageable damageable = other.GetComponent<IDamageable>();
                    if (damageable != null && !recentlyHitEnemies.Contains(damageable)){
                        if (wielderStats.gameObject.CompareTag(Strings.Tags.PLAYER)){
                            AudioManager.Instance.PlaySFX(SFXType.StunBatonImpact);
                            HitstopManager.Instance.StartHitstop(energySettings.hitStopTime);                        
                        }
                        if (!other.CompareTag(Strings.Tags.PLAYER)) {
                            EmitBlood();
                        }
                        impactEffect.Emit();
                        damageable.TakeDamage(energySettings.attack);
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

    void LayMine(){
        GameObject stunMine = ObjectPool.Instance.GetObject(PoolObjectType.Mine);
        if (stunMine != null){
            //AudioManager.Instance.PlaySFX(SFXType.StunBatonLayMine);
            projectileProperties.Initialize(GetWielderInstanceID(), attack);
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

    private void EmitBlood() {
        Vector3 bloodDirection = wielderStats.transform.rotation.eulerAngles;
        bloodDirection.x = 23.38f;
        Quaternion emissionRotation = Quaternion.Euler(bloodDirection);
        vfxHandler.EmitBloodInDirection(emissionRotation, transform.position);
    }    

}
