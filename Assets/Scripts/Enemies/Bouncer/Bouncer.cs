using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using ModMan;

[System.Serializable]
public class BouncerProperties : AIProperties
{
}

public class Bouncer : EnemyBase
{

    #region 2. Serialized Unity Inspector Fields
    [SerializeField] private BouncerProperties properties;
    [SerializeField] private BulletHellPatternController bulletPatternController;
    [SerializeField] private HitTrigger hitTrigger;
    [SerializeField] private SpriteRenderer shadowOutline;
    #endregion

    #region 3. Private fields
    private int maxBounces;
    private int minBounces;
    private int maxShots;
    private int minShots;
    private float rotationSpeed;
    private bool rotate;
    private bool isUp;

    //The AI States of the Zombie
    private BouncerChaseState chase;
    private BouncerFireState fire;
    private BouncerStunState stun;
    private BouncerCelebrateState celebrate;
    private BouncerGetUpState getUp;


    #endregion

    #region 4. Unity Lifecycle


    protected override void Awake()
    {        

		isUp = false;
        myStats = GetComponent<Stats>();
        SetAllStats();
        InitilizeStates();

        if (enemyType.Equals(SpawnType.Bouncer))
        {
            fire.animatorSpeed = EnemyStatsManager.Instance.bouncerStats.animationShootSpeed;
        }

        else if (enemyType.Equals(SpawnType.RedBouncer))
        {
            fire.animatorSpeed = EnemyStatsManager.Instance.redBouncerStats.animationShootSpeed;
        }

        else if (enemyType.Equals(SpawnType.GreenBouncer))
        {
            fire.animatorSpeed = EnemyStatsManager.Instance.greenBouncerStats.animationShootSpeed;
        }

        controller.Initialize(properties, velBody, animator, myStats, navAgent, navObstacle, bulletPatternController, aiStates);
        damaged.Set(DamagedType.MallCop, bloodEmissionLocation);

        controller.checksToUpdateState = new List<Func<bool>>() {
            CheckDoneGettingUp,
            CheckPlayerDead,
            CheckToAttack,
            CheckToFinishAttacking,
            CheckIfStunned

        };

        base.Awake();
    }

    #endregion

    #region 5. Public Methods  

    bool CheckDoneGettingUp()
    {
        if (!isUp)
        {
            return true;
        }
        return false;
    }

    bool CheckPlayerDead()
    {
        if (AIManager.Instance.GetPlayerDead())
        {
            if (myStats.health > myStats.skinnableHealth && !celebrate.isCelebrating())
            {
                chase.StopCoroutines();
                controller.CurrentState = celebrate;
                controller.UpdateState(EAIState.Celebrate);
                return true;
            }
        }
        return false;
    }


    bool CheckToAttack()
    {
        if (controller.CurrentState == chase && chase.OverMaxJumpCount())
        {
            controller.UpdateState(EAIState.Fire);
            return true;
        }
        return false;
    }
    bool CheckToFinishAttacking()
    {
        if (controller.CurrentState == fire)
        {
            if (myStats.health <= myStats.skinnableHealth)
            {
                controller.CurrentState = stun;
                controller.UpdateState(EAIState.Stun);
                controller.DeActivateAI();
            }

            else if (fire.DoneFiring())
            {
                controller.UpdateState(EAIState.Chase);
            }
            return true;
        }
        return false;
    }
    bool CheckIfStunned()
    {
        if (myStats.health <= myStats.skinnableHealth || alreadyStunned)
        {
            chase.gotStunned = true;
            controller.CurrentState = stun;
            controller.UpdateState(EAIState.Stun);
            controller.DeActivateAI();
            return true;
        }
        return false;

    }

    public void DoneJumpStart()
    {
        chase.doneStartingJump = true;
        shadowOutline.gameObject.SetActive(true);
    }

    public void DoneJumpLanding()
    {
        SFXManager.Instance.Play(SFXType.BouncerLand, transform.position);
        chase.doneLandingJump = true;
        shadowOutline.gameObject.SetActive(false);
    }

    public void ActivateHitTrigger()
    {
        hitTrigger.ActivateTriggerDamage();
    }

    public void DeactivateHitTrigger()
    {
        hitTrigger.DeactivateTriggerDamage();
    }

    public void DamageAttackTarget()
    {
        chase.Damage(controller.AttackTarget.gameObject.GetComponent<IDamageable>());
    }

    public override void OnDeath()
    {
        isUp = false;
        base.OnDeath();
    }

    public override void ResetForRebirth()
    {
        base.ResetForRebirth();
        shadowOutline.gameObject.SetActive(false);
    }

    public void GetUpDone()
    {
        isUp = true;
        controller.CurrentState = chase;
        controller.UpdateState(EAIState.Chase);
    }

