using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class BlasterBullet : MonoBehaviour {

    private ShooterProperties shooterProperties=new ShooterProperties();
    private Damager damager = new Damager();

    void OnEnable()
    {        
        Timing.RunCoroutine(DestroyAfter());
    }

    private IEnumerator<float> DestroyAfter() {
        yield return Timing.WaitForSeconds(3f);
        if (gameObject.activeSelf){
            EmitBulletCollision();
            gameObject.SetActive(false);
        }
    }

    void Update () {
        transform.Translate(Vector3.forward * shooterProperties.speed * Time.deltaTime);
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetInstanceID()!=shooterProperties.shooterInstanceID){
            bool isEnemy = other.gameObject.CompareTag(Strings.Tags.ENEMY);
            bool isPlayer = other.gameObject.CompareTag(Strings.Tags.PLAYER);
            if (isEnemy || isPlayer){                
                Damage(other.gameObject.GetComponent<IDamageable>());
                Push(other.gameObject.GetComponent<IMovable>());                
            }
            if (isEnemy || isPlayer || other.gameObject.layer==(int)Layers.Ground) {                
                gameObject.SetActive(false);
            }
        }
    }

    public void SetShooterProperties(ShooterProperties shooterProperties) {
        this.shooterProperties = shooterProperties;
    }

    private void Damage(IDamageable damageable) {        
        if (damageable != null) {

            if (this.transform.root.CompareTag(Strings.Tags.PLAYER))
            {
                AnalyticsManager.Instance.AddModDamage(ModType.ArmBlaster, shooterProperties.damage);

                if (damageable.GetHealth() - shooterProperties.damage <= 0.01f)
                {
                    AnalyticsManager.Instance.AddModKill(ModType.ArmBlaster);
                }
            }
            else if (this.transform.root.CompareTag(Strings.Tags.ENEMY))
            {
                AnalyticsManager.Instance.AddEnemyModDamage(ModType.ArmBlaster, shooterProperties.damage);
            }
            damager.Set(shooterProperties.damage, DamagerType.BlasterBullet, transform.forward);
            damageable.TakeDamage(damager);
        }
    }

    private void Push(IMovable movable) {
        Vector3 forceDirection = transform.forward;        
        if (movable != null) {
            movable.AddDecayingForce(forceDirection.normalized * shooterProperties.pushForce);
        }
    }
    private void EmitBulletCollision() {
        GameObject effect = ObjectPool.Instance.GetObject(PoolObjectType.BlasterImpactEffect);
        if (effect) {
            effect.transform.position = transform.position;
        }    
    }

}