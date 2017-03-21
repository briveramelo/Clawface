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
                if (!canAttackAgain) return;

                if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_ARM_LEFT, ButtonMode.HELD))
                {
                    chargeTimer += Time.deltaTime;
                    //pMove.CanMove(false);
                }
                if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_ARM_LEFT, ButtonMode.UP)) 
                {
                    //pMove.CanMove(true);
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
                break;

            case ModSpot.ArmR:
                if (!canAttackAgain) return;

                if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_ARM_RIGHT, ButtonMode.HELD))
                {
                    chargeTimer += Time.deltaTime;
                    //pMove.CanMove(false);
                }

                if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_ARM_RIGHT, ButtonMode.UP))
                {
                    //pMove.CanMove(true);
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

                break;
            case ModSpot.Legs:
                if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_LEGS, ButtonMode.DOWN))
                {
                    if (wielderMovable.IsGrounded())
                    {
                        wielderMovable.AddDecayingForce(Vector3.up * jumpForce);
                    }
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
        if (armRegularAttackHitboxIsActive)
        {
            if (objectsHitDuringAttack.Contains(other.transform.root)) return;
            IDamageable damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
            {
                objectsHitDuringAttack.Add(other.transform.root);

                if (!other.transform.root.CompareTag(Strings.Tags.PLAYER) &&
                    transform.root.CompareTag(Strings.Tags.PLAYER)) {
                    // If the player is the one attacking

                    HitstopManager.Instance.StartHitstop(armRegularHitstop);
                }
                if (!transform.root.CompareTag(other.transform.root.tag)) {
                    damageable.TakeDamage(armRegularDamage);
                }
            }
        }
        else if (armChargedAttackHitboxIsActive)
        {
            if (objectsHitDuringAttack.Contains(other.transform.root)) return;

            IDamageable damageable = other.GetComponent<IDamageable>();
            IMovable movable = other.GetComponent<IMovable>();
            if (damageable != null)
            {
                if (this.transform.root.tag == Strings.Tags.PLAYER)
                {
                    // If the player is the one attacking
                    if (other.transform.root.tag != Strings.Tags.PLAYER)
                    {
                        HitstopManager.Instance.StartHitstop(armChargedHitstop);
                        damageable.TakeDamage(armChargedDamage);
                    }
                }
                else
                {
                    // If the enemy is the one attacking, they can only affect the player
                    if (other.transform.root.tag == Strings.Tags.PLAYER)
                    {
                        damageable.TakeDamage(armChargedDamage);
                    }
                }
            }
            if (movable != null)
            {
                if (this.transform.root.tag == Strings.Tags.PLAYER)
                {
                    // If the player is the one attacking
                    movable.AddDecayingForce(GetNormalizedDistance(this.transform.root, other.transform.root) * armChargedForce);
                }
                else
                {
                    // If the enemy is the one attacking, they can only affect the player
                    if (other.transform.root.tag == Strings.Tags.PLAYER)
                    {
                        movable.AddDecayingForce(GetNormalizedDistance(this.transform.root, other.transform.root) * armChargedForce);
                    }
                }
            }
            if (damageable != null || movable != null)
            {
                objectsHitDuringAttack.Add(other.transform.root);
            }
        }
        else if (legAttackHitboxIsActive)
        {

        }
    }

    #endregion

    #region Public Methods

    public override void Activate()
    {

    }


    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        this.wielderStats = wielderStats;
        this.wielderMovable = wielderMovable;
        pickupCollider.enabled = false;
        attackCollider.enabled = false;
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

    public override void AlternateActivate(bool isHeld)
    {
        
    }
    #endregion

}