    public void FireBullet()
    {
        if (fire.shotCount >= fire.maxShots)
        {
            fire.doneFiring = true;
        }
        else
        {
            fire.FireBullet();
            fire.shotCount++;
        }
    }


    public override void DoPlayerKilledState(object[] parameters)
    {
    }

    public override Vector3 ReCalculateTargetPosition()
    {
        return Vector3.zero;
    }

    #endregion

    #region Protected Methods
    public override void GrabObject(Transform grabberTransform) {
        base.GrabObject(grabberTransform);
    }
    protected override bool IsFalling() {
        //Special case for the mommy because the raycast needs to be slightly longer in distance 
        return !Physics.CheckSphere(hips.transform.position, 4.0f, LayerMask.GetMask(Strings.Layers.GROUND));
    }
    #endregion

    #region 6. Private Methods    

    private void InitilizeStates()
    {
        aiStates = new List<AIState>();
        chase = new BouncerChaseState(shadowOutline);
        chase.stateName = "chase";
        fire = new BouncerFireState();
        fire.stateName = "fire";
        stun = new BouncerStunState();
        stun.stateName = "stun";
        celebrate = new BouncerCelebrateState();
        celebrate.stateName = "celebrate";
        getUp = new BouncerGetUpState();
        getUp.stateName = "getUp";
        aiStates.Add(chase);
        aiStates.Add(fire);
        aiStates.Add(stun);
        aiStates.Add(celebrate);
        aiStates.Add(getUp);
    }

