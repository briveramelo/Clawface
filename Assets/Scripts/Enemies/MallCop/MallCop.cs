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
    [SerializeField] private Mod mod;
    #endregion

    #region 3. Private fields


    private int stunCount;
    private Will will=new Will();

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
        ResetForRebirth();

        mod.setModSpot(ModSpot.ArmR);
        mod.AttachAffect(ref myStats, velBody);
    }    

    #endregion

    #region 5. Public Methods   

    void IDamageable.TakeDamage(float damage)
    {        
        if (myStats.health > 0){                        
            myStats.TakeDamage(damage);
            if (myStats.health <= 5 && !glowObject.isGlowing){
                glowObject.SetToGlow();
            }
            if (myStats.health <= 0) {
                controller.UpdateState(EMallCopState.Fall);

                mod.DetachAffect();
                OnDeath();
            }
            else {
                //TODO: update state to hit reaction state, THEN to chase (too abrupt right now)
                //TODO: Create hit reaction state
                if (controller.ECurrentState == EMallCopState.Patrol) {
                    controller.attackTarget = FindPlayer();
                    controller.UpdateState(EMallCopState.Chase);
                }
            }
        }
    }

    bool ISkinnable.IsSkinnable(){
        return myStats.health <= 5;
    }

    GameObject ISkinnable.DeSkin(){
        Invoke("Die", 0.1f);
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

    private Transform FindPlayer() {
        return GameObject.FindGameObjectWithTag(Strings.Tags.PLAYER).transform;
    }

    private void OnDeath() {
        if (will.willHasBeenWritten)
        {
            will.onDeath();
        }

        GameObject mallCopParts = ObjectPool.Instance.GetObject(PoolObjectType.MallCopExplosion);
        mallCopParts.transform.position = transform.position + Vector3.up*3f;
        mallCopParts.transform.rotation = transform.rotation;
        mallCopParts.DeActivate(5f);        
        gameObject.SetActive(false);
    }

    private void ResetForRebirth() {
        GetComponent<CapsuleCollider>().enabled = true;
        myStats.ResetForRebirth();
        controller.ResetForRebirth();
        velBody.ResetForRebirth();
        glowObject.ResetForRebirth();
        //TODO check for missing mod and create a new one and attach it
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

public class Will {
    public OnDeath onDeath;
    public bool willHasBeenWritten;
    public bool deathDocumented;
}