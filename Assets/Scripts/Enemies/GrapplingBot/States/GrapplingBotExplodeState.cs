using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
using System.Linq;

public class GrapplingBotExplodeState : GrapplingBotState {    

    public override void OnEnter() {
        Timing.RunCoroutine(BeginExplosion());
    }
    public override void Update() {

    }
    public override void OnExit() {

    }

    IEnumerator<float> BeginExplosion() {
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(Inflate()));
        if (controller.gameObject.activeInHierarchy) {
            Explode();
            controller.OnDeath();
        }
    }

    IEnumerator<float> Inflate() {
        float remainingTime = properties.timeToInflate;        
        while (remainingTime>0) {
            remainingTime -= Time.deltaTime;

            controller.transform.localScale += Vector3.one * properties.inflationRate;
            velBody.velocity = Vector3.up * properties.riseOnExplosionSpeed;
            Shake(controller.timeInLastState / properties.timeToInflate);
                        
            yield return Timing.WaitForOneFrame;
        }
    }

    void Shake(float shakeMultipler) {
        Vector3 shakeDirection = Random.insideUnitSphere * properties.maxShakeRadius * shakeMultipler;
        shakeDirection.y = 0;
        velBody.velocity += shakeDirection;
    }

    void Explode() {
        GameObject explosion=ObjectPool.Instance.GetObject(PoolObjectType.MineExplosionEffect);
        if (explosion) {
            explosion.transform.position = controller.transform.position;
        }
        List<Collider> nearbyColliders = Physics.OverlapSphere(controller.transform.position, properties.explosionRadius).ToList();

        nearbyColliders.ForEach(col => {
            if (col.gameObject!=controller.gameObject) {
                float distanceToCollider = 0f;
                Vector3 explosionDirection = GetExplosionDirection(col.transform.position, out distanceToCollider);

                IMovable movable = col.GetComponent<IMovable>();
                if (movable != null) {
                    float explosionForce = properties.maxExplosionForce * (1-(distanceToCollider/properties.explosionRadius));
                    movable.AddDecayingForce(explosionDirection * explosionForce);
                }
                IDamageable damageable = col.GetComponent<IDamageable>();
                if (damageable != null) {
                    float explosionDamage = properties.maxExplosionDamage * (1 - (distanceToCollider / properties.explosionRadius));
                    damageable.TakeDamage(explosionDamage);
                }
            }
        });        
    }

    Vector3 GetExplosionDirection(Vector3 other, out float distanceToCollider) {
        Vector3 explosionDirection = other - controller.transform.position;
        distanceToCollider = explosionDirection.magnitude;
        explosionDirection.y = 0;
        explosionDirection.Normalize();
        return explosionDirection;
    }

}
