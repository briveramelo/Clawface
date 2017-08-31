using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletStraightFire : MonoBehaviour {

    [SerializeField]
    float bulletSpeed;
    [SerializeField]
    float rateOfFire;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private int bulletArrays;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {



        GameObject newBullet = Instantiate(bulletPrefab, transform) as GameObject;
        bulletPrefab.transform.position = new Vector3(bulletSpeed * Time.deltaTime, 0, 0);

    }
}