    private void SetAllStats()
    {
        if (enemyType.Equals(SpawnType.Bouncer))
        {
            myStats.health = EnemyStatsManager.Instance.bouncerStats.health;
            myStats.maxHealth = EnemyStatsManager.Instance.bouncerStats.maxHealth;
            myStats.skinnableHealth = EnemyStatsManager.Instance.bouncerStats.skinnableHealth;
            myStats.moveSpeed = EnemyStatsManager.Instance.bouncerStats.speed;
            myStats.attack = EnemyStatsManager.Instance.bouncerStats.attack;

            navAgent.speed = EnemyStatsManager.Instance.bouncerStats.speed;
            navAgent.angularSpeed = EnemyStatsManager.Instance.bouncerStats.angularSpeed;
            navAgent.acceleration = EnemyStatsManager.Instance.bouncerStats.acceleration;
            navAgent.stoppingDistance = EnemyStatsManager.Instance.bouncerStats.stoppingDistance;

            scoreValue = EnemyStatsManager.Instance.bouncerStats.scoreValue;
            eatHealth = EnemyStatsManager.Instance.bouncerStats.eatHealth;
            stunnedTime = EnemyStatsManager.Instance.bouncerStats.stunnedTime;

            properties.maxBounces = EnemyStatsManager.Instance.bouncerStats.maxBounces;
            properties.minBounces = EnemyStatsManager.Instance.bouncerStats.minBounces;
            properties.maxShots = EnemyStatsManager.Instance.bouncerStats.maxShots;
            properties.minShots = EnemyStatsManager.Instance.bouncerStats.minShots;
            properties.rotationSpeed = EnemyStatsManager.Instance.bouncerStats.rotationSpeed;
            properties.rotate = EnemyStatsManager.Instance.bouncerStats.rotate;

            bulletPatternController.SetBulletHellProperties(EnemyStatsManager.Instance.bouncerStats.separationFromForwardVector, EnemyStatsManager.Instance.bouncerStats.bulletSpeed, EnemyStatsManager.Instance.bouncerStats.bulletDamage, EnemyStatsManager.Instance.bouncerStats.rateOfFire, EnemyStatsManager.Instance.bouncerStats.bulletOffsetFromOrigin, EnemyStatsManager.Instance.bouncerStats.bulletStrands, EnemyStatsManager.Instance.bouncerStats.separationAngleBetweenStrands, EnemyStatsManager.Instance.bouncerStats.rotationDirection, EnemyStatsManager.Instance.bouncerStats.rotationSpeed,
                EnemyStatsManager.Instance.bouncerStats.bulletLiveTime, EnemyStatsManager.Instance.bouncerStats.animationDriven);

            
        }
        else if (enemyType.Equals(SpawnType.RedBouncer))
        {
            myStats.health = EnemyStatsManager.Instance.redBouncerStats.health;
            myStats.maxHealth = EnemyStatsManager.Instance.redBouncerStats.maxHealth;
            myStats.skinnableHealth = EnemyStatsManager.Instance.redBouncerStats.skinnableHealth;
            myStats.moveSpeed = EnemyStatsManager.Instance.redBouncerStats.speed;
            myStats.attack = EnemyStatsManager.Instance.redBouncerStats.attack;

            navAgent.speed = EnemyStatsManager.Instance.redBouncerStats.speed;
            navAgent.angularSpeed = EnemyStatsManager.Instance.redBouncerStats.angularSpeed;
            navAgent.acceleration = EnemyStatsManager.Instance.redBouncerStats.acceleration;
            navAgent.stoppingDistance = EnemyStatsManager.Instance.redBouncerStats.stoppingDistance;

            scoreValue = EnemyStatsManager.Instance.redBouncerStats.scoreValue;
            eatHealth = EnemyStatsManager.Instance.redBouncerStats.eatHealth;
            stunnedTime = EnemyStatsManager.Instance.redBouncerStats.stunnedTime;

            properties.maxBounces = EnemyStatsManager.Instance.redBouncerStats.maxBounces;
            properties.minBounces = EnemyStatsManager.Instance.redBouncerStats.minBounces;
            properties.maxShots = EnemyStatsManager.Instance.redBouncerStats.maxShots;
            properties.minShots = EnemyStatsManager.Instance.redBouncerStats.minShots;
            properties.rotationSpeed = EnemyStatsManager.Instance.redBouncerStats.rotationSpeed;
            properties.rotate = EnemyStatsManager.Instance.redBouncerStats.rotate;

            bulletPatternController.SetBulletHellProperties(EnemyStatsManager.Instance.redBouncerStats.separationFromForwardVector, EnemyStatsManager.Instance.redBouncerStats.bulletSpeed, EnemyStatsManager.Instance.redBouncerStats.bulletDamage, EnemyStatsManager.Instance.redBouncerStats.rateOfFire, EnemyStatsManager.Instance.redBouncerStats.bulletOffsetFromOrigin, EnemyStatsManager.Instance.redBouncerStats.bulletStrands, EnemyStatsManager.Instance.redBouncerStats.separationAngleBetweenStrands, EnemyStatsManager.Instance.redBouncerStats.rotationDirection, EnemyStatsManager.Instance.redBouncerStats.rotationSpeed,
                EnemyStatsManager.Instance.redBouncerStats.bulletLiveTime, EnemyStatsManager.Instance.redBouncerStats.animationDriven);

           
        }
        else if (enemyType.Equals(SpawnType.GreenBouncer))
        {
            myStats.health = EnemyStatsManager.Instance.greenBouncerStats.health;
            myStats.maxHealth = EnemyStatsManager.Instance.greenBouncerStats.maxHealth;
            myStats.skinnableHealth = EnemyStatsManager.Instance.greenBouncerStats.skinnableHealth;
            myStats.moveSpeed = EnemyStatsManager.Instance.greenBouncerStats.speed;
            myStats.attack = EnemyStatsManager.Instance.greenBouncerStats.attack;

            navAgent.speed = EnemyStatsManager.Instance.greenBouncerStats.speed;
            navAgent.angularSpeed = EnemyStatsManager.Instance.greenBouncerStats.angularSpeed;
            navAgent.acceleration = EnemyStatsManager.Instance.greenBouncerStats.acceleration;
            navAgent.stoppingDistance = EnemyStatsManager.Instance.greenBouncerStats.stoppingDistance;

            scoreValue = EnemyStatsManager.Instance.greenBouncerStats.scoreValue;
            eatHealth = EnemyStatsManager.Instance.greenBouncerStats.eatHealth;
            stunnedTime = EnemyStatsManager.Instance.greenBouncerStats.stunnedTime;

            properties.maxBounces = EnemyStatsManager.Instance.greenBouncerStats.maxBounces;
            properties.minBounces = EnemyStatsManager.Instance.greenBouncerStats.minBounces;
            properties.maxShots = EnemyStatsManager.Instance.greenBouncerStats.maxShots;
            properties.minShots = EnemyStatsManager.Instance.greenBouncerStats.minShots;
            properties.rotationSpeed = EnemyStatsManager.Instance.greenBouncerStats.rotationSpeed;
            properties.rotate = EnemyStatsManager.Instance.greenBouncerStats.rotate;

            bulletPatternController.SetBulletHellProperties(EnemyStatsManager.Instance.greenBouncerStats.separationFromForwardVector, EnemyStatsManager.Instance.greenBouncerStats.bulletSpeed, EnemyStatsManager.Instance.greenBouncerStats.bulletDamage, EnemyStatsManager.Instance.greenBouncerStats.rateOfFire, EnemyStatsManager.Instance.greenBouncerStats.bulletOffsetFromOrigin, EnemyStatsManager.Instance.greenBouncerStats.bulletStrands, EnemyStatsManager.Instance.greenBouncerStats.separationAngleBetweenStrands, EnemyStatsManager.Instance.greenBouncerStats.rotationDirection, EnemyStatsManager.Instance.greenBouncerStats.rotationSpeed,
                EnemyStatsManager.Instance.greenBouncerStats.bulletLiveTime, EnemyStatsManager.Instance.greenBouncerStats.animationDriven);
        }

        myStats.SetStats();
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(transform.position, closeEnoughToAttackDistance);
    }


    #endregion
}