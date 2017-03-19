//MallCop AI created by Lai, Brandon, Bharat

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ModMan;

public class MallCop : MonoBehaviour, ICollectable, IStunnable, IMovable, IDamageable, ISkinnable
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
    private OnDeath onDeath;
    private bool willHasBeenWritten;

    #endregion

    #region 4. Unity Lifecycle

    private void OnEnable() {
        if (willHasBeenWritten) {
            Revive();
        }       
    }
    
    void Awake ()
    {
        controller.Initialize(properties, mod, velBody, animator, myStats);
        Revive();

        MoveState dummy = null;
        mod.setModSpot(ModSpot.ArmR);
        mod.AttachAffect(ref myStats, ref dummy);
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
            if (myStats.health <= 0){
                controller.UpdateState(EMallCopState.Fall);
                
                mod.DetachAffect();
                Invoke("Die", 5f);
            }
        }
    }

    GameObject ICollectable.Collect(){
        GameObject droppedSkin = Instantiate(mySkin, null, true) as GameObject;
        return droppedSkin;
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

    void IMovable.AddDecayingForce(Vector3 forceVector, float decay)
    {
        StartCoroutine(velBody.AddDecayingForce(forceVector, decay));
    }

    public bool HasWillBeenWritten() { return willHasBeenWritten; }

    public void RegisterDeathEvent(OnDeath onDeath)
    {
        willHasBeenWritten = true;
        this.onDeath = onDeath;
    }
    
    #endregion

    #region 6. Private Methods

    private void Die() {
        if (willHasBeenWritten)
        {
            onDeath();
        }
        gameObject.SetActive(false);
    }

    private void Revive() {
        StopAllCoroutines();

        GetComponent<CapsuleCollider>().enabled = true;
        myStats.Reset();
        controller.Reset();
        glowObject.Reset();
        velBody.Reset();
        //TODO check for missing mod and create a new one and attach it
    }       

    #endregion

    #region 7. Internal Structures
    
    public delegate void OnDeath();
    
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
    [Range(.1f, 5f)] public float strikingDistance;
}
