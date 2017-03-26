using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingBot : MonoBehaviour, IStunnable, IDamageable {

    [SerializeField] private GrapplingBotController controller;
    [SerializeField] private GrapplingBotProperties properties;
    [SerializeField] private VelocityBody velBody;
    [SerializeField] private Animator animator;
    [SerializeField] private Stats myStats;
    [SerializeField] private Mod mod;

    private void OnEnable() {
        ResetForRebirth();
    }

    void Awake() {
        controller.Initialize(properties, mod, velBody, animator, myStats);
    }

    void IStunnable.Stun() {
        if (controller.ECurrentState != EGrapplingBotState.Explode) {
            controller.UpdateState(EGrapplingBotState.Twitch);
        }
    }

    void IDamageable.TakeDamage(float damage) {
        myStats.TakeDamage(damage);
    }

    private void ResetForRebirth() {
        myStats.ResetForRebirth();
        controller.ResetForRebirth();
        velBody.ResetForRebirth();
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
}
