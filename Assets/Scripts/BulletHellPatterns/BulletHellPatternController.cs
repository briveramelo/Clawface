using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellPatternController : MonoBehaviour {

    #region 1. Serialized Unity Inspector Fields
    [SerializeField][Range(-360.0f, 360.0f)]private float separationFromForwardVector;
    [SerializeField][Range(0.0f, 100.0f)]   private float bulletSpeed;
    [SerializeField][Range(0.0f, 10.0f)]    private float rateOfFire;
    [SerializeField]                        private GameObject bulletPrefab;
    [SerializeField][Range(0.0f, 30.0f)]    private int bulletStrands;
    [SerializeField][Range(0.0f, 360.0f)]   private float separationAngleBetweenStrands;
    [SerializeField]                        private RotateDirection rotationDirection;
    [SerializeField][Range(0.0f, 10.0f)]    private float rotationSpeed;
    #endregion


    #region 2. Private fields
    private Vector3 referenceVector;
    private Quaternion rotationSeparationFromForward;
    private List<BulletHellStrand> bulletHellStrandList;
    private enum RotateDirection
    {
        Right,
        Left
    };
    private float currentTime;
    private int currentNumberOfStrands = 0;
    private int oldNumberOfStrands = 0;
    private bool createStrandList = true;
    #endregion

    #region 3. Unity Lifecycle
    //Used for Initialization
    private void Start()
    {
        bulletHellStrandList = new List<BulletHellStrand>();
    }

    // Update is called once per frame
    void Update () {

        CalculateReferenceVector();

        currentTime += Time.deltaTime;

        if (currentTime > rateOfFire)
        {
            UpdateStrands(bulletStrands);
            OrganizeBulletStrands(bulletStrands, separationAngleBetweenStrands);
            FireBullet();
            currentTime = 0.0f;
        }

        RotateBulletHellController();
	}


    #endregion

    #region 4. Private Methods  
    private void FireBullet()
    {
        //For each stand 
        for (int i = 0; i < bulletHellStrandList.Count; i++)
        {
            SFXManager.Instance.Play(SFXType.BlasterShoot, transform.position);
            GameObject newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;
            newBullet.AddComponent<BulletBehavior>().AssignBulletValues(bulletHellStrandList[i].movementDirection, bulletSpeed);
        }
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
