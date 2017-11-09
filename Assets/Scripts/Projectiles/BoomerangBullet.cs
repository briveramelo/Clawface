﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class BoomerangBullet : MonoBehaviour {

    private Damager damager = new Damager();
    private bool shooter;
    private TrailRenderer trail;
    private float speed;
    private float damage;
    private float timeUntilDestroyed;
    private float rayDistanceMultiplier;
    private LayerMask raycastLayermask;
    

    void OnEnable()
    {        
        Timing.RunCoroutine(DestroyAfter());
        trail = GetComponent<TrailRenderer>();
    }

    private IEnumerator<float> DestroyAfter() {
        yield return Timing.WaitForSeconds(timeUntilDestroyed);
        if (gameObject.activeSelf){
            EmitBulletCollision();
            trail.Clear();
            gameObject.SetActive(false);
        }
    }

    void Update () {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
	}

    private void LateUpdate()
    {
        RaycastHit hit;
        Ray ray = new Ray(this.transform.position, transform.forward);

        float rayDistance = speed * Time.deltaTime * rayDistanceMultiplier;

        Debug.DrawLine(this.transform.position, this.transform.position + (this.transform.forward * rayDistance));

        if (Physics.Raycast(ray, out hit, rayDistance, raycastLayermask))
        {
            if (hit.collider.CompareTag(Strings.Tags.WALL))
            {
                Vector3 incomingVec = hit.point - this.transform.position;
                Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);
                transform.forward = reflectVec;
                EmitBulletCollision();
                SFXManager.Instance.Play(SFXType.BlasterProjectileImpact, transform.position);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Strings.Tags.ENEMY)
        {
            Damage(other.gameObject.GetComponent<IDamageable>());               
        }
    }

    public void Initialize(float speed, float damage, float timeUntilDestroyed, float rayDistanceMultiplier, LayerMask raycastLayermask)
    {
        this.speed = speed;
        this.damage = damage;
        this.timeUntilDestroyed = timeUntilDestroyed;
        this.rayDistanceMultiplier = rayDistanceMultiplier;
        this.rayDistanceMultiplier = rayDistanceMultiplier;
    }

    // 0 = Player, Enemy = 1
    public void SetShooterType(bool playerOrEnemy)
    {
        shooter = playerOrEnemy;
    }

    private void Damage(IDamageable damageable) {        
        if (damageable != null) {

            // Shooter is player
            if (!shooter)
            {
                AnalyticsManager.Instance.AddModDamage(ModType.Boomerang, damage);

                if (damageable.GetHealth() - damage <= 0.01f)
                {
                    AnalyticsManager.Instance.AddModKill(ModType.Boomerang);
                }
            }

            damager.Set(damage, DamagerType.Boomerang, transform.forward);
            damageable.TakeDamage(damager);
        }
    }

    private void EmitBulletCollision() {
        GameObject effect = ObjectPool.Instance.GetObject(PoolObjectType.VFXBoomerangImpact);
        if (effect) {
            effect.transform.position = transform.position;
        }    
    }

}