using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeyserFissure : MonoBehaviour {

    private ShooterProperties shooterProperties = new ShooterProperties();
    private Damager damager = new Damager();

    [SerializeField] private float killAfterSeconds;
    [SerializeField] private PoolObjectType enemyImpactEffect;
    [SerializeField] private PoolObjectType wallImpactEffect;

    private List<Transform> hitEnemies;

    private float killTimer;

    private void Awake()
    {
        hitEnemies = new List<Transform>();
    }

    void OnEnable()
    {
        killTimer = killAfterSeconds;
        hitEnemies.Clear();
    }

    void Update()
    {
        killTimer -= Time.deltaTime;

        if (killTimer <= 0f)
        {
            if (gameObject.activeSelf)
            {
                SpawnPoolObjectAtCurrentPosition(wallImpactEffect);
                gameObject.SetActive(false);
            }
        }

        transform.Translate(Vector3.forward * shooterProperties.speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
 
        if (other.CompareTag(Strings.Tags.ENEMY))
        {

            IDamageable damageable = other.GetComponent<IDamageable>();

            if (damageable != null && !hitEnemies.Contains(other.transform))
            {
                Damage(damageable);
                SpawnPoolObjectAtCurrentPosition(enemyImpactEffect);
                hitEnemies.Add(other.transform);
            }
        }
        else if (other.CompareTag(Strings.Tags.WALL))
        {
            SpawnPoolObjectAtCurrentPosition(wallImpactEffect);
            gameObject.SetActive(false);
        }
    }

    public void SetShooterProperties(ShooterProperties shooterProperties)
    {
        this.shooterProperties = shooterProperties;
    }

    public void SetWielderInstanceID(int id)
    {
        shooterProperties.shooterInstanceID = id;
    }


    private void Damage(IDamageable damageable)
    {

        AnalyticsManager.Instance.AddModDamage(ModType.Geyser, shooterProperties.damage);

        if (damageable.GetHealth() - shooterProperties.damage <= 0.01f)
        {
            AnalyticsManager.Instance.AddModKill(ModType.Geyser);
        }

        damager.Set(shooterProperties.damage, DamagerType.Geyser, transform.forward);
        damageable.TakeDamage(damager);
    }


    private void SpawnPoolObjectAtCurrentPosition(PoolObjectType poolObject)
    {
        GameObject effect = ObjectPool.Instance.GetObject(poolObject);
        if (effect)
        {
            effect.transform.position = transform.position;
        }
    }
    
    
}
