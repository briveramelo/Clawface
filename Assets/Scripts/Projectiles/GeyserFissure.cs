using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeyserFissure : MonoBehaviour {
       
    private Damager damager = new Damager();
    private float killAfterSeconds;
    private float speed;
    private float damage;

    [SerializeField] FishEmitter fishEmitter;
    [SerializeField] private PoolObjectType enemyImpactEffect;
    [SerializeField] private PoolObjectType wallImpactEffect;
    [SerializeField] private float scaleUpTime = 0.5f;

    private List<Transform> hitEnemies;
    TrailRenderer[] trails;

    Transform effect;

    private float killTimer;

    private void Awake()
    {
        hitEnemies = new List<Transform>();
        trails = GetComponentsInChildren<TrailRenderer>();
        effect = transform.GetChild(0);
    }

    void Update()
    {
        killTimer += Time.deltaTime;

        if (killTimer >= killAfterSeconds)
        {

            SpawnPoolObjectAtCurrentPosition(wallImpactEffect);
            foreach (TrailRenderer trail in trails)
                trail.Clear();
            StopAllCoroutines();
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
                foreach (TrailRenderer trail in trails)
                {
                    trail.Clear();
                    trail.enabled = false;
                }
            }
        }
        else if (other.CompareTag(Strings.Tags.WALL))
        {
            SpawnPoolObjectAtCurrentPosition(wallImpactEffect);
            StopAllCoroutines();
            foreach (TrailRenderer trail in trails)
            {
                trail.Clear();
                trail.enabled = false;
            }
            gameObject.SetActive(false);
        }
    }

    IEnumerator ScaleUp ()
    {
        Vector3 originalScale = Vector3.one;
        effect.localScale = Vector3.zero;

        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime / scaleUpTime;

            effect.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);

            yield return null;
        }
    }


    private void Damage(IDamageable damageable)
    {
        damager.Set(damage, DamagerType.Geyser, transform.forward);
        damageable.TakeDamage(damager);

        GameObject blood = ObjectPool.Instance.GetObject(PoolObjectType.VFXBloodSpurt);
        if (blood) blood.transform.position = damageable.GetPosition();
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
        fishEmitter.SetBaseEmissionSpeed(speed*.4f);
        this.damage = damage;
        this.killAfterSeconds = killAfterSeconds;
        this.killTimer = 0f;
        hitEnemies.Clear();
        StartCoroutine (ScaleUp());
        foreach (TrailRenderer trail in trails)
        {
            trail.enabled = true;
            trail.Clear();
        }
    }
    
}
