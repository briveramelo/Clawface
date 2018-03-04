﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellPatternController : MonoBehaviour {

    #region 1. Serialized Unity Inspector Fields
    [SerializeField][Range(-360.0f, 360.0f)]private float separationFromForwardVector;
    [SerializeField][Range(0.0f, 100.0f)]   private float bulletSpeed;
    [SerializeField] [Range(0.0f, 100.0f)]  private float bulletDamage;
    [SerializeField][Range(0.0f, 10.0f)]    private float rateOfFire;
    [SerializeField] [Range(0.0f, 10.0f)]   private float bulletOffsetFromOrigin;
    [SerializeField][Range(0.0f, 30.0f)]    private int bulletStrands;
    [SerializeField][Range(0.0f, 360.0f)]   private float separationAngleBetweenStrands;
    [SerializeField]                        private RotateDirection rotationDirection;
    [SerializeField][Range(0.0f, 10.0f)]    private float rotationSpeed;
    [SerializeField] private float bulletLiveTime = 1.0f;
    [SerializeField] private bool animationDriven;
    [HideInInspector]public enum RotateDirection
    {
        Right,
        Left
    };
    [SerializeField] private SFXType shootSound;
    #endregion

    #region 2. Private fields
    private Vector3 referenceVector;
    private Quaternion rotationSeparationFromForward;
    private List<BulletHellStrand> bulletHellStrandList;
    
    private float currentTime;
    private int currentNumberOfStrands = 0;
    private int oldNumberOfStrands = 0;
    private bool createStrandList = true;
    #endregion

    #region 3. Unity Lifecycle
    //Used for Initialization
    private void Awake()
    {
        bulletHellStrandList = new List<BulletHellStrand>();
    }

    // Update is called once per frame
    void Update () {

        CalculateReferenceVector();

        currentTime += Time.deltaTime;

        if (currentTime > rateOfFire && !animationDriven)
        {
            FireBullet();
            currentTime = 0.0f;
        }
        RotateBulletHellController();
	}


    #endregion

    #region 4. Private Methods  
    public void FireBullet()
    {
        UpdateStrands(bulletStrands);
        OrganizeBulletStrands(bulletStrands, separationAngleBetweenStrands);

        //For each stand 
        for (int i = 0; i < bulletHellStrandList.Count; i++)
        {
            GameObject newBullet = ObjectPool.Instance.GetObject(PoolObjectType.EnemyBulletSmall);
            if (newBullet) {
                newBullet.transform.position = transform.position + (bulletHellStrandList[i].movementDirection * bulletOffsetFromOrigin);
                newBullet.GetComponent<BlasterBullet>().transform.forward = bulletHellStrandList[i].movementDirection;
                newBullet.GetComponent<BlasterBullet>().Initialize(bulletLiveTime, bulletSpeed,bulletDamage);
                newBullet.GetComponent<BlasterBullet>().SetShooterType(true);
            }
        }

        SFXManager.Instance.Play(shootSound, transform.position);
    }
    private void RotateBulletHellController()
    {
        transform.Rotate(Vector3.up * rotationSpeed * RotateDirectionValue());
    }
    private float RotateDirectionValue()
    {
        if (rotationDirection == RotateDirection.Left)
            return -1.0f;

        else
            return 1.0f;
    }
    private void CalculateReferenceVector()
    {
        //The reference vector for all other bullet directions
        referenceVector = transform.forward;

        rotationSeparationFromForward = Quaternion.Euler(0, separationFromForwardVector, 0);

        referenceVector = (rotationSeparationFromForward * transform.forward);
    }
    private void UpdateStrands(int numberOfStrands)
    {
        currentNumberOfStrands = numberOfStrands;

        if (oldNumberOfStrands != currentNumberOfStrands)
        {
            bulletHellStrandList.Clear();
            createStrandList = true;
        }

    }
    private void OrganizeBulletStrands(int numberOfStrands, float separation)
    {

        currentNumberOfStrands = numberOfStrands;
        oldNumberOfStrands = currentNumberOfStrands;

        for (int i = 0; i < currentNumberOfStrands; i++)
        {
            Quaternion rotation = Quaternion.identity;

                if (i == 0)
                    rotation = Quaternion.Euler(0, 0, 0);
                else
                {
                    rotation = Quaternion.Euler(0, separation * i, 0);
                }

            if (createStrandList)
            {
                BulletHellStrand strand = new BulletHellStrand(rotation * referenceVector);
                bulletHellStrandList.Add(strand);
                
            }
            else
            {
                bulletHellStrandList[i].SetMovement(rotation * referenceVector);
            }


        }
        //Finished creating the new list
        createStrandList = false;

    }
    #endregion

    public void SetBulletHellProperties(float newSeparationFromForwardVector, float newBulletSpeed, float newBulletDamage, float newRateOfFire, float newBulletOffsetFromOrigin, int newBulletStands, float newSeparationAngleBetweenStands, RotateDirection newRotationDirection, float newRotationSpeed, float newBulletLiveTime,bool newAnimationDriven)
    {
        separationFromForwardVector = newSeparationFromForwardVector;
        bulletSpeed = newBulletSpeed;
        bulletDamage = newBulletDamage;
        rateOfFire = newRateOfFire;
        bulletOffsetFromOrigin = newBulletOffsetFromOrigin;
        bulletStrands = newBulletStands;
        separationAngleBetweenStrands = newSeparationAngleBetweenStands;
        rotationDirection = newRotationDirection;
        rotationSpeed = newRotationSpeed;
        bulletLiveTime = newBulletLiveTime;
        animationDriven = newAnimationDriven;
    }


    #region 5. Public Methods  
    public float GetFireRate()
    {
        return rateOfFire;
    }
    #endregion

}

public class BulletHellStrand
{
    #region 1. Public fields
    public Vector3 movementDirection;
    #endregion

    #region 1. Public methods
    public BulletHellStrand(Vector3 movementDirection)
    {
        this.movementDirection = movementDirection;
    }
    public void SetMovement(Vector3 direction)
    {
        movementDirection = direction;
    }
    #endregion
}
