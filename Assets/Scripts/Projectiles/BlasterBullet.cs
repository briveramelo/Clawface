using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class BlasterBullet : MonoBehaviour {

    #region serialized fields

    [SerializeField] private PoolObjectType impactVFXType = PoolObjectType.VFXBlasterImpactEffect;

    #endregion

    #region private fields
    private Damager damager = new Damager();
    private bool shooter;
    private float killTimer;
    private float speed;
    private float damage;


    private TrailRenderer trail;
    #endregion
    
    #region unity lifecycle
    private void Awake()
    {
        trail = GetComponent<TrailRenderer>();
    }

    void Update () {

        //AdjustToPlayerHeight();

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
            DestroyBullet();
        }            

        if (other.transform.CompareTag(Strings.Tags.WALL))
        {
            DestroyBullet();
        }
    }
    #endregion

    #region public functions
    public void Initialize(float liveTime, float speed, float damage)
    {
        killTimer = liveTime;
        this.speed = speed;
        this.damage = damage;

        if (trail) trail.Clear();
    }

    public void SetShooterType(bool playerOrEnemy)
    {
        shooter = playerOrEnemy;
    }

    public void DestroyBullet()
    {
        EmitBulletCollision();
        gameObject.SetActive(false);
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

            damager.Set(damage, DamagerType.BlasterBullet, transform.forward);
            damageable.TakeDamage(damager);
        }
    }

    private void EmitBulletCollision() {
        GameObject effect = ObjectPool.Instance.GetObject(impactVFXType);
        if (effect) {
            effect.transform.position = transform.position;
            effect.transform.rotation = Quaternion.AngleAxis(180f, Vector3.up) * transform.rotation;
        }    
    }

    private void AdjustToPlayerHeight()
    {
        float threshold = 1.7f;

        //The shooter is the enemy
        if (shooter == true)
        {
            if (transform.position.y >= threshold)
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.025f, transform.position.z);
        }

    }
    #endregion

}