using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class BoomerangBullet : MonoBehaviour {

    private ShooterProperties shooterProperties=new ShooterProperties();
    private Damager damager = new Damager();
    private bool shooter;

    [SerializeField] private float timeUntilDestroyed;
    [SerializeField] private float rayDistanceMultiplier;
    [SerializeField] private LayerMask raycastLayermask;

    void OnEnable()
    {        
        Timing.RunCoroutine(DestroyAfter());
    }

    private IEnumerator<float> DestroyAfter() {
        yield return Timing.WaitForSeconds(timeUntilDestroyed);
        if (gameObject.activeSelf){
            EmitBulletCollision();
            gameObject.SetActive(false);
        }
    }

    void Update () {
        transform.Translate(Vector3.forward * shooterProperties.speed * Time.deltaTime);

        
	}

    private void LateUpdate()
    {
        RaycastHit hit;
        Ray ray = new Ray(this.transform.position, transform.forward);

        float rayDistance = shooterProperties.speed * Time.deltaTime * rayDistanceMultiplier;

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
        if (other.gameObject.GetInstanceID()!=shooterProperties.shooterInstanceID){
            bool isEnemy = other.gameObject.CompareTag(Strings.Tags.ENEMY);
            if (isEnemy){                
                Damage(other.gameObject.GetComponent<IDamageable>());
                
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
                AnalyticsManager.Instance.AddModDamage(ModType.Boomerang, shooterProperties.damage);

                if (damageable.GetHealth() - shooterProperties.damage <= 0.01f)
                {
                    AnalyticsManager.Instance.AddModKill(ModType.Boomerang);
                }
            }
            else
            {
                AnalyticsManager.Instance.AddEnemyModDamage(ModType.Boomerang, shooterProperties.damage);
            }
            damager.Set(shooterProperties.damage, DamagerType.Boomerang, transform.forward);
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