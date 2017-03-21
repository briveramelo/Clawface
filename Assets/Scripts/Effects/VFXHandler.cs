using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXHandler {

    public Transform transform;
    public VFXHandler(Transform owner) {
        this.transform = owner;
    }


    public void EmitForBulletCollision() {
        GameObject effect = ObjectPool.Instance.GetObject(PoolObjectType.BlasterImpactEffect);
        effect.transform.position = transform.position;
    }


    public void EmitBloodBilaterally() {
        SetupBlood(ObjectPool.Instance.GetObject(PoolObjectType.BloodEmitter), true);
        SetupBlood(ObjectPool.Instance.GetObject(PoolObjectType.BloodEmitter), false);
    }

    public void EmitBloodInDirection(Quaternion emissionDirection, Vector3 spawnPoint) {
        GameObject bloodEmitter = ObjectPool.Instance.GetObject(PoolObjectType.BloodEmitter);
        ObjectPool.Instance.StartCoroutine(RestoreOriginalRotation(bloodEmitter));
        bloodEmitter.transform.position = spawnPoint;
        bloodEmitter.transform.rotation = emissionDirection;
    }

    private void SetupBlood(GameObject emitter, bool facingFront)
    {
        if (emitter != null)
        {
            emitter.transform.position = transform.position;
            Vector3 bulletAngs = transform.rotation.eulerAngles + (facingFront ? Vector3.zero : Vector3.up * 180f);
            Vector3 projectileAngs = emitter.transform.rotation.eulerAngles;
            emitter.transform.rotation = Quaternion.Euler(projectileAngs.x, bulletAngs.y, 0f);
        }
    }

    IEnumerator RestoreOriginalRotation(GameObject gameObjectToRestore) {
        Quaternion startRotation = gameObjectToRestore.transform.rotation;
        yield return new WaitForSeconds(3f);
        gameObjectToRestore.transform.rotation = startRotation;
    }
}
