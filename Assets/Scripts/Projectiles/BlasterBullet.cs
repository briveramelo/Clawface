using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class BlasterBullet : MonoBehaviour {

    private ShooterProperties shooterProperties=new ShooterProperties();
    private Damager damager = new Damager();
    private bool shooter;

    [SerializeField]
    private float killAfterSeconds;

    private float killTimer;

    void OnEnable()
    {
        killTimer = killAfterSeconds;
        
    }

    private IEnumerator<float> DestroyAfter() {
        yield return Timing.WaitForSeconds(3f);
        if (gameObject.activeSelf){
            EmitBulletCollision();
            gameObject.SetActive(false);
        }
    }

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

        transform.Translate(Vector3.forward * shooterProperties.speed * Time.deltaTime);
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetInstanceID()!=shooterProperties.shooterInstanceID){
            bool isEnemy = other.gameObject.CompareTag(Strings.Tags.ENEMY);
            bool isPlayer = other.gameObject.CompareTag(Strings.Tags.PLAYER);

            if ((shooter && isPlayer) || (!shooter && isEnemy) || other.gameObject.layer == (int) Layers.Ground) {                
                Damage(other.gameObject.GetComponent<IDamageable>());
                Push(other.gameObject.GetComponent<IMovable>());
                SFXManager.Instance.Play(SFXType.BlasterProjectileImpact, transform.position);
                EmitBulletCollision();
                gameObject.SetActive(false);
            }
        }
    }

    public void SetShooterProperties(ShooterProperties shooterProperties) {
        this.shooterProperties = shooterProperties;
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
                AnalyticsManager.Instance.AddModDamage(ModType.ArmBlaster, shooterProperties.damage);

                if (damageable.GetHealth() - shooterProperties.damage <= 0.01f)
                {
                    AnalyticsManager.Instance.AddModKill(ModType.ArmBlaster);
                }
            }
            else
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
        GameObject effect = ObjectPool.Instance.GetObject(PoolObjectType.VFXBlasterImpactEffect);
        if (effect) {
            effect.transform.position = transform.position;
        }    
    }

    public void SetWielderInstanceID(int id)
    {
        shooterProperties.shooterInstanceID = id;
    }

}