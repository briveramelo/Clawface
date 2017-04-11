using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
using MovementEffects;

public class GrapplingBot : MonoBehaviour, IStunnable, IDamageable, ISpawnable {

    [SerializeField] private GrapplingBotController controller;
    [SerializeField] private GrapplingBotProperties properties;
    [SerializeField] private VelocityBody velBody;
    [SerializeField] private Animator animator;
    [SerializeField] private Stats myStats;
    [SerializeField] private Mod mod;

    private Will will = new Will();    

    private void OnEnable() {
        if (will.willHasBeenWritten) {
            ResetForRebirth();
        }
    }

    void Start() {        
        controller.Initialize(properties, mod, velBody, animator, myStats, this);
        Timing.RunCoroutine(Begin());
    }

    IEnumerator<float> Begin() {
        yield return 0f;
        mod.AttachAffect(ref myStats, velBody);
    }

    void IStunnable.Stun() {
        if (controller.ECurrentState != EGrapplingBotState.Explode) {
            controller.UpdateState(EGrapplingBotState.Twitch);
        }
    }

    void IDamageable.TakeDamage(float damage) {
        myStats.TakeDamage(damage);
        if (myStats.health <= 0)
        {
            OnDeath();
        }
        else {
            if (controller.ECurrentState==EGrapplingBotState.Patrol) {
                controller.UpdateState(EGrapplingBotState.Approach);
            }
        }
    }
    
    float IDamageable.GetHealth()
    {
        return myStats.health;
    }    

    private void ResetForRebirth() {
        will.deathDocumented = false;
        myStats.ResetForRebirth();
        controller.ResetForRebirth();
        velBody.ResetForRebirth();
    }

    public bool HasWillBeenWritten() { return will.willHasBeenWritten; }

    public void RegisterDeathEvent(OnDeath onDeath)
    {
        will.willHasBeenWritten = true;
        will.onDeath = onDeath;
    }

    public void OnDeath()
    {
        if (!will.deathDocumented) {
            if (will.willHasBeenWritten)
            {
                will.onDeath();
            }
            will.deathDocumented = true;
            GameObject mallCopParts = ObjectPool.Instance.GetObject(PoolObjectType.MallCopExplosion);
            if (mallCopParts) {
                mallCopParts.transform.position = transform.position + Vector3.up * 3f;
                mallCopParts.transform.rotation = transform.rotation;
                mallCopParts.DeActivate(5f);
            }
            transform.parent.gameObject.SetActive(false);
        }
    }
}

[System.Serializable]
public class GrapplingBotProperties {
    public Transform rotationCenter;
    public float approachForce;
    public float grappleDistance;
    public float timeToInflate;
    public float inflationRate;
    public float riseOnExplosionSpeed;
    public float maxShakeRadius;
    public float explosionRadius;
    public float maxExplosionForce;
    public float maxExplosionDamage;
}
