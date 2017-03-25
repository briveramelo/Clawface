using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunMine : MonoBehaviour {

    [SerializeField]
    private float damage;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Strings.Tags.ENEMY)
        {
            GameObject stunMineExplosionEffect = ObjectPool.Instance.GetObject(PoolObjectType.MineExplosionEffect);
            stunMineExplosionEffect.transform.position = transform.position;
            stunMineExplosionEffect.transform.rotation = transform.rotation;

            other.GetComponent<IDamageable>().TakeDamage(damage);
            other.GetComponent<IStunnable>().Stun();
            gameObject.SetActive(false);
        }
    }
}
