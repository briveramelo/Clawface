using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangMod : Mod {


    #region Public fields
    public bool isActive;
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private float maxDistance;
    [SerializeField]
    private float maxWidth;
    [SerializeField]
    private SphereCollider damageCollider;
    [SerializeField]
    private float standardDamageMultiplier;
    [SerializeField]
    private float chargeDamageMultiplier;
    [SerializeField]
    private float movementSpeed;
    #endregion

    #region Private Fields
    private Vector3 initialPosition;
    private Transform player;
    private bool returning;
    private Matrix4x4 TRMatrix;
    private float enemyDistance;
    private float minorAxisLength;
    private float majorAxisLength;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        type = ModType.Boomerang;
        category = ModCategory.Ranged;
        damageCollider.enabled = false;
        if (modCanvas)
        {
            modCanvas.SetActive(false);
        }
        initialPosition = Vector3.zero;
        player = null;
        enemyDistance = Mathf.Infinity;
        minorAxisLength = maxWidth;
    }
	
	// Update is called once per frame
	void Update () {
        if (isActive) {
            if (getModSpot() == ModSpot.ArmR)
            {
                UpdateBoomerangPosition();
            }
        }
	}

    void UpdateBoomerangPosition()
    {
        //Equation of an ellipse x^2/a^2 + y^2/b^2 = 1
        Vector3 relativePosition = TRMatrix.inverse.MultiplyPoint3x4(transform.position);
        float xCoordinate = relativePosition.x;
        float yCoordinate = relativePosition.y;
        float zCoordinate = relativePosition.z;
        zCoordinate += movementSpeed * Time.deltaTime;
        if (zCoordinate > majorAxisLength * 2.0f)
        {
            returning = true;
            zCoordinate = majorAxisLength * 2.0f;
            movementSpeed = -movementSpeed;
            float relativeZ = zCoordinate - majorAxisLength;
            xCoordinate = (Mathf.Sqrt(1 - ((relativeZ) * (relativeZ)) / (majorAxisLength * majorAxisLength)) * minorAxisLength);
            float relativeX = -xCoordinate;
            transform.position = TRMatrix.MultiplyPoint3x4(new Vector3(relativeX, yCoordinate, zCoordinate));
        }
        else if(zCoordinate < 0.0f)
        {
            zCoordinate = 0.0f;
            movementSpeed = -movementSpeed;
            DeActivate();
            returning = false;
        }
        else
        {
            float relativeZ = zCoordinate - majorAxisLength;
            xCoordinate = (Mathf.Sqrt(1 - ((relativeZ) * (relativeZ)) / (majorAxisLength * majorAxisLength)) * minorAxisLength);
            if (returning)
            {
                xCoordinate = -xCoordinate;
            }
            float relativeX = xCoordinate;
            transform.position = TRMatrix.MultiplyPoint3x4(new Vector3(relativeX, yCoordinate, zCoordinate));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag != Strings.Tags.PLAYER)
        {
            if(other.tag == Strings.Tags.ENEMY)
            {
                other.gameObject.GetComponent<IDamageable>().TakeDamage(wielderStats.attack * standardDamageMultiplier);
            }
            else
            {
                //TODO: Figure out what to do
            }
        }
    }
    #endregion

    #region Public Methods
    public override void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            damageCollider.enabled = true;
            initialPosition = transform.position;
            transform.rotation = Quaternion.identity;
            player = transform.parent;
            transform.parent = null;
            TRMatrix = Matrix4x4.TRS(initialPosition, wielderMovable.GetRotation(), Vector3.one);
            majorAxisLength = enemyDistance > maxDistance ? maxDistance / 2.0f : enemyDistance / 2.0f;
        }
    }

    public override void ActivateModCanvas()
    {
        if (modCanvas && !isAttached)
        {
            modCanvas.SetActive(true);
        }        
    }

    public override void AlternateActivate(bool isHeld, float holdTime)
    {
        
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        isAttached = true;
        this.wielderMovable = wielderMovable;        
        this.wielderStats = wielderStats;
        pickupCollider.enabled = false;        
    }

    public override void DeActivate()
    {
        isActive = false;
        damageCollider.enabled = false;
        transform.parent = player;
        transform.localPosition = Vector3.zero;
        initialPosition = Vector3.zero;
        player = null;
    }

    public override void DeactivateModCanvas()
    {
        if (modCanvas)
        {
            modCanvas.SetActive(false);
        }
    }

    public override void DetachAffect()
    {
        isAttached = false;
        pickupCollider.enabled = true;
        damageCollider.enabled = false;
        wielderMovable = null;
        setModSpot(ModSpot.Default);
    }

    public void SetEnemyDistance(float distance)
    {
        enemyDistance = distance;
    }
    #endregion

    #region Private Methods
    #endregion

    #region Private Structures
    #endregion

}
