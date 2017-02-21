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
    private float dampenForcesOnPlayerBy;
    #endregion

    #region Privates
    private float savedMoveSpeed;
    private float savedDampenForces;
    private float chargeTimer;

    private bool armRegularAttackHitboxIsActive;
    private bool armChargedAttackHitboxIsActive;

    private List<Transform> objectsHitDuringAttack;
    #endregion Privates


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (getModSpot())
        {
            case ModSpot.ArmL:
                if (Input.GetButton(Strings.LEFT))
                {
                    chargeTimer += Time.deltaTime;
                }
                if (Input.GetButtonUp(Strings.LEFT))
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
                break;

            case ModSpot.ArmR:
                if (Input.GetButton(Strings.RIGHT))
                {
                    chargeTimer += Time.deltaTime;
                }

                if (Input.GetButtonUp(Strings.RIGHT))
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

            IDamageable damageable = other.transform.root.GetComponent<IDamageable>();

            if (damageable != null)
            {
                objectsHitDuringAttack.Add(other.transform.root);

                if (this.transform.root.tag == Strings.PLAYER)
                {
                    // If the player is the one attacking
                    HitstopManager.Instance.StartHitstop(armRegularHitstop);
                    damageable.TakeDamage(armRegularDamage);
                    

                }
                else
                {
                    // If the enemy is the one attacking, they can only affect the player
                    if (other.transform.root.tag == Strings.PLAYER)
                    {
                        damageable.TakeDamage(armRegularDamage);
                    }
                }
            }
        }
        else if (armChargedAttackHitboxIsActive)
        {
            if (objectsHitDuringAttack.Contains(other.transform.root)) return;

            IDamageable damageable = other.transform.root.GetComponent<IDamageable>();
            IMovable movable = other.transform.root.GetComponent<IMovable>();

            if (damageable != null)
            {

            }

            if (movable != null)
            {

            }

            if (damageable != null || movable != null)
            {
                objectsHitDuringAttack.Add(other.transform.root);
            }
        }
    }

    public override void Activate()
    {
        //switch (getModSpot())
        //{
        //    case ModSpot.ArmL:
        //        Hit();
        //        break;
        //    case ModSpot.ArmR:
        //        Hit();
        //        break;
        //    case ModSpot.Head:
        //        break;
        //    case ModSpot.Legs:
        //        LayMine();
        //        break;
        //    default:
        //        break;
        //}
    }



    public override void AttachAffect(ref Stats playerStats, ref PlayerMovement playerMovement)
    {
        //TODO:Disable pickup collider
        this.playerStats = playerStats;
        pickupCollider.enabled = false;

        if (getModSpot() == ModSpot.Legs)
        {
            playerStats.Modify(StatType.MoveSpeed, legsMoveSpeedMod);
        }
        else
        {

        }
    }

    public override void DeActivate()
    {
        throw new NotImplementedException();
    }

    public override void DetachAffect()
    {
        pickupCollider.enabled = true;
        attackCollider.enabled = false;

        if (getModSpot() == ModSpot.Legs)
        {
            playerStats.Modify(StatType.MoveSpeed, 1f / legsMoveSpeedMod);
        }
        else
        {

        }
    }

    public void ChargedHit()
    {

    }

    public void Hit()
    {
        
    }

    #region Private Methods
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
        yield return new WaitForSeconds(armRegularStartupTime);
        ArmRegularActive();
    }

    private IEnumerator ArmRegularActive()
    {
        EnableAttackCollider();
        armRegularAttackHitboxIsActive = true;

        yield return new WaitForSeconds(armRegularActiveTime);
        ArmRegularCooldown();
    }

    private IEnumerator ArmRegularCooldown()
    {
        DisableAttackCollider();
        armRegularAttackHitboxIsActive = false;
        objectsHitDuringAttack.Clear();

        yield return new WaitForSeconds(armRegularCooldownTime);
    }

    private IEnumerator ArmChargedStartup()
    {
        

        yield return new WaitForSeconds(armChargedStartupTime);
        ArmChargedActive();
    }

    private IEnumerator ArmChargedActive()
    {
        EnableAttackCollider();
        armChargedAttackHitboxIsActive = true;
        

        yield return new WaitForSeconds(armChargedActiveTime);
        ArmChargedCooldown();
    }

    private IEnumerator ArmChargedCooldown()
    {
        DisableAttackCollider();
        armChargedAttackHitboxIsActive = false;
        objectsHitDuringAttack.Clear();

        yield return new WaitForSeconds(armChargedCooldownTime);
    }
    #endregion

}
