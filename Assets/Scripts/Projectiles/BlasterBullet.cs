using System.Collections;
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
    #endregion    

    #region unity lifecycle
    void Update () {
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

            // Shooter is player
            if (!shooter)
            {
                AnalyticsManager.Instance.AddModDamage(ModType.Blaster, damage);

                if (damageable.GetHealth() - damage <= 0.01f)
                {
                    AnalyticsManager.Instance.AddModKill(ModType.Blaster);
                }
            }

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
    #endregion

}