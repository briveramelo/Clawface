using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeyserFissure : MonoBehaviour {
       
    private Damager damager = new Damager();
    private float killAfterSeconds;
    private float speed;
    private float damage;

    [SerializeField] private PoolObjectType enemyImpactEffect;
    [SerializeField] private PoolObjectType wallImpactEffect;

    private List<Transform> hitEnemies;

    private float killTimer;

    private void Awake()
    {
        hitEnemies = new List<Transform>();
    }

    void Update()
    {
        killTimer += Time.deltaTime;

        if (killTimer >= killAfterSeconds)
        {

            SpawnPoolObjectAtCurrentPosition(wallImpactEffect);
            gameObject.SetActive(false);
            return;

        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
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


    private void Damage(IDamageable damageable)
    {
        damager.Set(damage, DamagerType.Geyser, transform.forward);
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
    
    public void Initialize(float speed, float damage, float killAfterSeconds)
    {
        this.speed = speed;
        this.damage = damage;
        this.killAfterSeconds = killAfterSeconds;
        this.killTimer = 0f;
        hitEnemies.Clear();
    }
    
}
