//MallCop AI created by Lai, Brandon, Bharat

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ModMan;

public class MallCop : MonoBehaviour, IStunnable, IDamageable, ISkinnable, ISpawnable
{

    #region 2. Serialized Unity Inspector Fields
    [SerializeField] private MallCopController controller;
    [SerializeField] private MallCopProperties properties;
    [SerializeField] private VelocityBody velBody;
    [SerializeField] private GlowObject glowObject;
    [SerializeField] private Animator animator;
    [SerializeField] private Stats myStats;
    [SerializeField] private GameObject mySkin;
    [SerializeField] private CopUI copUICanvas;
    [SerializeField] private Mod mod;
    [SerializeField] private Transform bloodEmissionLocation;
    #endregion

    #region 3. Private fields


    private int stunCount;
    private Will will=new Will();
    private Damaged damaged = new Damaged();
    private DamagePack damagePack=new DamagePack();

    #endregion

    #region 4. Unity Lifecycle

    private void OnEnable() {
        if (will.willHasBeenWritten) {
            ResetForRebirth();
        }       
    }
    
    void Awake ()
    {
        controller.Initialize(properties, mod, velBody, animator, myStats);
        damaged.Set(DamagedType.MallCop, bloodEmissionLocation);
        mod.setModSpot(ModSpot.ArmR);
        mod.AttachAffect(ref myStats, velBody);
        ResetForRebirth();
    }    

    #endregion

    #region 5. Public Methods   

    void IDamageable.TakeDamage(Damager damager)
    {        
        if (myStats.health > 0){                        
            myStats.TakeDamage(damager.damage);            
            damagePack.Set(damager, damaged);
            SFXManager.Instance.Play(SFXType.MallCopHurt, transform.position);
            DamageFXManager.Instance.EmitDamageEffect(damagePack);
            if (myStats.health <= myStats.skinnableHealth && !glowObject.isGlowing){
                glowObject.SetToGlow();
                copUICanvas.gameObject.SetActive(true);
                copUICanvas.ShowAction(ActionType.Skin);
            }
            if (myStats.health <= 0) {
                controller.UpdateState(EMallCopState.Fall);

                //mod.DetachAffect();
                OnDeath();
            }
            else {
                //TODO: update state to hit reaction state, THEN to chase (too abrupt right now)
                //TODO: Create hit reaction state
                if (controller.ECurrentState == EMallCopState.Patrol) {
                    controller.UpdateState(EMallCopState.Chase);
                }
            }
        }
    }

    float IDamageable.GetHealth()
    {
        return myStats.health;
    }

    bool ISkinnable.IsSkinnable(){
        return myStats.health <= myStats.skinnableHealth;
    }

    GameObject ISkinnable.DeSkin(){
        Invoke("OnDeath", 0.1f);
        return Instantiate(mySkin, null, false);
    }

    void IStunnable.Stun(){
        stunCount++;
        if (stunCount >= properties.numShocksToStun)
        {
            stunCount = 0;
            if (myStats.health > 0 ) {
                controller.UpdateState(EMallCopState.Twitch);
            }
        }
    }    

    public bool HasWillBeenWritten() { return will.willHasBeenWritten; }

    public void RegisterDeathEvent(OnDeath onDeath)
    {
        will.willHasBeenWritten = true;
        will.onDeath = onDeath;
    }

    #endregion

    #region 6. Private Methods    

    private void OnDeath() {
        if (!will.isDead) {
            will.isDead=true;
            if (will.willHasBeenWritten)
            {
                will.onDeath();
            }

            UpgradeManager.Instance.AddEXP(Mathf.FloorToInt(myStats.exp));

            GameObject mallCopParts = ObjectPool.Instance.GetObject(PoolObjectType.MallCopExplosion);
            if (mallCopParts) {
                SFXManager.Instance.Play(SFXType.BloodExplosion, transform.position);
                mallCopParts.transform.position = transform.position + Vector3.up*3f;
                mallCopParts.transform.rotation = transform.rotation;
                mallCopParts.DeActivate(5f);                
            }
            mod.KillCoroutines();            
            gameObject.SetActive(false);
        }
    }

    private void ResetForRebirth() {
        GetComponent<CapsuleCollider>().enabled = true;
        copUICanvas.gameObject.SetActive(false);
        mod.DeactivateModCanvas();

        myStats.ResetForRebirth();
        controller.ResetForRebirth();
        velBody.ResetForRebirth();
        glowObject.ResetForRebirth();
        will.Reset();
        //TODO check for missing mod and create a new one and attach it
        mod.setModSpot(ModSpot.ArmR);        
    }       

    #endregion

    #region 7. Internal Structures            
    #endregion

}

[System.Serializable]
public class MallCopProperties {
    public float runMultiplier;
    [Range(5f, 15f)] public float maxChaseTime;
    [Range(5f, 15f)] public float walkTime;
    [Range(1, 6)] public int numShocksToStun;
    [Range(.1f, 1)] public float twitchRange;
    [Range(.1f, 1f)] public float twitchTime;    
}