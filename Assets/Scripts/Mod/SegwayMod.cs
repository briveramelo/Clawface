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

    [SerializeField] private CapsuleBoundsDirection capsuleBoundsDirection;
    [SerializeField] private float aoeRadius;

    // Use this for initialization
    protected override void Awake()
    {
        setModType(ModType.ForceSegway); 
        base.Awake();       
    }

    protected override void Update(){
        if (wielderMovable != null){
            transform.forward = wielderMovable.GetForward();
        }
    }    

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        base.AttachAffect(ref wielderStats, wielderMovable);        
        segwayVFX.SetIdle(false);
        segwayVFX.SetMoving(false);
    }

    public override void Activate(Action onCompleteCoolDown=null, Action onActivate=null){
        capsuleBoundsDirection.length = standardForceSettings.capsuleLength;
        base.Activate(onCompleteCoolDown, onActivate);   
    }

    protected override void ActivateStandardArms(){ ForcePush(); }

    public override void DeActivate()
    {
        
    }

    public override void DetachAffect()
    {        
        segwayVFX.SetMoving(false);
        segwayVFX.SetIdle(true);        
        base.DetachAffect();
    }

    void ForcePush(){
        SFXManager.Instance.Play(SFXType.SegwayBlast_Standard, transform.position);
        PoolObjectType poolObjType = PoolObjectType.VFXSegwayBlaster;
        GameObject blasterFX = ObjectPool.Instance.GetObject(poolObjType);
        if (blasterFX) {
            blasterFX.DeActivate(1.1f);
            blasterFX.transform.position = capsuleBoundsDirection.Start;
            blasterFX.transform.forward = transform.forward;
        }        
        Timing.RunCoroutine(PushForTime());                        
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
        return Physics.OverlapCapsule(capsuleBoundsDirection.Start, capsuleBoundsDirection.End, capsuleBoundsDirection.radius).ToList();
    }

    private float pushForce {
        get {            
            return standardForceSettings.armpushForce;
        }
    }

    #region Private Structures
    [System.Serializable]
    private class ForceSettings {
        public float armpushForce;
        public float capsuleLength;
    }
    #endregion
}
