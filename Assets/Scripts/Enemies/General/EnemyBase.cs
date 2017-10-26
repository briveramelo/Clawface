using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : RoutineRunner, IStunnable, IDamageable, ISkinnable, ISpawnable
{

    [SerializeField] protected AIController controller;
    [SerializeField] protected VelocityBody velBody;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Stats myStats;
    [SerializeField] protected NavMeshAgent navAgent;
    [SerializeField] protected NavMeshObstacle navObstacle;
    [SerializeField] protected int skinHealth;
    [SerializeField] protected CopUI copUICanvas;
    [SerializeField] protected Transform bloodEmissionLocation;
    [SerializeField] protected int scorePopupDelay = 2;
    [SerializeField] protected int scoreValue = 200;

    #region 3. Private fields
    private int stunCount;
    private bool lastChance = false;
    private bool alreadyStunned = false;
    private Collider[] playerColliderList = new Collider[10];
    #endregion

    #region 0. Protected fields
    protected Damaged damaged = new Damaged();
    protected DamagePack damagePack = new DamagePack();
    protected Will will = new Will();
    protected List<AIState> aiStates;
    #endregion

    #region 4. Unity Lifecycle

    private void OnEnable()
    {

        if (will.willHasBeenWritten)
        {
            ResetForRebirth();
        }
        navAgent.enabled = true;
    }

    public virtual void Awake()
    {
        ResetForRebirth();
        navAgent.speed = myStats.moveSpeed;
    }

    private void Update()
    {
        controller.Update();
    }

    #endregion

    #region 5. Public Methods   

    void IDamageable.TakeDamage(Damager damager)
    {
        if (myStats.health > 0)
        {
            myStats.TakeDamage(damager.damage);
            damagePack.Set(damager, damaged);
            SFXManager.Instance.Play(SFXType.MallCopHurt, transform.position);
            DamageFXManager.Instance.EmitDamageEffect(damagePack);

            if (myStats.health <= 0)
            {
                if (alreadyStunned)
                    lastChance = true;

                if (lastChance && alreadyStunned)
                {
                    OnDeath();
                }
                else
                {
                    myStats.health = 1;
                    lastChance = true;
                }
            }


            if (myStats.health <= myStats.skinnableHealth)
            {
                copUICanvas.gameObject.SetActive(true);
                copUICanvas.ShowAction(ActionType.Skin);
                if (!alreadyStunned)
                {
                    myStats.health = 1;
                    alreadyStunned = true;
                }
            }


        }
    }

    float IDamageable.GetHealth()
    {
        return myStats.health;
    }

    void ISpawnable.WarpToNavMesh(Vector3 position)
    {
        navAgent.Warp(position);
    }


    bool ISkinnable.IsSkinnable()
    {
        return myStats.health <= myStats.skinnableHealth;
    }

    int ISkinnable.DeSkin()
    {
        Invoke("OnDeath", 0.1f);
        return skinHealth;
    }

    void IStunnable.Stun()
    {
    }

    public bool HasWillBeenWritten() { return will.willHasBeenWritten; }

    public void RegisterDeathEvent(OnDeath onDeath)
    {
        will.willHasBeenWritten = true;
        will.onDeath = onDeath;
    }

    #endregion


    #region 6. Private Methods   

    public virtual void OnDeath()
    {
        if (!will.isDead)
        {
            will.isDead = true;
            if (will.willHasBeenWritten)
            {
                will.onDeath();
            }

            UpgradeManager.Instance.AddEXP(Mathf.FloorToInt(myStats.exp));

            GameObject worldScoreObject = ObjectPool.Instance.GetObject(PoolObjectType.WorldScoreCanvas);
            if (worldScoreObject)
            {
                worldScoreObject.GetComponent<Canvas>().GetComponent<RectTransform>().SetPositionAndRotation(transform.position, transform.rotation);                //worldScoreObject.transform.position = transform.position /*+ Vector3.up * 3f*/;
                WorldScoreUI popUpScore = worldScoreObject.GetComponent<WorldScoreUI>();

                int scoreBonus = scoreValue * ScoreManager.Instance.GetCurrentMultiplier();
                popUpScore.DisplayScoreAndHide(scoreBonus, scorePopupDelay);
                ScoreManager.Instance.AddToScoreAndCombo(scoreBonus);
            }


            gameObject.SetActive(false);
        }
    }

    public virtual void ResetForRebirth()
    {
        GetComponent<CapsuleCollider>().enabled = true;
        myStats.ResetForRebirth();
        controller.ResetForRebirth();
        velBody.ResetForRebirth();
        will.Reset();
        lastChance = false;
        alreadyStunned = false;
    }
    #endregion
}
