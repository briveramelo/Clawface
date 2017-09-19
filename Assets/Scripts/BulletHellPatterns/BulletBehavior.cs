using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
using ModMan;
public class BulletBehavior : MonoBehaviour {

    Vector3 movementVector;
    float bulletSpeed;

    private ShooterProperties shooterProperties = new ShooterProperties();
    private Damager damager = new Damager();
    private bool shooter;

    string coroutineName { get { return GetHashCode().ToString(); } }
    private void OnDisable()
    {
        Timing.KillCoroutines(coroutineName);
    }

    private void Start()
    {
        shooterProperties.Initialize(0, 5, 6, 2);
        SetShooterProperties(shooterProperties);
        gameObject.DeActivate(3f);
    }

    



    public void AssignBulletValues(Vector3 forwardDirection, float speed)
    {
        movementVector = forwardDirection;
        bulletSpeed = speed;
        
    }

    void Update()
    {
        transform.position += movementVector * bulletSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
            bool isEnemy = other.gameObject.CompareTag(Strings.Tags.ENEMY);
            bool isPlayer = other.gameObject.CompareTag(Strings.Tags.PLAYER);
            if (isEnemy || isPlayer)
            {
                Damage(other.gameObject.GetComponent<IDamageable>());
                Push(other.gameObject.GetComponent<IMovable>());
            }
            if (isEnemy || isPlayer || other.gameObject.layer == (int)Layers.Ground)
            {
                SFXManager.Instance.Play(SFXType.BlasterProjectileImpact, transform.position);
                EmitBulletCollision();
                gameObject.DeActivate(0f);
        }
    }

    public void SetShooterProperties(ShooterProperties shooterProperties)
    {
        this.shooterProperties = shooterProperties;
    }


    private void Damage(IDamageable damageable)
    {
        if (damageable != null)
        {

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

    private void Push(IMovable movable)
    {
        Vector3 forceDirection = transform.forward;
        if (movable != null)
        {
            movable.AddDecayingForce(forceDirection.normalized * shooterProperties.pushForce);
        }
    }
    private void EmitBulletCollision()
    {
        GameObject effect = ObjectPool.Instance.GetObject(PoolObjectType.VFXBlasterImpactEffect);
        if (effect)
        {
            effect.transform.position = transform.position;
        }
    }

    public void SetWielderInstanceID(int id)
    {
        shooterProperties.shooterInstanceID = id;
    }


}
