// Adam Kay

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
using System.Linq;
using MovementEffects;

public class SegwayMod : Mod {

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        Gizmos.DrawWireSphere(endPosition, attackRadius);
        Gizmos.DrawLine(transform.position, endPosition);
    }

    [SerializeField] private Collider aoeCollider;

    [SerializeField] private VFXSegway segwayVFX;

    [SerializeField] private float attackRadius;
    [SerializeField] private float attackLength;

    [SerializeField] private float speedBoostMultiplier;
    [SerializeField] private float jumpForce;
    [SerializeField] private float rangedAccuracyLoss;

    [SerializeField] private float pushDamageMultiplier;
    [SerializeField] private float pushForce;
    [SerializeField] private float pushTime;

    [SerializeField] private float aoeDamageMultiplier;
    [SerializeField] private float aoeForce;
    [SerializeField] private float aoeTime;

    private Vector3 endPosition {
        get { return transform.position + transform.forward * attackLength; }
    }

    private static string ENABLEATTACKCOLLIDER = "EnableAttackCollider";
    private static string DISABLEATTACKCOLLIDER = "DisableAttackCollider";
    private static string ENABLEAOECOLLIDER = "EnableAoeCollider";
    private static string DISABLEAOECOLLIDER = "DisableAoeCollider";

    private bool canActivate;
    private bool isPushing;
    private bool isAoeAttacking;

    // Use this for initialization
    void Start()
    {
        setModType(ModType.ForceSegway);        
    }

    protected override void Update()
    {
        if (wielderMovable != null)
        {
            if (getModSpot() != ModSpot.Legs)
            {
                //transform.up = -wielderMovable.GetForward();
            }
        }
    }    

    public override void AttachAffect(ref Stats i_playerStats, IMovable wielderMovable)
    {
        isAttached = true;
        this.wielderMovable = wielderMovable;
        wielderStats = i_playerStats;
        pickupCollider.enabled = false;
        segwayVFX.SetIdle(false);

        //TODO ask art to set default rotation to this value
        transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
        transform.localPosition = new Vector3(0f, 0.015f, 0.09f);
        //

        if (getModSpot() == ModSpot.Legs)
        {
            segwayVFX.SetMoving(true);
            wielderStats.Modify(StatType.MoveSpeed, speedBoostMultiplier);
            wielderStats.Modify(StatType.RangedAccuracy, rangedAccuracyLoss);
            this.wielderMovable = wielderMovable;
            this.wielderMovable.SetMovementMode(MovementMode.ICE);
        }
        else
        {
            segwayVFX.SetMoving(false);
        }
    }

    public override void Activate()
    {
        Action pushAction = chargeSettings.isCharged ? (Action)ActivateCharged : ActivateStandard;
        switch (getModSpot())
        {
            case ModSpot.ArmL:
                if (!isPushing) {
                    pushAction();
                }
                break;
            case ModSpot.ArmR:
                if (!isPushing) {
                    pushAction();
                }
                break;
            case ModSpot.Legs:
                if (!isAoeAttacking) {
                    AoeAttack();
                }
                if (wielderMovable.IsGrounded())
                {
                    Jump();
                }
                break;
        }
        
    }

    protected override void ActivateCharged()
    {
        MegaForcePush();
    }
    protected override void ActivateStandard()
    {
        ForcePush();
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
            wielderStats.Modify(StatType.RangedAccuracy, 1f / rangedAccuracyLoss);
            this.wielderMovable.SetMovementMode(MovementMode.PRECISE);
        }
        base.DetachAffect();
    }

    void BoostSpeed()
    {
        wielderStats.Modify(StatType.MoveSpeed, speedBoostMultiplier);
    }

    void RemoveSpeedBoost()
    {
        wielderStats.Modify(StatType.MoveSpeed, 1 / speedBoostMultiplier);
    }

    void AoeAttack()
    {
        //AudioManager.Instance.PlaySFX(SFXType.ForceSegwayPush);
        EnableAoeCollider();
        isAoeAttacking = true;
        recentlyHitEnemies.Clear();
        Invoke(SegwayMod.DISABLEAOECOLLIDER, aoeTime);
    }

    void ForcePush()
    {
        //AudioManager.Instance.PlaySFX(SFXType.ForceSegwayPush);
        GameObject blasterFX = ObjectPool.Instance.GetObject(PoolObjectType.VFXSegwayBlaster);
        if (blasterFX) {
            blasterFX.DeActivate(1.1f);
            blasterFX.transform.position = transform.position;
            blasterFX.transform.forward = -transform.up;
        }        
        Timing.RunCoroutine(PushForTime(pushTime));                        
    }

    void MegaForcePush() {
        ForcePush();
    }

    IEnumerator<float> PushForTime(float timeToPush) {
        recentlyHitEnemies.Clear();
        isPushing = true;
        float timeRemaining = timeToPush;
        while (timeRemaining>0) {
            List<Collider> cols = Physics.OverlapCapsule(transform.position, endPosition, attackRadius).ToList();
            cols.ForEach(other => {
                if (GetWielderInstanceID() != other.gameObject.GetInstanceID()){
                    if (isPushing || isAoeAttacking){
                        IDamageable damageable = other.GetComponent<IDamageable>();
                        IMovable movable = other.GetComponent<IMovable>();
                        float damage = isPushing ? pushDamageMultiplier : aoeDamageMultiplier;
                        float force = isPushing ? pushForce : aoeForce;

                        if (damageable != null && !recentlyHitEnemies.Contains(damageable)) {
                            damageable.TakeDamage(wielderStats.GetStat(StatType.Attack) * damage);
                            
                            if (movable != null){
                                Vector3 pushDirection = (other.transform.position - this.transform.position).normalized;
                                movable.AddDecayingForce(pushDirection * force);
                            }
                        }
                        if (damageable != null || movable != null){
                            recentlyHitEnemies.Add(damageable);
                        }
                    }
                }
            });
            timeRemaining -= Time.deltaTime;
            yield return 0f;
        }
        isPushing = false;
    }

    void EnableAoeCollider()
    {
        aoeCollider.enabled = true;
    }

    void DisableAoeCollider()
    {
        aoeCollider.enabled = false;
        isAoeAttacking = false;
    }    

    void Jump() {
        wielderMovable.AddDecayingForce(Vector3.up * jumpForce);
    }
}
