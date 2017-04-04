// Adam Kay

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankTreadsMod : Mod
{
    #region Serialized
    [SerializeField]
    private Collider attackCollider;

    [SerializeField]
    private float chargeTime;

    [SerializeField]
    private float armRegularDamage;

    [SerializeField]
    private float armRegularStartupTime;

    [SerializeField]
    private float armRegularActiveTime;

    [SerializeField]
    private float armRegularCooldownTime;

    [SerializeField]
    private float armRegularHitstop;

    [SerializeField]
    private float armChargedDamage;

    [SerializeField]
    private float armChargedStartupTime;

    [SerializeField]
    private float armChargedActiveTime;

    [SerializeField]
    private float armChargedCooldownTime;

    [SerializeField]
    private float armChargedHitstop;

    [SerializeField]
    private float armChargedForce;

    [SerializeField]
    private float legsMoveSpeedMod;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private float legsCrushDamage;

    [SerializeField]
    private float timeBetweenLegsDamageTick;

    [SerializeField]
    private float dampenForcesOnPlayerBy;
    #endregion

    #region Privates
    private float savedMoveSpeed;
    private float savedDampenForces;
    private float chargeTimer;

    private bool armRegularAttackHitboxIsActive;
    private bool armChargedAttackHitboxIsActive;
    private bool legAttackHitboxIsActive;

    private List<Transform> objectsHitDuringAttack;
    private bool canAttackAgain;

    private float legsTimer;
    #endregion Privates

    #region Unity Lifetime

    // Use this for initialization
    void Start()
    {
        attackCollider.enabled = false;
        setModSpot(ModSpot.Default);
        objectsHitDuringAttack = new List<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (getModSpot())
        {
            case ModSpot.ArmL:
                break;

            case ModSpot.ArmR:
                break;

            case ModSpot.Legs:
                legsTimer -= Time.deltaTime;

                if (wielderMovable.IsGrounded())
                {
                    legAttackHitboxIsActive = false;
                    attackCollider.enabled = false;
                }
                break;
        }
    }

    void Awake()
    {
        setModType(ModType.TankTreads);
    }

    private void OnTriggerStay(Collider other)
    {
        IDamageable damageable = other.transform.root.GetComponentInChildren<IDamageable>();
        IMovable movable = other.transform.root.GetComponentInChildren<IMovable>();

        if (armRegularAttackHitboxIsActive)
        {
            if (objectsHitDuringAttack.Contains(other.transform.root)) return;

            if (damageable != null)
            {
                objectsHitDuringAttack.Add(other.transform.root);

                bool isPlayer = false;
                // If the player is the one attacking, and they're attacking something other than the player
                if (!other.transform.root.CompareTag(Strings.Tags.PLAYER) && transform.root.CompareTag(Strings.Tags.PLAYER)) {
                    HitstopManager.Instance.StartHitstop(armRegularHitstop);
                    isPlayer = true;
                    
                }


                if (!transform.root.CompareTag(other.transform.root.tag)) {
                    if (isPlayer)
                    {
                        AnalyticsManager.Instance.AddModDamage(this.getModType(), (int)armRegularDamage);
                    }
                    else
                    {
                        AnalyticsManager.Instance.AddEnemyModDamage(this.getModType(), (int)armRegularDamage);
                    }
                    damageable.TakeDamage(armRegularDamage);
                    if (damageable.GetHealth() <= 0.01f)
                    {
                        AnalyticsManager.Instance.AddModKill(this.getModType());
                    }
                }
            }
        }
        else if (armChargedAttackHitboxIsActive)
        {
            if (objectsHitDuringAttack.Contains(other.transform.root)) return;

            if (damageable != null)
            {
                bool isPlayer = false;
                if (this.transform.root.tag == Strings.Tags.PLAYER && other.transform.root.tag != Strings.Tags.PLAYER)
                {
                    isPlayer = true;
                    HitstopManager.Instance.StartHitstop(armChargedHitstop);
                }

                if (!transform.root.CompareTag(other.transform.root.tag))
                {
                    if (isPlayer)
                    {
                        AnalyticsManager.Instance.AddModDamage(this.getModType(), (int)armChargedDamage);
                    }
                    else
                    {
                        AnalyticsManager.Instance.AddEnemyModDamage(this.getModType(), (int)armChargedDamage);
                    }
                    damageable.TakeDamage(armChargedDamage);
                    if (damageable.GetHealth() <= 0.01f)
                    {
                        AnalyticsManager.Instance.AddModKill(this.getModType());
                    }
                }
            }

            if (movable != null)
            {
                if (!this.transform.root.CompareTag(other.transform.root.tag))
                {
                    movable.AddDecayingForce(GetNormalizedDistance(this.transform.root, other.transform.root) * armChargedForce);
                }
            }

            if (damageable != null || movable != null)
            {
                objectsHitDuringAttack.Add(other.transform.root);
            }
        }
        else if (legAttackHitboxIsActive)
        {
            if (damageable != null)
            {
                if (legsTimer < 0f)
                {
                    if (!this.transform.root.CompareTag(other.transform.root.tag))
                    {
                        if (this.transform.root.CompareTag(Strings.Tags.PLAYER))
                        {
                            AnalyticsManager.Instance.AddModDamage(this.getModType(), legsCrushDamage);
                        }
                        else
                        {
                            AnalyticsManager.Instance.AddEnemyModDamage(this.getModType(), legsCrushDamage);
                        }
                        damageable.TakeDamage(legsCrushDamage);

                        if (damageable.GetHealth() <= 0.01f)
                        {
                            AnalyticsManager.Instance.AddModKill(this.getModType());
                        }

                        legsTimer = timeBetweenLegsDamageTick;
                        legAttackHitboxIsActive = false;
                        attackCollider.enabled = false;

                        IMovable selfMovable = this.transform.root.GetComponentInChildren<IMovable>();
                        if (selfMovable != null)
                        {
                            selfMovable.AddDecayingForce(Vector3.up * jumpForce);
                        }
                    }
                }
            }
        }
    }

    #endregion

    #region Public Methods

    public override void Activate()
    {

        switch (getModSpot())
        {
            case ModSpot.ArmL:
            case ModSpot.ArmR:
                if (!canAttackAgain) return;
                Hit();
                break;

            case ModSpot.Legs:
                if (wielderMovable.IsGrounded())
                {
                    wielderMovable.AddDecayingForce(Vector3.up * jumpForce);
                    legAttackHitboxIsActive = true;
                    attackCollider.enabled = true;
                }
                break;
        }
    }

    public override void AlternateActivate(bool isHeld, float holdTime)
    {
        switch (getModSpot())
        {
            case ModSpot.ArmL:
            case ModSpot.ArmR:
                if (isHeld)
                {
                    wielderStats.moveSpeed = 0;
                }
                else
                {
                    wielderStats.moveSpeed = savedMoveSpeed;
                    ChargedHit();
                }
                break;
        }
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        this.wielderStats = wielderStats;
        this.wielderMovable = wielderMovable;
        pickupCollider.enabled = false;
        attackCollider.enabled = false;
        savedMoveSpeed = wielderStats.moveSpeed;
        canAttackAgain = true;

        if (getModSpot() == ModSpot.Legs)
        {
            this.wielderStats.Modify(StatType.MoveSpeed, legsMoveSpeedMod);
        }        
    }

    public override void DeActivate()
    {

    }

    public override void DetachAffect()
    {
        pickupCollider.enabled = true;
        attackCollider.enabled = false;
        canAttackAgain = false;        

        if (getModSpot() == ModSpot.Legs)
        {
            wielderStats.Modify(StatType.MoveSpeed, 1f / legsMoveSpeedMod);
        }

        setModSpot(ModSpot.Default);
    }

    public void ChargedHit()
    {
        StartCoroutine(ArmChargedStartup());
    }

    public void Hit()
    {
        StartCoroutine(ArmRegularStartup());
    }

    #endregion

    #region Private Methods

    private Vector3 GetNormalizedDistance(Transform forceSender, Transform forceReceiver)
    {
        Vector3 normalizedDistance = forceReceiver.position - forceSender.position;
        normalizedDistance = normalizedDistance.normalized;
        return normalizedDistance;
    }

    private void EnableAttackCollider()
    {
        attackCollider.enabled = true;
    }

    private void DisableAttackCollider()
    {
        attackCollider.enabled = false;
    }

    private IEnumerator ArmRegularStartup()
    {
        canAttackAgain = false;
        yield return new WaitForSeconds(armRegularStartupTime);
        StartCoroutine(ArmRegularActive());
    }

    private IEnumerator ArmRegularActive()
    {
        EnableAttackCollider();
        armRegularAttackHitboxIsActive = true;

        yield return new WaitForSeconds(armRegularActiveTime);
        StartCoroutine(ArmRegularCooldown());
    }

    private IEnumerator ArmRegularCooldown()
    {
        DisableAttackCollider();
        armRegularAttackHitboxIsActive = false;
        objectsHitDuringAttack.Clear();

        yield return new WaitForSeconds(armRegularCooldownTime);
        canAttackAgain = true;
    }

    private IEnumerator ArmChargedStartup()
    {
        yield return new WaitForSeconds(armChargedStartupTime);
        StartCoroutine(ArmChargedActive());
    }

    private IEnumerator ArmChargedActive()
    {
        EnableAttackCollider();
        armChargedAttackHitboxIsActive = true;
        
        yield return new WaitForSeconds(armChargedActiveTime);
        StartCoroutine(ArmChargedCooldown());
    }

    private IEnumerator ArmChargedCooldown()
    {
        DisableAttackCollider();
        armChargedAttackHitboxIsActive = false;
        objectsHitDuringAttack.Clear();

        yield return new WaitForSeconds(armChargedCooldownTime);
        canAttackAgain = true;
    }

    private void AttackBasedOnCharge()
    {
        if (chargeTimer >= chargeTime)
        {
            ChargedHit();
        }
        else
        {
            Hit();
        }
        chargeTimer = 0f;
	}
	
    
    #endregion

}
