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
    #endregion

    #region Unity LifeCycle
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            AddRandomForce();
        }

        if (CheckGroundedSide(side1))
        {

        }
        else if (CheckGroundedSide(side2))
        {

        }
        else if (CheckGroundedSide(side3))
        {

        }
        else if (CheckGroundedSide(side4))
        {

        }
        else if (CheckGroundedSide(side5))
        {

        }
        else if (CheckGroundedSide(side6))
        {

        } 
        else
        {
            upSide = null;
        }

        if (upSide != null)
        {
            explosionTimer += Time.deltaTime;
            if (willExplode && explosionTimer >= timeTilExplosion)
            {
                Explode(explosionDamage);
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
            damageable.TakeDamage(damage);
        }

        Destroy(this.gameObject);
    }

    #endregion


}
