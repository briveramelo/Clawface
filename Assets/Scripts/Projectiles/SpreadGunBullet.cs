using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadGunBullet : MonoBehaviour {

    #region Private variables
    private float speed;
    private float maxLifeTime;
    private float maxDistance;
    private float damage;
    private bool isReady;
    private Vector3 initPosition;
    #endregion

    #region Unity lifecycle
    // Use this for initialization
    void Start () {
        isReady = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (isReady)
        {
            //Move and shit
            transform.Translate(transform.forward * speed);
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
            Damager damager = new Damager();
            damager.damage = damage;
            damager.damagerType = DamagerType.SpreadGun;
            damageable.TakeDamage(damager);
        }
        ResetBullet();
    }
    #endregion

    #region Public functions
    public void Init(float speed, float maxLifeTime, float maxDistance, float damage)
    {
        this.speed = speed;
        this.maxDistance = maxDistance;
        this.maxLifeTime = maxLifeTime;
        this.damage = damage;
        isReady = true;
        initPosition = transform.position;
        //Start death timer
        StartCoroutine(DeathTimer());
    }
    #endregion

    #region Private functions
    private void ResetBullet()
    {        
        isReady = false;
        gameObject.SetActive(false);
        StopAllCoroutines();
    }

    private IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(maxLifeTime);
        ResetBullet();
    }
    #endregion

}
