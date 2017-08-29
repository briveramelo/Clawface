using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellPattern1 : MonoBehaviour {

    [SerializeField]
    private GameObject bulletPrefab;


    float amplitudeX = 10.0f;
    float amplitudeY = 5.0f;
    float omegaX = 1.0f;
    float omegaY = 5.0f;
    float index;
    public float k;
    public int n;
    public int d;

    public void Start()
    {
        k = (float) n / d;
    }

    public void Update()
    {


        index += Time.deltaTime;
        float theta = index % (2 * Mathf.PI);

        float x = amplitudeX * Mathf.Cos(k * theta * index) * Mathf.Cos(theta * index); 
        float z = amplitudeX * Mathf.Cos(k * theta * index) * Mathf.Sin(theta * index);


        GameObject newBullet = Instantiate(bulletPrefab,transform) as GameObject;
        bulletPrefab.transform.position= new Vector3(x, 0, z);
    }

    Vector2 CartesianToPolar(Vector3 point)
    {
     Vector2 polar;
 
     //calc longitude
     polar.y = Mathf.Atan2(point.x, point.z);

    //this is easier to write and read than sqrt(pow(x,2), pow(y,2))!
    float xzLen = new Vector2(point.x,point.z).magnitude;
    
    //atan2 does the magic
    polar.x = Mathf.Atan2(-point.y,xzLen);
 
     //convert to deg
     polar *= Mathf.Rad2Deg;
 
     return polar;
    }
}
