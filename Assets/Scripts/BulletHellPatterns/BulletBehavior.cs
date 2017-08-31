using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour {

    Vector3 movementVector;
    float bulletSpeed;


    private void Start()
    {
        
    }


    public void AssignBulletValues(Vector3 forwardDirection, float speed)
    {
        movementVector = forwardDirection;
        bulletSpeed = speed;
    }

    void Update()
    {
        transform.position += movementVector * bulletSpeed * Time.deltaTime;
    }



}
