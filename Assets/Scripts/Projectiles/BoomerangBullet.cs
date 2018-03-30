using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class BoomerangBullet : MonoBehaviour {

    #region Serialized
    [SerializeField]
    private TrailRenderer trail;

    #endregion

    #region Private Variables
    private Damager damager = new Damager();
    private bool shooter;
    
    private float speed;
    private float damage;
    private float timeUntilDestroyed;
    private float rayDistanceMultiplier;
    private LayerMask raycastLayermask;

    private float maxDistance;
    private int maxBounces;

    private float deathTimer;

    private int currentBounces = 0;
    private float currentDistanceTraveled = 0f;
    #endregion

    

    #region Unity Lifecycle
    void Update () {
        deathTimer += Time.deltaTime;

        if (deathTimer > timeUntilDestroyed || currentDistanceTraveled >= maxDistance || currentBounces > maxBounces)
        {
            ResetBullet();
            return;
        }

        Vector3 distanceVector = Vector3.forward * speed * Time.deltaTime;
        currentDistanceTraveled += distanceVector.magnitude;
        transform.Translate(distanceVector);
	}

    private void LateUpdate()
    {
        RaycastHit hit;
        Ray ray = new Ray(this.transform.position, transform.forward);

        float rayDistance = speed * Time.deltaTime * rayDistanceMultiplier;

        Debug.DrawLine(this.transform.position, this.transform.position + (this.transform.forward * rayDistance));

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.CompareTag(Strings.Tags.WALL))
            {
                Vector3 incomingVec = hit.point - this.transform.position;
                Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);
                transform.forward = reflectVec;
                SFXManager.Instance.Play(SFXType.BoomerangWallImpact, transform.position);
                currentBounces++;

                GameObject effect = ObjectPool.Instance.GetObject(PoolObjectType.VFXBoomerangImpact);
                if (effect)
                {
                    effect.transform.position = hit.point;
                }    
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag(Strings.Tags.ENEMY))
        {
            Damage(other.gameObject.GetComponent<IDamageable>());
        }
    }

    #endregion

    #region Publics
    public void Initialize(float speed, float damage, float timeUntilDestroyed, float rayDistanceMultiplier, LayerMask raycastLayermask, float maxDistance, int maxBounces)
    {
        this.speed = speed;
        this.damage = damage;
        this.timeUntilDestroyed = timeUntilDestroyed;
        this.rayDistanceMultiplier = rayDistanceMultiplier;
        this.rayDistanceMultiplier = rayDistanceMultiplier;
        this.currentBounces = 0;
        this.currentDistanceTraveled = 0f;
        this.trail.Clear();

        this.maxDistance = maxDistance;
        this.maxBounces = maxBounces;

        deathTimer = 0f;
    }

    // 0 = Player, Enemy = 1
    public void SetShooterType(bool playerOrEnemy)
    {
        shooter = playerOrEnemy;
    }

    public void ResetBullet()
    {
        trail.Clear();
        gameObject.SetActive(false);
        GameObject vfx = ObjectPool.Instance.GetObject(PoolObjectType.VFXBoomerangProjectileDie);
        if (vfx) vfx.transform.position = transform.position;
    }

    #endregion

    #region Privates
    private void Damage(IDamageable damageable) {        
        if (damageable != null) {

            damager.Set(damage, DamagerType.Boomerang, transform.forward);
            damageable.TakeDamage(damager);

            GameObject blood = ObjectPool.Instance.GetObject(PoolObjectType.VFXBloodSpurt);
            if (blood) blood.transform.position = damageable.GetPosition();
            SFXManager.Instance.Play(SFXType.Boomerang_Impact, damageable.GetPosition());
        }
    }

    private IEnumerator<float> DestroyAfter()
    {
        yield return Timing.WaitForSeconds(timeUntilDestroyed);
        if (gameObject.activeSelf)
        {
            ResetBullet();
        }
    }

    #endregion
}