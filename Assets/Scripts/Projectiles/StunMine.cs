using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunMine : MonoBehaviour {

    [SerializeField]
    private float damage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Strings.ENEMY)
        {
            GameObject stunMineExplosionEffect = ObjectPool.Instance.GetObject(PoolObjectType.MineExplosionEffect);
            stunMineExplosionEffect.SetActive(true);
            stunMineExplosionEffect.transform.position = transform.position;
            stunMineExplosionEffect.transform.rotation = transform.rotation;

            other.GetComponent<IDamageable>().TakeDamage(damage);
            other.GetComponent<IStunnable>().Stun();
            gameObject.SetActive(false);
        }
    }
}
