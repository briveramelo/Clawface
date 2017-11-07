using MovementEffects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceBlock : MonoBehaviour, IMovable {

    #region Unity Inspector Fields
    [SerializeField]
    private Rigidbody rigid;

    [SerializeField]
    private Transform side1;

    [SerializeField]
    private Transform side2;

    [SerializeField]
    private Transform side3;

    [SerializeField]
    private Transform side4;

    [SerializeField]
    private Transform side5;

    [SerializeField]
    private Transform side6;

    [SerializeField]
    private float oneMultiply;

    [SerializeField]
    private float twoMultiply;

    [SerializeField]
    private float threeMultiply;

    [SerializeField]
    private float fourMultiply;

    [SerializeField]
    private float fiveMultiply;

    [SerializeField]
    private float sixMultiply;

    [SerializeField]
    private float sphereRadius;

    [SerializeField]
    private float explosionRadius;

    [SerializeField]
    private float forceStrength;

    [SerializeField]
    private float tumbleStrength;

    [SerializeField]
    private float decayingForceMultiplier;

    [SerializeField]
    private float timeTilExplosion;

    [SerializeField]
    private float startScale;

    [SerializeField]
    private float endScale;

    [SerializeField]
    private float scaleSpeed;

    [SerializeField]
    private float totalTimeAlive;

    #endregion

    #region Privates
    private Transform upSide;
    private float explosionTimer;
    private bool willExplode;
    private float timeAliveTimer;
    private DiceSide faceUpSide;
    private bool isExploding;
    private float drawExplosionTimer;
    private ShooterProperties shooterProperties = new ShooterProperties();
    private Transform assignedExplosionEffect;
    Damager damager = new Damager();

    private bool isPlayer;

    private DiceSide faceUpSideLastUpdate;
    #endregion

    #region Unity LifeCycle
    // Use this for initialization
    void Start()
    {
        this.gameObject.transform.localScale = new Vector3(startScale, startScale, startScale);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(side1.transform.position, sphereRadius);
        Gizmos.DrawWireSphere(side2.transform.position, sphereRadius);
        Gizmos.DrawWireSphere(side3.transform.position, sphereRadius);
        Gizmos.DrawWireSphere(side4.transform.position, sphereRadius);
        Gizmos.DrawWireSphere(side5.transform.position, sphereRadius);
        Gizmos.DrawWireSphere(side6.transform.position, sphereRadius);

        if (isExploding)
        {
            Gizmos.DrawWireSphere(this.transform.position, explosionRadius * GetMultiplierFromDiceSide());
        }
        
    }

    // Update is called once per frame
    void Update()
    {
            timeAliveTimer += Time.deltaTime;
            if (timeAliveTimer > totalTimeAlive) Explode();

            if (this.gameObject.transform.localScale.x < endScale)
            {
                this.gameObject.transform.localScale = new Vector3(this.gameObject.transform.localScale.x + scaleSpeed * Time.deltaTime,
                    this.gameObject.transform.localScale.y + scaleSpeed * Time.deltaTime, this.gameObject.transform.localScale.z + scaleSpeed * Time.deltaTime);
            }


            if (CheckGroundedSide(side1))
            {
                faceUpSide = DiceSide.One;
            }
            else if (CheckGroundedSide(side2))
            {
                faceUpSide = DiceSide.Two;
            }
            else if (CheckGroundedSide(side3))
            {
                faceUpSide = DiceSide.Three;
            }
            else if (CheckGroundedSide(side4))
            {
                faceUpSide = DiceSide.Four;
            }
            else if (CheckGroundedSide(side5))
            {
                faceUpSide = DiceSide.Five;
            }
            else if (CheckGroundedSide(side6))
            {
                faceUpSide = DiceSide.Six;
            }
            else
            {
                faceUpSide = DiceSide.None;
            }

            if (faceUpSide == faceUpSideLastUpdate && faceUpSide != DiceSide.None)
            {
                explosionTimer += Time.deltaTime;
            }
            else
            {
                explosionTimer = 0f;
            }

            if (willExplode && explosionTimer >= timeTilExplosion)
            {
                Explode();
            }




            faceUpSideLastUpdate = faceUpSide;
    }

    
    #endregion


    #region Public Methods
    void IMovable.AddDecayingForce(Vector3 force, float decay=0.1f) {
        rigid.AddForce(force * decayingForceMultiplier);
    }
    bool IMovable.IsGrounded() { return false;}
    void IMovable.SetMovementMode(MovementMode mode) { }
    Vector3 IMovable.GetForward() { return transform.forward;}
    Quaternion IMovable.GetRotation() { return transform.rotation;}


    public void AddRandomForce()
    {
        rigid.AddForce(new Vector3(Random.Range(0f, 1f), 0.3f, Random.Range(0f, 1f)) * forceStrength);
        rigid.AddTorque(Random.onUnitSphere * tumbleStrength);
        willExplode = true;
        explosionTimer = 0;
    }

    public void StopAndReset()
    {
        explosionTimer = 0f;
        timeAliveTimer = 0f;
        willExplode = false;
        rigid.velocity = new Vector3(0f, 0f, 0f);
        this.gameObject.transform.localScale = new Vector3(startScale, startScale, startScale);
        this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
    }

    public void Roll(Vector3 direction)
    {
        rigid.AddForce(direction.normalized * forceStrength);
        rigid.AddTorque(Random.onUnitSphere * tumbleStrength);
    }

    public void SetShooterType(bool isPlayer)
    {
        this.isPlayer = isPlayer;
    }

    public void PrimeExplosion(float explodeAfterHowLong)
    {
        timeTilExplosion = explodeAfterHowLong;
        willExplode = true;
        explosionTimer = 0;
        drawExplosionTimer = 0f;
    }

    public void SetShooterProperties(ShooterProperties shooter)
    {
        shooterProperties = shooter;
    }

    public float GetMultiplierFromDiceSide()
    {
        switch (faceUpSide)
        {
            case DiceSide.One:
                return oneMultiply;
            case DiceSide.Two:
                return twoMultiply;
            case DiceSide.Three:
                return threeMultiply;
            case DiceSide.Four:
                return fourMultiply;
            case DiceSide.Five:
                return fiveMultiply;
            case DiceSide.Six:
                return sixMultiply;
            case DiceSide.None:
                return threeMultiply;
        }

        return 0f;
    }
    #endregion


    #region Private Methods
    private void StartExplosionEffect()
    {
        this.gameObject.SetActive(false);
        StopAndReset();

        assignedExplosionEffect = ObjectPool.Instance.GetObject(PoolObjectType.VFXDiceBlockExplosion).transform;
        assignedExplosionEffect.position = this.transform.position;
        assignedExplosionEffect.localScale = new Vector3(1f, 1f, 1f) * GetMultiplierFromDiceSide();
    }

    private bool CheckGroundedSide(Transform side)
    {
        Collider[] cols = Physics.OverlapSphere(side.position, sphereRadius);
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].gameObject.layer == (int)Layers.Ground && cols[i].transform.root != this.transform.root)
            {
                upSide = side;
                return true;
                
            }
        }

        return false;
    }

    private void Explode()
    {
        List<IDamageable> damageableList = new List<IDamageable>();

        Collider[] cols = Physics.OverlapSphere(this.transform.position, explosionRadius * GetMultiplierFromDiceSide());
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].gameObject.GetInstanceID() != shooterProperties.shooterInstanceID)
            {
                damageableList.Add(cols[i].GetComponentInChildren<IDamageable>());
            }
        }

        foreach (IDamageable damageable in damageableList)
        {
            if (damageable != null)
            {
                damager.Set(shooterProperties.damage * GetMultiplierFromDiceSide(), DamagerType.Dice, Vector3.down);

                // Shooter is player

                AnalyticsManager.Instance.AddModDamage(ModType.Dice, damager.damage);

                if (damageable.GetHealth() - damager.damage <= 0.01f)
                {
                    AnalyticsManager.Instance.AddModKill(ModType.Dice);
                }




                damageable.TakeDamage(damager);
            }
        }

        StartExplosionEffect();

    }

    public void StopVerticalMovement() {
     //Suck a dick
    }

    #endregion

    #region Private Declarations
    private enum DiceSide {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        None = 7
    }

#endregion

}
