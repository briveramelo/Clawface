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
    private float legsMoveSpeedMod;

    [SerializeField]
    private float dampenForcesOnPlayerBy;
    #endregion

    #region Privates
    private float savedMoveSpeed;
    private float savedDampenForces;
    private float chargeTimer;

    private bool attackHitboxIsActive;
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
        Invoke("EnableAttackCollider", armRegularStartupTime);
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
        yield return new WaitForSeconds(armRegularStartupTime);
        ArmRegularActive();
    }

    private IEnumerator ArmRegularActive()
    {
        EnableAttackCollider();
        attackHitboxIsActive = true;

        yield return new WaitForSeconds(armRegularActiveTime);
        ArmRegularCooldown();
    }

    private IEnumerator ArmRegularCooldown()
    {
        DisableAttackCollider();
        attackHitboxIsActive = false;

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
        attackHitboxIsActive = true;
        

        yield return new WaitForSeconds(armChargedActiveTime);
        ArmChargedCooldown();
    }

    private IEnumerator ArmChargedCooldown()
    {
        DisableAttackCollider();
        attackHitboxIsActive = false;

        yield return new WaitForSeconds(armChargedCooldownTime);
    }

}
