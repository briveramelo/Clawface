using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {


    #region Serialized
    [Tooltip("This is just for testing purposes, the actual variable that controls this in the MissileMod script!")]
    [SerializeField] private float testCloseDamageRadius;

    [Tooltip("This is just for testing purposes, the actual variable that controls this in the MissileMod script!")]
    [SerializeField] private float testFarDamageRadius;


    #endregion

    #region Private variables
    private float speed;
    
    private float closeDamage;
    private float farDamage;

    private bool isReady;

    private float highDamageRadius;
    private float lowDamageRadius;
    private Vector3 initPosition;

    private float deathTimer;

    private float timeTilDeath;
    #endregion

    #region Unity lifecycle
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (isReady)
        {
            deathTimer += Time.deltaTime;
            transform.position += transform.forward * speed;

            if (deathTimer > timeTilDeath)
            {
                Explode();
                ResetBullet();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Strings.Tags.ENEMY) || other.CompareTag(Strings.Tags.WALL))
        {
            Explode();
            ResetBullet();
        }
    }
    #endregion

    #region Public functions
    public void Init(float speed, float closeRadius, float farRadius, float closeDamage, float farDamage, float timeTilDeath)
    {
        this.speed = speed;
        this.highDamageRadius = closeRadius;
        this.lowDamageRadius = farRadius;
        this.closeDamage = closeDamage;
        this.farDamage = farDamage;
        this.timeTilDeath = timeTilDeath;
        isReady = true;
        initPosition = transform.position;
        deathTimer = 0f;
    }
    #endregion

    #region Private functions
    private void Explode()
    {
        Collider[] allHitObjects = Physics.OverlapSphere(transform.position, lowDamageRadius);
        Collider[] closeHitObjects = Physics.OverlapSphere(transform.position, highDamageRadius);

        List<Collider> farHitEnemies = new List<Collider>();
        List<Collider> closeHitEnemies = new List<Collider>();

        foreach (Collider c in allHitObjects)
        { 
            if (c.gameObject.CompareTag(Strings.Tags.ENEMY)) {
                farHitEnemies.Add(c);
            }
        }

        foreach (Collider c in closeHitObjects)
        {
            if (c.gameObject.CompareTag(Strings.Tags.ENEMY))
            {
                closeHitEnemies.Add(c);
            }

            if (farHitEnemies.Contains(c))
            {
                farHitEnemies.Remove(c);
            }
        }

        Damager damage = new Damager();
        damage.damagerType = DamagerType.Dice;
        damage.impactDirection = Vector3.zero;
        damage.damage = farDamage;

        foreach (Collider c in farHitEnemies)
        {
            IDamageable damageable = c.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }

        damage.damage = closeDamage;
        foreach (Collider c in closeHitEnemies)
        {
            IDamageable damageable = c.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
    }

    private void ResetBullet()
    {
        isReady = false;
        deathTimer = 0f;
        gameObject.SetActive(false);
        transform.SetParent(ObjectPool.Instance.transform);
    }
    #endregion


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, testCloseDamageRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, testFarDamageRadius);

    }
}
