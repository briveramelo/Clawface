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
    [SerializeField]
    private float growRate;
    #endregion

    #region Private Fields
    private Vector3 initialPosition;
    private Transform player;
    private bool returning;
    private Matrix4x4 TRMatrix;
    private float enemyDistance;
    private float minorAxisLength;
    private float majorAxisLength;
    private Vector3 initialScale;
    private float damageMultiplier;
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
        damageMultiplier = standardDamageMultiplier;
    }
	
	// Update is called once per frame
	void Update () {
        if (isActive) {
            if (getModSpot() == ModSpot.ArmR)
            {
                UpdateBoomerangPosition();
            }else if (getModSpot() == ModSpot.ArmL)
            {
                UpdateBoomerangPosition(-1);
            }
        }
	}

    void UpdateBoomerangPosition(int leftHandMultiplier = 1)
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
            float relativeX = -xCoordinate * leftHandMultiplier;
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
            float relativeX = xCoordinate * leftHandMultiplier;
            transform.position = TRMatrix.MultiplyPoint3x4(new Vector3(relativeX, yCoordinate, zCoordinate));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag != Strings.Tags.PLAYER)
        {
            if(other.tag == Strings.Tags.ENEMY)
            {
                other.gameObject.GetComponent<IDamageable>().TakeDamage(wielderStats.attack * damageMultiplier);
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
            ReleaseBoomerang();
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
        if (!isActive)
        {
            if (isHeld)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, initialScale * 2.0f, growRate);
            }
            else
            {
                damageMultiplier = chargeDamageMultiplier;
                ReleaseBoomerang();
            }
        }
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        isAttached = true;
        this.wielderMovable = wielderMovable;        
        this.wielderStats = wielderStats;
        pickupCollider.enabled = false;
        initialScale = transform.localScale;
    }

    public override void DeActivate()
    {
        isActive = false;
        damageCollider.enabled = false;
        transform.parent = player;
        transform.localPosition = Vector3.zero;
        initialPosition = Vector3.zero;
        player = null;
        transform.forward = wielderMovable.GetForward();
        transform.localScale = initialScale;
        damageMultiplier = standardDamageMultiplier;
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
    private void ReleaseBoomerang()
    {
        isActive = true;
        damageCollider.enabled = true;
        initialPosition = transform.position;
        transform.rotation = Quaternion.identity;
        player = transform.parent;
        transform.parent = null;
        TRMatrix = Matrix4x4.TRS(initialPosition, wielderMovable.GetRotation(), Vector3.one);
        transform.forward = wielderMovable.GetForward();
        majorAxisLength = enemyDistance > maxDistance ? maxDistance / 2.0f : enemyDistance / 2.0f;
    }
    #endregion

    #region Private Structures
    #endregion

}
