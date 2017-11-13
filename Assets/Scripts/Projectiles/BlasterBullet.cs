﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class BlasterBullet : MonoBehaviour {

    #region private fields
    private Damager damager = new Damager();
    private bool shooter;
    private float killTimer;
    private float speed;
    private float damage;


    private TrailRenderer trail;
    #endregion    

    private void Awake()
    {
        trail = GetComponent<TrailRenderer>();
    }

    #region unity lifecycle
    void Update () {

        AdjustToPlayerHeight();

        killTimer -= Time.deltaTime;

        if (killTimer <= 0f)
        {
            if (gameObject.activeSelf)
            {
                EmitBulletCollision();
                gameObject.SetActive(false);
            }
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
	}

    private void OnTriggerEnter(Collider other)
    {
        bool isEnemy = other.gameObject.CompareTag(Strings.Tags.ENEMY);
        bool isPlayer = other.gameObject.CompareTag(Strings.Tags.PLAYER);

        if ((shooter && isPlayer) || (!shooter && isEnemy) || other.gameObject.layer == (int) Layers.Ground) {                
            Damage(other.gameObject.GetComponent<IDamageable>());                
            SFXManager.Instance.Play(SFXType.BlasterProjectileImpact, transform.position);
            EmitBulletCollision();
            gameObject.SetActive(false);
        }            

        if (other.transform.CompareTag(Strings.Tags.WALL))
        {
            EmitBulletCollision();
            gameObject.SetActive(false);
        }
    }
    #endregion

    #region public functions
    public void Initialize(float liveTime, float speed, float damage)
    {
        killTimer = liveTime;
        this.speed = speed;
        this.damage = damage;

        if (trail) trail.Clear();
    }

    public void SetShooterType(bool playerOrEnemy)
    {
        shooter = playerOrEnemy;
    }


    #endregion



    #region private function
    private IEnumerator<float> DestroyAfter()
    {
        yield return Timing.WaitForSeconds(3f);
        if (gameObject.activeSelf)
        {
            EmitBulletCollision();
            gameObject.SetActive(false);
        }
    }

    private void Damage(IDamageable damageable) {        
        if (damageable != null) {

            damager.Set(damage, DamagerType.BlasterBullet, transform.forward);
            damageable.TakeDamage(damager);
        }
    }

    private void EmitBulletCollision() {
        GameObject effect = ObjectPool.Instance.GetObject(PoolObjectType.VFXBlasterImpactEffect);
        if (effect) {
            effect.transform.position = transform.position;
        }    
    }

    private void AdjustToPlayerHeight()
    {
        float threshold = 1.7f;

        //The shooter is the enemy
        if (shooter == true)
        {
            if (transform.position.y >= threshold)
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.025f, transform.position.z);
        }

    }
    #endregion

}