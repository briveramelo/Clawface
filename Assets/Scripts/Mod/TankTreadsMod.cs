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
    private bool isCrushing = false;
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

    protected override void Update()
    {
        base.Update();
        if (wielderMovable != null)
        {
            if (wielderMovable.IsGrounded()) isCrushing = false;
        }
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
    protected override void ActivateStandardLegs()
    {
        if (wielderMovable.IsGrounded())
        {
            Jump();
        }
        else
        {
            Stomp();
        }
    }

    protected override void ActivateChargedLegs()
    {
        if (wielderMovable.IsGrounded())
        {
            Jump();
        }
        else
        {
            Stomp();
        }
    }


    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable){
        base.AttachAffect(ref wielderStats, wielderMovable);
        if (getModSpot() == ModSpot.Legs){
            this.wielderStats.Multiply(StatType.MoveSpeed, legsMoveSpeedMod);
            hasState = false;
        }
        else
        {
            hasState = true;
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

    IEnumerator<float> DoCrush()
    {
        float timeBetweenHits = energySettings.coolDownTime;

        while (isCrushing)
        {
            Physics.OverlapSphere(transform.position, attackSphereRadius).ToList().ForEach(other =>
            {
                if (GetWielderInstanceID() != other.gameObject.GetInstanceID())
                {
                    IDamageable damageable = other.GetComponent<IDamageable>();
                    IMovable movable = other.GetComponent<IMovable>();

                   
                    if (damageable != null && timeBetweenHits < 0f)
                    {
                        SFXManager.Instance.Play(SFXType.TankTreads_Attack, transform.position);
                        recentlyHitEnemies.Add(damageable);
                        timeBetweenHits = energySettings.coolDownTime;

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

                    if (movable != null)
                    {
                        AddCrushPushForce(other, movable);
                    }
                }
            });
            timeBetweenHits -= Time.deltaTime;
            yield return 0f;
        }
    }

    #endregion

    #region Private Methods

    private void Jump()
    {
        wielderMovable.AddDecayingForce(Vector3.up * jumpForce);
    }

    private void Stomp()
    {
        wielderMovable.AddDecayingForce(Vector3.down * crushForce);
        isCrushing = true;
        Timing.RunCoroutine(DoCrush());
    }

    private void AddCrushPushForce(Collider other, IMovable movable)
    {
        Vector3 direction = (other.transform.position - transform.position).normalized;
        movable.AddDecayingForce(direction * crushPushForce);
    }

    private float jumpForce {
        get {
            return IsCharged() ? chargedForceSettings.jumpForce : standardForceSettings.jumpForce;
        }
    }

    private float crushForce
    {
        get
        {
            return IsCharged() ? chargedForceSettings.crushForce : standardForceSettings.crushForce;
        }
    }

    private float crushPushForce
    {
        get
        {
            return IsCharged() ? chargedForceSettings.crushPushForce : standardForceSettings.crushPushForce;
        }
    }

    private float pushForce {
        get {
            return IsCharged() ? chargedForceSettings.armPushForce : standardForceSettings.armPushForce;
        }
    }

    #endregion
    #region PRIVATE Structures
    [System.Serializable]
    private class ForceSettings {
        public float jumpForce;
        public float armPushForce;
        public float crushForce;
        public float crushPushForce;
    }
    #endregion
}
