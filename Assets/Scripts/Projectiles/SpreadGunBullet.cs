using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadGunBullet : MonoBehaviour {

    #region Private variables
    private float speed;
    private float maxDistance;
    private float damage;
    private bool isReady;
    private Vector3 initPosition;
    #endregion

    #region Unity lifecycle
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (isReady)
        {
            //Move and shit
            transform.position += transform.forward * speed * Time.deltaTime;
            float distanceTravelled = Vector3.Distance(initPosition, transform.position);
            if(distanceTravelled > maxDistance)
            {
                ResetBullet();
            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {        
        if(other.tag == Strings.Tags.ENEMY)
        {
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Damager damager = new Damager();
                damager.damage = damage;
                damager.damagerType = DamagerType.SpreadGun;
                damageable.TakeDamage(damager);
            }
            ResetBullet();
        }
        else if (other.transform.CompareTag(Strings.Tags.WALL))
        {
            ResetBullet();
        }
    }
    #endregion

    #region Public functions
    public void Init(float speed, float maxDistance, float damage)
    {
        this.speed = speed;
        this.maxDistance = maxDistance;
        this.damage = damage;
        isReady = true;
        initPosition = transform.position;
    }
    #endregion

    #region Private functions
    private void ResetBullet()
    {        
        isReady = false;
        gameObject.SetActive(false);
        transform.SetParent(ObjectPool.Instance.transform);
    }
    #endregion

}
