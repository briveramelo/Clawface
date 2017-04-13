using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceBlock : MonoBehaviour {

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
    private float timeTilExplosion;

    [SerializeField]
    private float explosionDamage;

    [SerializeField]
    private float startScale;

    [SerializeField]
    private float endScale;

    [SerializeField]
    private float scaleSpeed;

    #endregion

    #region Privates
    private Transform upSide;
    private float explosionTimer;
    private bool willExplode;
    private float totalTimeAlive;
    private DiceSide faceUpSide;
    private bool drawExplosion;
    private float drawExplosionTimer;
    private ShooterProperties shooterProperties = new ShooterProperties();
    #endregion

    #region Unity LifeCycle
    // Use this for initialization
    void Start()
    {
        this.gameObject.transform.localScale = new Vector3(startScale, startScale, startScale);
    }

    private void OnDrawGizmos()
    {
        if (drawExplosion)
        {
            drawExplosionTimer += Time.deltaTime;
            if (drawExplosionTimer > 1.5f)
            {
                drawExplosion = false;
                StopAndReset();
            }

            Gizmos.DrawWireSphere(this.transform.position, explosionRadius * GetMultiplierFromDiceSide());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.activeSelf)
        {
            if (this.gameObject.transform.localScale.x < endScale)
            {
                this.gameObject.transform.localScale = new Vector3(this.gameObject.transform.localScale.x + scaleSpeed * Time.deltaTime, 
                    this.gameObject.transform.localScale.y + scaleSpeed * Time.deltaTime, this.gameObject.transform.localScale.z + scaleSpeed * Time.deltaTime);
            }

            totalTimeAlive += Time.deltaTime;

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

            if (faceUpSide != DiceSide.None)
            {
                explosionTimer += Time.deltaTime;
                if (willExplode && explosionTimer >= timeTilExplosion)
                {
                    Explode();
                }
            }
        }

        if (totalTimeAlive > 10f) Explode();

        
    }
    #endregion


    #region Public Methods
    public void AddRandomForce()
    {
        rigid.AddForce(new Vector3(Random.Range(0f, 1f), 0.3f, Random.Range(0f, 1f)) * forceStrength);
        rigid.AddTorque(Random.onUnitSphere * tumbleStrength);
        willExplode = true;
        explosionTimer = 0;
    }

    public void StopAndReset()
    {
        totalTimeAlive = 0f;
        gameObject.SetActive(false);
        willExplode = false;
        this.gameObject.transform.localScale = new Vector3(startScale, startScale, startScale);
        this.gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;
    }

    public void Roll(Vector3 direction)
    {
        rigid.AddForce(direction.normalized * forceStrength);
        rigid.AddTorque(Random.onUnitSphere * tumbleStrength);
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
                return 0f;
        }

        return 0f;
    }
    #endregion


    #region Private Methods
    private bool CheckGroundedSide(Transform side)
    {
        Collider[] cols = Physics.OverlapSphere(side.position, sphereRadius);
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].gameObject.layer == (int)Layers.Ground)
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
                Damager damager = new Damager();
                damager.Set(shooterProperties.damage * GetMultiplierFromDiceSide(), DamagerType.Dice, this.transform.position);
                Debug.Log(damager.damage);
                damageable.TakeDamage(damager);
            }
        }

        // Temp code for the sake of drawing explosions as gizmo wireframes
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        drawExplosion = true;
        willExplode = false;
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
