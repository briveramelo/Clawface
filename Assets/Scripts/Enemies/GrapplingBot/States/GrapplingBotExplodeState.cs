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
        Explode();
    }

    IEnumerator<float> Inflate() {
        float remainingTime = properties.timeToInflate;
        while (remainingTime>0) {
            remainingTime -= Time.deltaTime;

            controller.transform.localScale += Vector3.one * properties.inflationRate;
            velBody.velocity = Vector3.up * properties.riseOnExplosionSpeed;
            Shake(remainingTime / properties.timeToInflate);
                        
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
        explosion.transform.position = controller.transform.position;
        List<Collider> nearbyColliders = Physics.OverlapSphere(controller.transform.position, properties.explosionRadius).ToList();
        nearbyColliders.ForEach(col => {
            IMovable movable = col.GetComponent<IMovable>();
            if (movable != null) {
                Vector3 explosionDirection = col.transform.position - controller.transform.position;
                float distanceToCollider = explosionDirection.magnitude;
                explosionDirection.Normalize();
                float explosionForce = properties.maxExplosionForce * (1-(distanceToCollider/properties.explosionRadius));//properties.maxExplosionForce;
                movable.AddDecayingForce(explosionDirection * explosionForce);
            }
        });

        controller.gameObject.SetActive(false);
    }

}
