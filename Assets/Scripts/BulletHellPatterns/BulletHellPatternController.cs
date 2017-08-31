using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellPatternController : MonoBehaviour {

    [SerializeField]
    float bulletSpeed;
    [SerializeField]
    float rateOfFire;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private int bulletStrands;

    private List<BulletHellStrand> bulletHellStrandList;

    [SerializeField]
    private float separationAngle;

    //Rotation types
    private enum RotateDirection
    {
        Right,
        Left
    };

    [SerializeField]
    private RotateDirection rotationDirection;

    [SerializeField]
    private float rotationSpeed;


    private Vector3 referenceVector;

    private float currentTime;

    private void Start()
    {
        bulletHellStrandList = new List<BulletHellStrand>();
    }


    // Update is called once per frame
    void Update () {

        //The reference vector for all other bullet directions
        referenceVector = transform.forward;

        currentTime += Time.deltaTime;

        if (currentTime > rateOfFire)
        {

            OrganizeBulletStrands(bulletStrands, separationAngle);

            FireBullet();

            currentTime = 0.0f;
        }

        RotateBulletHellController();
	}

    private void FireBullet()
    {

        for (int i = 0; i < bulletHellStrandList.Count; i++)
        {
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

    private void OrganizeBulletStrands(int numberOfStrands, float separation)
    {
        
            for (int i = 0; i < numberOfStrands; i++)
            {
                Quaternion rotation = Quaternion.identity;
                if (i % 2 != 0)
                {
                rotation = Quaternion.Euler(0,-separation + (-separation * (i - 1)), 0);
                }
                else {

                if (i == 0)
                    rotation = Quaternion.Euler(0, separation + (separation * i), 0);
                else {
                    rotation = Quaternion.Euler(0, separation + (separation * i -1), 0);
                }
                }

            if (bulletStrands != bulletHellStrandList.Count)
            {
                BulletHellStrand strand = new BulletHellStrand(rotation * transform.forward);
                bulletHellStrandList.Add(strand);
            }
            else {
                bulletHellStrandList[i].SetMovement(rotation * transform.forward);
            }


        }
        
    }

}
