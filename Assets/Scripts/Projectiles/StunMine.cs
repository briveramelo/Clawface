using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunMine : MonoBehaviour {

    private ProjectileProperties projectileProperties;

    public void SetProjectileProperties(ProjectileProperties projectileProperties) {
        this.projectileProperties = projectileProperties;
    }

    private void OnTriggerEnter(Collider other){
        if((other.gameObject.CompareTag(Strings.Tags.ENEMY) ||
           other.gameObject.CompareTag(Strings.Tags.PLAYER)) &&
           other.gameObject.GetInstanceID()!=projectileProperties.shooterInstanceID){
            GameObject stunMineExplosionEffect = ObjectPool.Instance.GetObject(PoolObjectType.MineExplosionEffect);
            if (stunMineExplosionEffect) {
                stunMineExplosionEffect.transform.position = transform.position;
                stunMineExplosionEffect.transform.rotation = transform.rotation;
            }
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable!=null) {
                damageable.TakeDamage(projectileProperties.damage);
            }
            IStunnable stunnable = other.GetComponent<IStunnable>();
            if (stunnable!=null) {
                stunnable.Stun();
            }
            gameObject.SetActive(false);
        }
    }
}
