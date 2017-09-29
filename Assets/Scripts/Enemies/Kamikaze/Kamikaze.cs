using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Kamikaze : MonoBehaviour, IStunnable, IDamageable, ISkinnable, ISpawnable
{

    #region 2. Serialized Unity Inspector Fields
    [SerializeField] private KamikazeController controller;
    [SerializeField] private KamikazeProperties properties;
    [SerializeField] private VelocityBody velBody;
    [SerializeField] private GlowObject glowObject;
    [SerializeField] private Animator animator;
    [SerializeField] private Stats myStats;
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private GameObject mySkin;
    [SerializeField] private CopUI copUICanvas;
    [SerializeField] private Transform bloodEmissionLocation;
    [SerializeField] private int scorePopupDelay = 2;
    [SerializeField] private int scoreValue = 200;
    #endregion

    #region 3. Private fields


    private int stunCount;
    private Will will = new Will();
    private Damaged damaged = new Damaged();
    private DamagePack damagePack = new DamagePack();
    private bool lastChance;


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

    void Awake()
    {
        controller.Initialize(properties, velBody, animator, myStats, navAgent);
        ResetForRebirth();
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
            damaged.Set(DamagedType.Bouncer, bloodEmissionLocation);
            DamageFXManager.Instance.EmitDamageEffect(damagePack);
            if (myStats.health <= myStats.skinnableHealth && !glowObject.isGlowing)
            {
                //glowObject.SetToGlow();
                copUICanvas.gameObject.SetActive(true);
                copUICanvas.ShowAction(ActionType.Skin);
            }
            if (myStats.health <= 0)
            {
                    controller.UpdateState(EKamikazeState.SelfDestruct);
                    OnDeath();
            }
            else
            {
                controller.UpdateState(EKamikazeState.Chase);
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

    GameObject ISkinnable.DeSkin()
    {
        Invoke("OnDeath", 0.1f);
        return Instantiate(mySkin, null, false);
    }

    void IStunnable.Stun()
    {
        //stunCount++;
        //if (stunCount >= properties.numShocksToStun)
        //{
        //    stunCount = 0;
        //    if (myStats.health > 0)
        //    {
        //        controller.UpdateState(EMallCopState.Twitch);
        //    }
        //}
    }

    public bool HasWillBeenWritten() { return will.willHasBeenWritten; }

    public void RegisterDeathEvent(OnDeath onDeath)
    {
        will.willHasBeenWritten = true;
        will.onDeath = onDeath;
    }

    #endregion

    #region 6. Private Methods    

    private void OnDeath()
    {
        if (!will.isDead)
        {
            will.isDead = true;
            if (will.willHasBeenWritten)
            {
                will.onDeath();
            }

            UpgradeManager.Instance.AddEXP(Mathf.FloorToInt(myStats.exp));

            //Create a particle effect to show zombie died

            //GameObject zombieParts = ObjectPool.Instance.GetObject(PoolObjectType.MallCopExplosion);
            //if (zombieParts)
            //{
            //    SFXManager.Instance.Play(SFXType.BloodExplosion, transform.position);
            //    zombieParts.transform.position = transform.position + Vector3.up * 3f;
            //    zombieParts.transform.rotation = transform.rotation;
            //    zombieParts.DeActivate(5f);
            //}
            //mod.KillCoroutines();

            //grab score ui from pool to display
            GameObject worldScoreObject = ObjectPool.Instance.GetObject(PoolObjectType.WorldScoreCanvas);
            if (worldScoreObject)
            {
                worldScoreObject.GetComponent<Canvas>().GetComponent<RectTransform>().SetPositionAndRotation(transform.position, transform.rotation);                //worldScoreObject.transform.position = transform.position /*+ Vector3.up * 3f*/;
                WorldScoreUI popUpScore = worldScoreObject.GetComponent<WorldScoreUI>();

                int scoreBonus = scoreValue * ScoreManager.Instance.GetCurrentMultiplier();
                popUpScore.DisplayScoreAndHide(scoreBonus, scorePopupDelay);
                ScoreManager.Instance.AddToScoreAndCombo(scoreBonus);
            }


            //KILL SELF
            Death();
        }
    }

    private void ResetForRebirth()
    {
        GetComponent<CapsuleCollider>().enabled = true;
        copUICanvas.gameObject.SetActive(false);
        //mod.DeactivateModCanvas();

        myStats.ResetForRebirth();
        controller.ResetForRebirth();
        velBody.ResetForRebirth();
        glowObject.ResetForRebirth();
        will.Reset();
        //TODO check for missing mod and create a new one and attach it
        //mod.setModSpot(ModSpot.ArmR);
        lastChance = false;
    }

    private void Death()
    {
        navAgent.enabled = false;
        gameObject.SetActive(false);
    }

    #endregion


}

[System.Serializable]
public class KamikazeProperties
{
    public float runMultiplier;
    [Range(5f, 15f)] public float maxChaseTime;
    [Range(5f, 15f)] public float walkTime;
    [Range(1, 6)] public int numShocksToStun;
    [Range(.1f, 1)] public float twitchRange;
    [Range(.1f, 1f)] public float twitchTime;
}