using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadGunBullet : MonoBehaviour {

#region Private variables
    private float speed;
    private float maxLifeTime;
    private float maxDistance;
    private float damage;
    private bool isReady;
#endregion

    #region Unity lifecycle
    // Use this for initialization
    void Start () {
        isReady = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (isReady)
        {

        }
	}
    #endregion

#region Public functions
    public void Init(float speed, float maxLifeTime, float maxDistance, float damage)
    {
        this.speed = speed;
        this.maxDistance = maxDistance;
        this.maxLifeTime = maxLifeTime;
        this.damage = damage;
        isReady = true;
    }
#endregion

}
