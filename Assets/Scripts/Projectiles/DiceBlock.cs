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

    #endregion

    #region Privates
    private Transform upSide;
    private float explosionTimer;
    private bool willExplode;

    private DiceSide faceUpSide;
    #endregion

    #region Unity LifeCycle
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
                Explode(explosionDamage * GetMultiplierFromDiceSide());
            }
        }
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

    public void Roll(Vector3 direction)
    {
        rigid.AddForce(direction.normalized * forceStrength);
        rigid.AddTorque(Random.onUnitSphere * tumbleStrength);
        willExplode = true;
        explosionTimer = 0;
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

    private void Explode(float damage)
    {
        List<IDamageable> damageableList = new List<IDamageable>();

        Collider[] cols = Physics.OverlapSphere(this.transform.position, explosionRadius);
        for (int i = 0; i < cols.Length; i++)
        {

            damageableList.Add(cols[i].GetComponentInChildren<IDamageable>());

        }

        foreach (IDamageable damageable in damageableList)
        {
            Damager damager = new Damager();
            damager.damage = damage;
            damager.impactDirection = this.transform.position;
            damager.damagerType = DamagerType.Dice;
            damageable.TakeDamage(damager);
        }

        Destroy(this.gameObject);
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
